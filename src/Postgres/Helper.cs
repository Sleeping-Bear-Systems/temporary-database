using Npgsql;

namespace SleepingBearSystems.TemporaryDatabase.Postgres;

/// <summary>
/// Helper methods for the <see cref="TemporaryDatabaseGuard"/>.
/// </summary>
public static class Helper
{
    /// <summary>
    /// Creates a temporary database from the specified environment variable.
    /// </summary>
    /// <param name="variable">The environment variable.</param>
    /// <param name="database">The database name.</param>
    public static TemporaryDatabaseGuard FromEnvironmentVariable(string variable, string? database = default)
    {
        var connectionString = Environment.GetEnvironmentVariable(variable);

        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = (database ?? string.Empty).Trim()
        };
        return TemporaryDatabaseGuard.Create(builder.ToString());
    }

    /// <summary>
    /// Creates a temporary database from the supplied parameters.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="username">The user name.</param>
    /// <param name="password">The password.</param>
    /// <param name="database">The database name.</param>
    public static TemporaryDatabaseGuard FromParameters(
        string host,
        string username,
        string password,
        string? database = default) =>
        FromParameters(host, port: null, username, password, database);

    /// <summary>
    /// Creates a temporary database from the supplied parameters.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="port">The port.</param>
    /// <param name="username">The user name.</param>
    /// <param name="password">The password.</param>
    /// <param name="database">The database name.</param>
    public static TemporaryDatabaseGuard FromParameters(
        string host,
        ushort? port,
        string username,
        string password,
        string? database = default)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Username = username,
            Password = password,
            Database = (database ?? string.Empty).Trim()
        };
        if (port.HasValue)
        {
            builder.Port = port.Value;
        }

        return TemporaryDatabaseGuard.Create(builder.ToString());
    }
}
