using System.Globalization;
using Npgsql;

namespace SleepingBearSystems.TemporaryDatabase.Postgres;

/// <summary>
/// Temporary database guard for Postgres databases.
/// </summary>
public sealed class TemporaryDatabaseGuard : IDisposable
{
    private TemporaryDatabaseGuard(string database, string connectionString, string masterConnectionString)
    {
        this._database = database;
        this.ConnectionString = connectionString;
        this._masterConnectionString = masterConnectionString;
    }

    /// <inheritdoc cref="IDisposable"/>
    public void Dispose()
    {
        using var connection = new NpgsqlConnection(this._masterConnectionString);
        connection.Open();
        var cmdText = string.Format(
            CultureInfo.InvariantCulture,
            "DROP DATABASE IF EXISTS {0} WITH (FORCE);",
            this._database);
        using var command = new NpgsqlCommand(cmdText, connection);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// The connection string.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard Create(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var database = builder.Database;
        if (string.IsNullOrWhiteSpace(database))
        {
            database = Helper.GenerateRandomDatabaseName();
        }

        builder.Database = "postgres";
        var masterConnectionString = builder.ToString();

        using var connection = new NpgsqlConnection(masterConnectionString);
        connection.Open();

        var cmdText = string.Format(CultureInfo.InvariantCulture, "CREATE DATABASE {0};", database);
        using var command = new NpgsqlCommand(cmdText, connection);
        command.ExecuteNonQuery();

        return new TemporaryDatabaseGuard(database, connectionString, masterConnectionString);
    }

    private readonly string _masterConnectionString;

    private readonly string _database;
}
