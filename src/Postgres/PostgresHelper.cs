using System.Globalization;
using Npgsql;
using SleepingBearSystems.TemporaryDatabase.Common;

namespace SleepingBearSystems.TemporaryDatabase.Postgres;

/// <summary>
/// Helper methods for Postgres databases.
/// </summary>
public static class PostgresHelper
{
    /// <summary>
    /// Creates a Postgres database.
    /// </summary>
    public static CreateDatabaseResult CreateDatabase(
        string connectionString,
        string database,
        CreateDatabaseOptions? options = default)
    {
        var masterConnectionString = GetMasterConnectionString(connectionString);
        using var connection = new NpgsqlConnection(masterConnectionString);
        connection.Open();

        var cmdText = string.Format(CultureInfo.InvariantCulture, "CREATE DATABASE {0};", database);
        using var command = new NpgsqlCommand(cmdText, connection);
        command.ExecuteNonQuery();
        return new CreateDatabaseResult(masterConnectionString, connectionString, database);
    }

    /// <summary>
    /// Drops a Postgres database.
    /// </summary>
    public static void DropDatabase(CreateDatabaseResult result)
    {
        using var connection = new NpgsqlConnection(result.MasterConnectionString);
        connection.Open();
        var cmdText = string.Format(
            CultureInfo.InvariantCulture,
            "DROP DATABASE IF EXISTS {0} WITH (FORCE);",
            result.Database);
        using var command = new NpgsqlCommand(cmdText, connection);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Gets the master database connection string.
    /// </summary>
    private static string GetMasterConnectionString(string connectionString) =>
        new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres"
        }.ToString();
}
