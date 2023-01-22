using MySql.Data.MySqlClient;

namespace SleepingBearSystems.TemporaryDatabase.MySql;

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

        var builder = new MySqlConnectionStringBuilder(connectionString)
        {
            Database = (database ?? string.Empty).Trim()
        };
        return TemporaryDatabaseGuard.Create(builder.ToString());
    }

    /// <summary>
    /// Creates a temporary database from the supplied parameters.
    /// </summary>
    /// <param name="server">The server.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="password">The password.</param>
    public static TemporaryDatabaseGuard FromParameters(
        string server,
        string userId,
        string password) =>
        FromParameters(server, port: null, userId, password);

    /// <summary>
    /// Creates a temporary database from the supplied parameters.
    /// </summary>
    /// <param name="server">The server.</param>
    /// <param name="port">The port.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="password">The password.</param>
    /// <param name="database">The database name.</param>
    public static TemporaryDatabaseGuard FromParameters(
        string server,
        ushort? port,
        string userId,
        string password,
        string? database = default)
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = server,
            UserID = userId,
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
