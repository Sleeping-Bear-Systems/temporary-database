﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Npgsql;
using SleepingBear.Functional.Common;
using SleepingBear.Functional.Errors;
using SleepingBear.Functional.Monads;
using SleepingBear.Functional.Validation;

namespace SleepingBear.TemporaryDatabase.Postgres;

/// <summary>
///     Temporary database guard for Postgres databases.
/// </summary>
[SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task")]
[SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities")]
public sealed class TemporaryDatabaseGuard : IAsyncDisposable
{
    private TemporaryDatabaseGuard(string connectionString)
    {
        this.ConnectionString = connectionString;
    }

    /// <summary>
    ///     Connection string.
    /// </summary>
    public string ConnectionString { get; }

    /// <inheritdoc cref="IAsyncDisposable" />
    public async ValueTask DisposeAsync()
    {
        await DropDatabaseAsync(this.ConnectionString);
    }

    /// <summary>
    ///     Factory method for creating a <see cref="TemporaryDatabaseGuard" /> instance.
    /// </summary>
    public static async Task<TemporaryDatabaseGuard> FromEnvironmentVariableAsync(
        string? variable = null,
        DatabaseOptions? options = null)
    {
        var validVariable = variable.IfNull("SBS_TEST_SERVER_POSTGRES");
        return await FromConnectionStringAsync(
            Environment.GetEnvironmentVariable(validVariable).IfNull(),
            options);
    }

    /// <summary>
    ///     Factory method for creating a <see cref="TemporaryDatabaseGuard" /> instance.
    /// </summary>
    public static async Task<TemporaryDatabaseGuard> FromConnectionStringAsync(
        string? rawConnectionString,
        DatabaseOptions? options = null)
    {
        var connectionString = await CreateDatabaseAsync(
            rawConnectionString,
            $"sbs_tmp_{Guid.NewGuid():N}",
            options);
        return new TemporaryDatabaseGuard(connectionString);
    }

    /// <summary>
    ///     Creates a Postgres database.
    /// </summary>
    private static async Task<string> CreateDatabaseAsync(
        string? rawConnectionString,
        string database,
        DatabaseOptions? options = null)
    {
        // set connection string
        var validOptions = options ?? DatabaseOptions.Defaults;
        var (connectionStringBuilder, databaseConnectionString) = ConvertFromUri(rawConnectionString)
            .Bind(connectionString =>
            {
                try
                {
                    return new NpgsqlConnectionStringBuilder(connectionString)
                    {
                        Database = database,
                        SslMode = validOptions.SslMode
                    };
                }
                catch (KeyNotFoundException ex)
                {
                    return ex.ToExceptionError().ToResultError<NpgsqlConnectionStringBuilder>();
                }
                catch (FormatException ex)
                {
                    return ex.ToExceptionError().ToResultError<NpgsqlConnectionStringBuilder>();
                }
                catch (ArgumentException ex)
                {
                    return ex.ToExceptionError().ToResultError<NpgsqlConnectionStringBuilder>();
                }
            })
            .Bind(builder =>
            {
                var databaseConnectionString = builder.ConnectionString;
                builder.Database = "postgres";
                return (builder, databaseConnectionString).ToResultOk();
            })
            .MatchOrThrow(error =>
            {
                const string message = "Invalid connection string.";
                return error switch
                {
                    ExceptionError ex => new InvalidOperationException(message, ex.Exception),
                    _ => new InvalidOperationException(message)
                };
            });


        // create database
        await using var connection = new NpgsqlConnection(connectionStringBuilder.ToString());
        await connection.OpenAsync();
        var stringBuilder = new StringBuilder()
            .Append(CultureInfo.InvariantCulture, $"CREATE DATABASE {database}")
            .AppendIf(!string.IsNullOrWhiteSpace(validOptions.Encoding), $" ENCODING '{validOptions.Encoding}'")
            .AppendIf(!string.IsNullOrWhiteSpace(validOptions.Collation), $" LC_COLLATE '{validOptions.Collation}'")
            .AppendIf(!string.IsNullOrWhiteSpace(validOptions.CType), $"LC_CTYPE '{validOptions.CType}'")
            .Append(';');
        await using var command = new NpgsqlCommand(stringBuilder.ToString(), connection);
        await command.ExecuteNonQueryAsync();
        return databaseConnectionString;
    }

    /// <summary>
    ///     Drops a Postgres database.
    /// </summary>
    private static async Task DropDatabaseAsync(string connectionString)
    {
        // set up connection string
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        var database = connectionStringBuilder.Database;
        connectionStringBuilder.Database = "postgres";

        // drop database
        await using var connection = new NpgsqlConnection(connectionStringBuilder.ToString());
        await connection.OpenAsync();
        var cmdText = $"DROP DATABASE IF EXISTS {database} WITH (FORCE);";
        await using var command = new NpgsqlCommand(cmdText, connection);
        await command.ExecuteNonQueryAsync();
    }

    internal static Result<string> ConvertFromUri(string? rawConnectionString)
    {
        return rawConnectionString
            .AsToken(() => new InvalidFormatError())
            .BindIf(
                v => v.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase),
                ConvertUriToConnectionString,
                ValidateConnectionString);

        static Result<string> ValidateConnectionString(string value)
        {
            return value.TryCatch(
                x => new NpgsqlConnectionStringBuilder(x).ConnectionString.ToResultOk(),
                ex => ex.ExceptionHandler<ArgumentException, FormatException, KeyNotFoundException>());
        }

        static Result<string> ConvertUriToConnectionString(string value)
        {
            return value.TryCatch(
                v => new Uri(v)
                    .Pipe(uri => new NpgsqlConnectionStringBuilder
                    {
                        Host = uri.Host,
                        Port = uri.Port,
                        Username = uri.UserInfo.Split(':')[0],
                        Password = uri.UserInfo.Split(':')[1],
                        Database = uri.LocalPath.TrimStart('/')
                    }.ConnectionString)
                    .ToResultOk(),
                ex =>
                    ex.ExceptionHandler<UriFormatException, ArgumentException, FormatException,
                        KeyNotFoundException>());
        }
    }
}