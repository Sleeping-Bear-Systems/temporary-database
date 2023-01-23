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
        MySqlHelper.DropDatabase(this.MasterConnectionString, this.Database);
    }

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard FromEnvironmentVariable(
        string variable,
        TemporaryDatabaseGuardOptions? options = default) =>
        TemporaryDatabaseGuard.FromConnectionString(
            Environment.GetEnvironmentVariable(variable) ?? string.Empty,
            options);

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard FromParameters(
        string server,
        string userId,
        string password,
        TemporaryDatabaseGuardOptions? options = default) =>
        TemporaryDatabaseGuard.FromParameters(server, null, userId, password, options);

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard FromParameters(
        string server,
        ushort? port,
        string userId,
        string password,
        TemporaryDatabaseGuardOptions? options = default)
    {
        var builder = new MySqlConnectionStringBuilder()
        {
            Server = server,
            UserID = userId,
            Password = password
        };
        if (port.HasValue)
        {
            builder.Port = port.Value;
        }

        return FromConnectionString(builder.ToString(), options);
    }

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard FromConnectionString(
        string connectionString,
        TemporaryDatabaseGuardOptions? options = default)
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

        MySqlHelper.CreateDatabase(masterConnectionString, database);

        return new TemporaryDatabaseGuard(database, connectionString, masterConnectionString);
    }
}
