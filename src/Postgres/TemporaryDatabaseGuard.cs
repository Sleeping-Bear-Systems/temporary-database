using System.Globalization;
using System.Reflection;
using Npgsql;
using SleepingBearSystems.TemporaryDatabase.Common;

namespace SleepingBearSystems.TemporaryDatabase.Postgres;

/// <summary>
/// Temporary database guard for Postgres databases.
/// </summary>
public sealed class TemporaryDatabaseGuard : TemporaryDatabaseGuardBase, IDisposable
{
    private TemporaryDatabaseGuard(string database, string connectionString, string masterConnectionString)
        : base(database, connectionString, masterConnectionString)
    {
    }

    /// <inheritdoc cref="IDisposable"/>
    public void Dispose()
    {
        PostgresHelper.DropDatabase(this.MasterConnectionString, this.Database);
    }

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard FromEnvironmentVariable(
        string variable,
        CreateDatabaseOptions? options = default) =>
        TemporaryDatabaseGuard.FromConnectionString(
            Environment.GetEnvironmentVariable(variable) ?? string.Empty,
            options);

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard FromParameters(
        string host,
        string username,
        string password,
        CreateDatabaseOptions? options) =>
        TemporaryDatabaseGuard.FromParameters(host, port: null, username, password, options);

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard FromParameters(
        string host,
        ushort? port,
        string username,
        string password,
        CreateDatabaseOptions? options = default)
    {
        var builder = new NpgsqlConnectionStringBuilder()
        {
            Host = host,
            Username = username,
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
    public static TemporaryDatabaseGuard FromConnectionString(string connectionString,
        CreateDatabaseOptions? options = default)
    {
        var validOptions = options ?? CreateDatabaseOptions.Defaults;

        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var database = builder.Database;
        if (string.IsNullOrWhiteSpace(database))
        {
            database = validOptions.GenerateDatabaseName();
        }

        builder.Database = "postgres";
        var masterConnectionString = builder.ToString();

        PostgresHelper.CreateDatabase(masterConnectionString, database);

        return new TemporaryDatabaseGuard(database, connectionString, masterConnectionString);
    }
}
