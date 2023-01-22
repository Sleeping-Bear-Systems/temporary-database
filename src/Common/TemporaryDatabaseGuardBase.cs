namespace SleepingBearSystems.TemporaryDatabase.Common;

/// <summary>
/// Abstract base class for temporary database guard implementations.
/// </summary>
public abstract class TemporaryDatabaseGuardBase
{
    /// <summary>
    /// Constructor
    /// </summary>
    protected TemporaryDatabaseGuardBase(string database, string connectionString, string masterConnectionString)
    {
        this.Database = database;
        this.ConnectionString = connectionString;
        this.MasterConnectionString = masterConnectionString;
    }

    /// <summary>
    /// The database name.
    /// </summary>
    protected string Database { get; }

    /// <summary>
    /// The connection string.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// The master connection string.
    /// </summary>
    protected string MasterConnectionString { get; }
}
