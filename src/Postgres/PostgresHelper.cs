using System.Globalization;
using System.Text;
using Npgsql;
using SleepingBearSystems.TemporaryDatabase.Common;

namespace SleepingBearSystems.TemporaryDatabase.Postgres;

/// <summary>
///     Helper methods for Postgres databases.
/// </summary>
internal static class PostgresHelper
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
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = database
        };
        var databaseConnectionString = connectionStringBuilder.ToString();

        connectionStringBuilder.Database = "postgres";
        var masterConnectionString = connectionStringBuilder.ToString();
        using var connection = new NpgsqlConnection(masterConnectionString);
        connection.Open();

        var builder = new StringBuilder()
            .Append(CultureInfo.InvariantCulture, $"CREATE DATABASE {database}");
        if (!string.IsNullOrWhiteSpace(validOptions.Encoding))
        {
            builder.Append(CultureInfo.InvariantCulture, $" ENCODING '{validOptions.Encoding}'");
        }

        if (!string.IsNullOrWhiteSpace(validOptions.Collation))
        {
            builder.Append(CultureInfo.InvariantCulture, $" LC_COLLATE '{validOptions.Collation}'");
        }

        if (!string.IsNullOrWhiteSpace(validOptions.CType))
        {
            builder.Append(CultureInfo.InvariantCulture, $"LC_CTYPE '{validOptions.CType}'");
        }

        builder.Append(';');

        using var command = new NpgsqlCommand(builder.ToString(), connection);
        command.ExecuteNonQuery();
        return new DatabaseInformation(databaseConnectionString, database);
    }

    /// <summary>
    ///     Drops a Postgres database.
    /// </summary>
    public static void DropDatabase(this DatabaseInformation information)
    {
        using var connection = new NpgsqlConnection(information.ToMasterConnectionString());
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
    public static bool CheckDatabaseExists(this DatabaseInformation information, string database,
        bool ignoreCase = true)
    {
        var masterConnectionString = information.ToMasterConnectionString();
        using var connection = new NpgsqlConnection(masterConnectionString);
        connection.Open();
        // ReSharper disable once StringLiteralTypo
        using var command = new NpgsqlCommand("SELECT datname FROM pg_database", connection);
        using var reader = command.ExecuteReader();
        var databases = new List<string>();
        while (reader.Read())
        {
            databases.Add(reader.GetString(0));
        }

        return databases.Contains(database, ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
    }

    /// <summary>
    ///     Gets the master database connection string.
    /// </summary>
    private static string ToMasterConnectionString(this DatabaseInformation information)
    {
        return new NpgsqlConnectionStringBuilder(information.ConnectionString)
        {
            Database = "postgres"
        }.ToString();
    }
}