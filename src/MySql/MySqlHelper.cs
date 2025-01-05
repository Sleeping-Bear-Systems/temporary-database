using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using MySql.Data.MySqlClient;
using SleepingBearSystems.TemporaryDatabase.Common;

namespace SleepingBearSystems.TemporaryDatabase.MySql;

/// <summary>
///     Helper methods for MySQL databases.
/// </summary>
internal static class MySqlHelper
{
    /// <summary>
    ///     Creates a MySQL database.
    /// </summary>
    [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static DatabaseInformation CreateDatabase(
        string connectionString,
        string database,
        DatabaseOptions? options = null)
    {
        var validOptions = options ?? DatabaseOptions.Defaults;
        var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString)
        {
            SslMode = validOptions.SslMode,
            Database = database
        };

        var databaseConnectionString = connectionStringBuilder.ToString();

        connectionStringBuilder.Database = "mysql";
        var masterConnectionString = connectionStringBuilder.ToString();
        using var connection = new MySqlConnection(masterConnectionString);
        connection.Open();

        var builder = new StringBuilder()
            .Append(CultureInfo.InvariantCulture, $"CREATE DATABASE {database}");
        if (!string.IsNullOrWhiteSpace(validOptions.CharacterSet))
        {
            builder.Append(CultureInfo.InvariantCulture, $" CHARACTER SET = {validOptions.CharacterSet}");
        }

        if (!string.IsNullOrWhiteSpace(validOptions.Collation))
        {
            builder.Append(CultureInfo.InvariantCulture, $" COLLATE {validOptions.Collation}");
        }

        builder.Append(';');

        using var command = new MySqlCommand(builder.ToString(), connection);
        command.ExecuteNonQuery();
        return new DatabaseInformation(databaseConnectionString, database);
    }

    /// <summary>
    ///     Drops a MySQL database.
    /// </summary>
    [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static void DropDatabase(this DatabaseInformation information)
    {
        using var connection = new MySqlConnection(information.ToMasterConnectionString());
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
    public static bool CheckDatabaseExists(this DatabaseInformation information, string database,
        bool ignoreCase = true)
    {
        using var connection = new MySqlConnection(information.ToMasterConnectionString());
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

    private static string ToMasterConnectionString(this DatabaseInformation information)
    {
        return new MySqlConnectionStringBuilder(information.ConnectionString)
        {
            Database = "mysql"
        }.ToString();
    }
}