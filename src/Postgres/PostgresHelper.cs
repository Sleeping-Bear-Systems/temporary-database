using System.Globalization;
using Npgsql;

namespace SleepingBearSystems.TemporaryDatabase.Postgres;

/// <summary>
/// Helper methods for Postgres databases.
/// </summary>
public static class PostgresHelper
{
    /// <summary>
    /// Creates a Postgres database.
    /// </summary>
    public static void CreateDatabase(string connectionString, string database)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        var cmdText = string.Format(CultureInfo.InvariantCulture, "CREATE DATABASE {0};", database);
        using var command = new NpgsqlCommand(cmdText, connection);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Drops a Postgres database.
    /// </summary>
    public static void DropDatabase(string connectionString, string database)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var cmdText = string.Format(
            CultureInfo.InvariantCulture,
            "DROP DATABASE IF EXISTS {0} WITH (FORCE);",
            database);
        using var command = new NpgsqlCommand(cmdText, connection);
        command.ExecuteNonQuery();
    }
}
