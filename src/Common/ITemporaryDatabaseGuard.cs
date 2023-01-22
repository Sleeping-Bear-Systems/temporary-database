namespace SleepingBearSystems.TemporaryDatabase.Common;

/// <summary>
/// Temporary database guard interface.
/// </summary>
public interface ITemporaryDatabaseGuard : IDisposable
{
    /// <summary>
    /// Gets the connection string.
    /// </summary>
    string ConnectionString { get; }
}
