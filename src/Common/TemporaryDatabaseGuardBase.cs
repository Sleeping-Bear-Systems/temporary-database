namespace SleepingBearSystems.TemporaryDatabase.Common;

/// <summary>
/// Base class for temporary database guard implementations.
/// </summary>
public abstract class TemporaryDatabaseGuardBase
{
    /// <summary>
    /// Constructor.
    /// </summary>
    protected TemporaryDatabaseGuardBase(string connectionString)
    {
        this.ConnectionString = connectionString;
    }

    /// <summary>
    /// Connection string.
    /// </summary>
    public string ConnectionString { get; }
}
