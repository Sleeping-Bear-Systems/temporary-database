using System.Globalization;
using MySql.Data.MySqlClient;
using SleepingBearSystems.TemporaryDatabase.Common;

namespace SleepingBearSystems.TemporaryDatabase.MySql;

/// <summary>
/// Helper methods for MySQL databases.
/// </summary>
public static class MySqlHelper
{
    /// <summary>
    /// Creates a MySQL database.
    /// </summary>
    public static CreateDatabaseResult CreateDatabase(
        string connectionString,
        string database,
        CreateDatabaseOptions? options = default)
    {
        var masterConnectionString = GetMasterConnectionString(connectionString);
        using var connection = new MySqlConnection(masterConnectionString);
        connection.Open();

        var cmdText = string.Format(CultureInfo.InvariantCulture, "CREATE DATABASE {0};", database);
        using var command = new MySqlCommand(cmdText, connection);
        command.ExecuteNonQuery();
        return new CreateDatabaseResult(masterConnectionString, connectionString, database);
    }

    /// <summary>
    /// Drops a MySQL database.
    /// </summary>
    public static void DropDatabase(CreateDatabaseResult result)
    {
        using var connection = new MySqlConnection(result.MasterConnectionString);
        connection.Open();
        var cmdText = string.Format(
            CultureInfo.InvariantCulture,
            "DROP DATABASE IF EXISTS {0};",
            result.Database);
        using var command = new MySqlCommand(cmdText, connection);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Checks if a database exists.
    /// </summary>
    public static bool CheckDatabaseExists(string masterConnectionString, string database, bool ignoreCase = true)
    {
        using var connection = new MySqlConnection(masterConnectionString);
        connection.Open();
        using var command = new MySqlCommand("SHOW DATABASES;", connection);
        using var reader = command.ExecuteReader();
        var databases = new List<string>();
        while (reader.Read())
        {
            databases.Add(reader.GetString(0));
        }

        return databases.Contains(database, ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
    }
    /// <summary>
    /// Gets the master database connection string.
    /// </summary>
    private static string GetMasterConnectionString(string connectionString) =>
        new MySqlConnectionStringBuilder(connectionString)
        {
            Database = "mysql"
        }.ToString();
}
