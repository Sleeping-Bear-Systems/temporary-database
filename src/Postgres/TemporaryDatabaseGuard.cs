using Npgsql;
using SleepingBearSystems.TemporaryDatabase.Common;

namespace SleepingBearSystems.TemporaryDatabase.Postgres;

/// <summary>
/// Temporary database guard for Postgres databases.
/// </summary>
public sealed class TemporaryDatabaseGuard : TemporaryDatabaseGuardBase, ITemporaryDatabaseGuard
{
    private TemporaryDatabaseGuard(DatabaseInformation information)
        : base(information)
    {
    }

    /// <inheritdoc cref="IDisposable"/>
    public void Dispose()
    {
        PostgresHelper.DropDatabase(this.Information);
    }

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard FromEnvironmentVariable(
        string variable,
        string? prefix = default,
        CreateDatabaseOptions? options = default) =>
        FromConnectionString(
            Environment.GetEnvironmentVariable(variable) ?? string.Empty,
            prefix,
            options);

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public static TemporaryDatabaseGuard FromParameters(
        string host,
        string username,
        string password,
        string? prefix = default,
        CreateDatabaseOptions? options = default) =>
        FromParameters(host, port: null, username, password, prefix, options);

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard FromParameters(
        string host,
        ushort? port,
        string username,
        string password,
        string? prefix = default,
        CreateDatabaseOptions? options = default)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Username = username,
            Password = password
        };
        if (port.HasValue)
        {
            builder.Port = port.Value;
        }

        return FromConnectionString(builder.ToString(), prefix, options);
    }

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard FromConnectionString(
        string connectionString,
        string? prefix = default,
        CreateDatabaseOptions? options = default)
    {
        var result = PostgresHelper.CreateDatabase(
            connectionString,
            DatabaseHelper.GenerateDatabaseName(prefix),
            options);
        return new TemporaryDatabaseGuard(result);
    }
}
