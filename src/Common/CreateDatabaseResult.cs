namespace SleepingBearSystems.TemporaryDatabase.Common;

/// <summary>
/// Container class hold database creation information.
/// </summary>
public sealed class CreateDatabaseResult
{
    /// <summary>
    /// The master connection string.
    /// </summary>
    public string MasterConnectionString { get; init; }

    /// <summary>
    /// The connection string.
    /// </summary>
    public string ConnectionString { get; init; }

    /// <summary>
    /// The database name.
    /// </summary>
    public string Database { get; init; }
}
