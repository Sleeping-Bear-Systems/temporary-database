namespace SleepingBearSystems.TemporaryDatabase.Common;

/// <summary>
///     Abstract base class for temporary database guard implementations.
/// </summary>
public abstract class TemporaryDatabaseGuardBase
{
    /// <summary>
    ///     Constructor
    /// </summary>
    protected TemporaryDatabaseGuardBase(DatabaseInformation information)
    {
        Information = information ?? throw new ArgumentNullException(nameof(information));
    }

    /// <summary>
    ///     Gets the create database result.
    /// </summary>
    public DatabaseInformation Information { get; }
}
