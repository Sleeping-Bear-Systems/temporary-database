using System.Globalization;
using System.Text;
using MySql.Data.MySqlClient;
using SleepingBearSystems.TemporaryDatabase.Common;

namespace SleepingBearSystems.TemporaryDatabase.MySql;

/// <summary>
///     Helper methods for MySQL databases.
/// </summary>
public static class MySqlHelper
{
    /// <summary>
    ///     Creates a MySQL database.
    /// </summary>
    public static DatabaseInformation CreateDatabase(
        string connectionString,
        string database,
        DatabaseOptions? options = default)
    {
        var validOptions = options ?? DatabaseOptions.Defaults;
        var masterConnectionString = GetMasterConnectionString(connectionString);
        using var connection = new MySqlConnection(masterConnectionString);
        connection.Open();

        var builder = new StringBuilder()
            .Append(CultureInfo.InvariantCulture, $"CREATE DATABASE {database}");
        if (!string.IsNullOrWhiteSpace(validOptions.CharacterSet))
        {
            builder.Append(CultureInfo.InvariantCulture, $" CHARACTER SET {validOptions.CharacterSet}");
        }

        if (!string.IsNullOrWhiteSpace(validOptions.Collation))
        {
            builder.Append(CultureInfo.InvariantCulture, $" COLLATE {validOptions.Collation}");
        }

        if (validOptions.Encrypted)
        {
            builder.Append(CultureInfo.InvariantCulture, $" ENCRYPTED 'Y'");
        }

        builder.Append(';');

        using var command = new MySqlCommand(builder.ToString(), connection);
        command.ExecuteNonQuery();
        return new DatabaseInformation(connectionString, database);
    }

    /// <summary>
    ///     Drops a MySQL database.
    /// </summary>
    public static void DropDatabase(DatabaseInformation information)
    {
        using var connection = new MySqlConnection(GetMasterConnectionString(information.ConnectionString));
        connection.Open();
        var cmdText = string.Format(
            CultureInfo.InvariantCulture,
            "DROP DATABASE IF EXISTS {0};",
            information.Database);
        using var command = new MySqlCommand(cmdText, connection);
        command.ExecuteNonQuery();
    }

    /// <summary>
    ///     Checks if a database exists.
    /// </summary>
    public static bool CheckDatabaseExists(DatabaseInformation information, string database, bool ignoreCase = true)
    {
        using var connection = new MySqlConnection(GetMasterConnectionString(information.ConnectionString));
        connection.Open();
        using var command = new MySqlCommand("SHOW DATABASES;", connection);
        using var reader = command.ExecuteReader();
        var databases = new List<string>();
        while (reader.Read()) databases.Add(reader.GetString(0));

        return databases.Contains(database, ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
    }

    /// <summary>
    ///     Gets the master database connection string.
    /// </summary>
    public static string GetMasterConnectionString(string connectionString)
    {
        return new MySqlConnectionStringBuilder(connectionString)
        {
            Database = "mysql"
        }.ToString();
    }
}
