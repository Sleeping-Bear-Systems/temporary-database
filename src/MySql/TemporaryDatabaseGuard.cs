using System.Globalization;
using MySql.Data.MySqlClient;
using SleepingBearSystems.TemporaryDatabase.Common;

namespace SleepingBearSystems.TemporaryDatabase.MySql;

/// <summary>
/// Temporary database guard for Postgres databases.
/// </summary>
public sealed class TemporaryDatabaseGuard : TemporaryDatabaseGuardBase, ITemporaryDatabaseGuard
{
    private TemporaryDatabaseGuard(string database, string connectionString, string masterConnectionString)
        : base(database, connectionString, masterConnectionString)
    {
    }

    /// <inheritdoc cref="IDisposable"/>
    public void Dispose()
    {
        using var connection = new MySqlConnection(this.MasterConnectionString);
        connection.Open();
        var cmdText = string.Format(
            CultureInfo.InvariantCulture,
            "DROP DATABASE IF EXISTS {0};",
            this.Database);
        using var command = new MySqlCommand(cmdText, connection);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard Create(string connectionString, TemporaryDatabaseGuardOptions? options = default)
    {
        var validOptions = options ?? TemporaryDatabaseGuardOptions.Defaults;

        var builder = new MySqlConnectionStringBuilder(connectionString);
        var database = builder.Database;
        if (string.IsNullOrWhiteSpace(database))
        {
            database = validOptions.GenerateDatabaseName();
        }

        builder.Database = "mysql";
        var masterConnectionString = builder.ToString();

        using var connection = new MySqlConnection(masterConnectionString);
        connection.Open();

        var cmdText = string.Format(CultureInfo.InvariantCulture, "CREATE DATABASE {0};", database);
        using var command = new MySqlCommand(cmdText, connection);
        command.ExecuteNonQuery();

        return new TemporaryDatabaseGuard(database, connectionString, masterConnectionString);
    }
}
