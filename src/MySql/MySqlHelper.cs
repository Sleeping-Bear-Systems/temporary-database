using System.Globalization;
using MySql.Data.MySqlClient;

namespace SleepingBearSystems.TemporaryDatabase.MySql;

/// <summary>
/// Helper methods for MySQL databases.
/// </summary>
public static class MySqlHelper
{
    /// <summary>
    /// Creates a MySQL database.
    /// </summary>
    public static void CreateDatabase(string connectionString, string database)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        var cmdText = string.Format(CultureInfo.InvariantCulture, "CREATE DATABASE {0};", database);
        using var command = new MySqlCommand(cmdText, connection);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Drops a MySQL database.
    /// </summary>
    public static void DropDatabase(string connectionString, string database)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();
        var cmdText = string.Format(
            CultureInfo.InvariantCulture,
            "DROP DATABASE IF EXISTS {0};",
            database);
        using var command = new MySqlCommand(cmdText, connection);
        command.ExecuteNonQuery();
    }
}
