using System.Globalization;
using System.Text;
using Npgsql;
using SleepingBearSystems.TemporaryDatabase.Common;

namespace SleepingBearSystems.TemporaryDatabase.Postgres;

/// <summary>
///     Helper methods for Postgres databases.
/// </summary>
public static class PostgresHelper
{
    /// <summary>
    ///     Creates a Postgres database.
    /// </summary>
    public static DatabaseInformation CreateDatabase(
        string connectionString,
        string database,
        DatabaseOptions? options = default)
    {
        var validOptions = options ?? DatabaseOptions.Defaults;
        var masterConnectionString = GetMasterConnectionString(connectionString);
        using var connection = new NpgsqlConnection(masterConnectionString);
        connection.Open();

        var builder = new StringBuilder()
            .Append(CultureInfo.InvariantCulture, $"CREATE DATABASE {database}");
        if (!string.IsNullOrWhiteSpace(validOptions.Collation))
        {
            // TODO - Add support for Postgres collation
        }

        builder.Append(';');
        using var command = new NpgsqlCommand(builder.ToString(), connection);
        command.ExecuteNonQuery();
        return new DatabaseInformation(connectionString, database);
    }

    /// <summary>
    ///     Drops a Postgres database.
    /// </summary>
    public static void DropDatabase(DatabaseInformation information)
    {
        using var connection = new NpgsqlConnection(GetMasterConnectionString(information.ConnectionString));
        connection.Open();
        var cmdText = string.Format(
            CultureInfo.InvariantCulture,
            "DROP DATABASE IF EXISTS {0} WITH (FORCE);",
            information.Database);
        using var command = new NpgsqlCommand(cmdText, connection);
        command.ExecuteNonQuery();
    }

    /// <summary>
    ///     Checks if a database exists.
    /// </summary>
    public static bool CheckDatabaseExists(DatabaseInformation information, string database, bool ignoreCase = true)
    {
        var masterConnectionString = GetMasterConnectionString(information.ConnectionString);
        using var connection = new NpgsqlConnection(masterConnectionString);
        connection.Open();
        // ReSharper disable once StringLiteralTypo
        using var command = new NpgsqlCommand("SELECT datname FROM pg_database", connection);
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
        return new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres"
        }.ToString();
    }
}
