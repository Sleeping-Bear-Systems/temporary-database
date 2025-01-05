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
    private readonly string _connectionString;

    private TemporaryDatabaseGuard(string connectionString)
    {
        this._connectionString = connectionString;
    }

    /// <inheritdoc cref="IAsyncDisposable" />
    public async ValueTask DisposeAsync()
    {
        await DropDatabase(this._connectionString).ConfigureAwait(false);
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
        string connectionString,
        DatabaseOptions? options = null)
    {
        var databaseConnectionString = await CreateDatabaseAsync(
            connectionString,
            DatabaseHelper.GenerateDatabaseName(),
            options).ConfigureAwait(false);
        return new TemporaryDatabaseGuard(databaseConnectionString);
    }

    /// <summary>
    ///     Creates a Postgres database.
    /// </summary>
    private static async Task<string> CreateDatabaseAsync(
        string connectionString,
        string database,
        DatabaseOptions? options = null)
    {
        // set connection string
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = database
        };
        var databaseConnectionString = connectionStringBuilder.ToString();
        connectionStringBuilder.Database = "postgres";
        var masterConnectionString = connectionStringBuilder.ToString();

        // create database
        await using var connection = new NpgsqlConnection(masterConnectionString);
        await connection.OpenAsync();
        var validOptions = options ?? DatabaseOptions.Defaults;
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
    private static async Task DropDatabase(string connectionString)
    {
        // set up connection string
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var database = builder.Database;
        builder.Database = "postgres";

        // drop database
        await using var connection = new NpgsqlConnection(builder.ToString());
        await connection.OpenAsync();
        var cmdText = $"DROP DATABASE IF EXISTS {database} WITH (FORCE);";
        await using var command = new NpgsqlCommand(cmdText, connection);
        await command.ExecuteNonQueryAsync();
    }
}