using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using MySql.Data.MySqlClient;
using SleepingBear.TemporaryDatabase.Common;

namespace SleepingBear.TemporaryDatabase.MySql;

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
        await DropDatabaseAsync(this._connectionString);
    }

    /// <summary>
    ///     Factory method for creating a <see cref="TemporaryDatabaseGuard" /> instance.
    /// </summary>
    public static async Task<TemporaryDatabaseGuard> FromEnvironmentVariable(
        string? variable,
        DatabaseOptions? options = null)
    {
        var validVariable = variable ?? "SBS_TEST_SERVER_MYSQL";
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
        var result = await CreateDatabaseAsync(connectionString, DatabaseHelper.GenerateDatabaseName(), options);
        return new TemporaryDatabaseGuard(result);
    }

    /// <summary>
    ///     Creates a MySQL database.
    /// </summary>
    private static async Task<string> CreateDatabaseAsync(
        string connectionString,
        string database,
        DatabaseOptions? options = null)
    {
        // set connection string
        var validOptions = options ?? DatabaseOptions.Defaults;
        var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString)
        {
            SslMode = validOptions.SslMode,
            Database = database
        };
        var databaseConnectionString = connectionStringBuilder.ToString();
        connectionStringBuilder.Database = "mysql";

        // create database
        await using var connection = new MySqlConnection(connectionStringBuilder.ToString());
        await connection.OpenAsync();
        var builder = new StringBuilder()
            .Append(CultureInfo.InvariantCulture, $"CREATE DATABASE {database}")
            .AppendIf(
                !string.IsNullOrWhiteSpace(validOptions.CharacterSet),
                $" CHARACTER SET = {validOptions.CharacterSet}")
            .AppendIf(!string.IsNullOrWhiteSpace(validOptions.Collation), $" COLLATE {validOptions.Collation}")
            .Append(';');
        await using var command = new MySqlCommand(builder.ToString(), connection);
        await command.ExecuteNonQueryAsync();
        return databaseConnectionString;
    }

    /// <summary>
    ///     Drops a MySQL database.
    /// </summary>
    private static async Task DropDatabaseAsync(string connectionString)
    {
        var builder = new MySqlConnectionStringBuilder(connectionString);
        var database = builder.Database;
        builder.Database = "mysql";

        await using var connection = new MySqlConnection(builder.ToString());
        await connection.OpenAsync();
        var cmdText = string.Format(
            CultureInfo.InvariantCulture,
            (string)"DROP DATABASE IF EXISTS {0};",
            database);
        await using var command = new MySqlCommand(cmdText, connection);
        await command.ExecuteNonQueryAsync();
    }
}