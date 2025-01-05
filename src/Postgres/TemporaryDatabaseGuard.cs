using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Npgsql;
using SleepingBear.TemporaryDatabase.Common;

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
        var validVariable = variable ?? "SBS_TEST_SERVER_POSTGRES";
        return await FromConnectionStringAsync(
            Environment.GetEnvironmentVariable(validVariable) ?? string.Empty,
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
            DatabaseHelper.GenerateDatabaseName(),
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
        rawConnectionString = ConvertFromUri(rawConnectionString);
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(rawConnectionString ?? string.Empty)
        {
            Database = database
        };
        var databaseConnectionString = connectionStringBuilder.ToString();
        connectionStringBuilder.Database = "postgres";

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

    private static string? ConvertFromUri(string? rawConnectionString)
    {
        if (string.IsNullOrWhiteSpace(rawConnectionString) || !rawConnectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return rawConnectionString;
        }

        var uri = new Uri(rawConnectionString);
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port,
            Username = uri.UserInfo.Split(':')[0],
            Password = uri.UserInfo.Split(':')[1],
            Database = uri.LocalPath.TrimStart('/'),
        };
        return connectionStringBuilder.ToString();

    }
}