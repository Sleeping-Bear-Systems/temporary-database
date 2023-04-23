namespace SleepingBearSystems.TemporaryDatabase.Common;

/// <summary>
/// Temporary database guard interface.
/// </summary>
public interface ITemporaryDatabaseGuard : IDisposable
{
    /// <summary>
    /// Connection string.
    /// </summary>
    DatabaseInformation Information { get; }
}
