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
        var validVariable = variable ?? "SBS_TEST_SERVER_MYSQL";
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
    ///     Creates a MySQL database.
    /// </summary>
    private static async Task<string> CreateDatabaseAsync(
        string? rawConnectionString,
        string database,
        DatabaseOptions? options = null)
    {
        // set connection string
        var validOptions = options ?? DatabaseOptions.Defaults;
        var connectionStringBuilder = new MySqlConnectionStringBuilder(rawConnectionString ?? string.Empty)
        {
            SslMode = validOptions.SslMode,
            Database = database
        };
        var databaseConnectionString = connectionStringBuilder.ToString();
        connectionStringBuilder.Database = "mysql";

        // create database
        await using var connection = new MySqlConnection(connectionStringBuilder.ToString());
        await connection.OpenAsync();
        var stringBuilder = new StringBuilder()
            .Append(CultureInfo.InvariantCulture, $"CREATE DATABASE {database}")
            .AppendIf(
                !string.IsNullOrWhiteSpace(validOptions.CharacterSet),
                $" CHARACTER SET = {validOptions.CharacterSet}")
            .AppendIf(!string.IsNullOrWhiteSpace(validOptions.Collation), $" COLLATE {validOptions.Collation}")
            .Append(';');
        await using var command = new MySqlCommand(stringBuilder.ToString(), connection);
        await command.ExecuteNonQueryAsync();
        return databaseConnectionString;
    }

    /// <summary>
    ///     Drops a MySQL database.
    /// </summary>
    private static async Task DropDatabaseAsync(string connectionString)
    {
        // set up connection string
        var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);
        var database = connectionStringBuilder.Database;
        connectionStringBuilder.Database = "mysql";

        // drop database
        await using var connection = new MySqlConnection(connectionStringBuilder.ToString());
        await connection.OpenAsync();
        var cmdText = $"DROP DATABASE IF EXISTS {database};";
        await using var command = new MySqlCommand(cmdText, connection);
        await command.ExecuteNonQueryAsync();
    }
}