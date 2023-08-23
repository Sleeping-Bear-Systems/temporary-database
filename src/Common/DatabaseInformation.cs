namespace SleepingBearSystems.TemporaryDatabase.Common;

/// <summary>
///     Container class hold database creation information.
/// </summary>
public sealed class DatabaseInformation
{
    /// <summary>
    ///     Constructor.
    /// </summary>
    public DatabaseInformation(string connectionString, string database)
    {
        ConnectionString = connectionString;
        Database = database;
    }

    /// <summary>
    ///     The connection string.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    ///     The database name.
    /// </summary>
    public string Database { get; }
}
