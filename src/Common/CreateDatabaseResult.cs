namespace SleepingBearSystems.TemporaryDatabase.Common;

/// <summary>
/// Container class hold database creation information.
/// </summary>
public sealed class CreateDatabaseResult
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public CreateDatabaseResult(string masterConnectionString, string connectionString, string database)
    {
        this.MasterConnectionString = masterConnectionString;
        this.ConnectionString = connectionString;
        this.Database = database;
    }

    /// <summary>
    /// The master connection string.
    /// </summary>
    public string MasterConnectionString { get; }

    /// <summary>
    /// The connection string.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// The database name.
    /// </summary>
    public string Database { get; }
}
