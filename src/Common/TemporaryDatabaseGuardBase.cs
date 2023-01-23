namespace SleepingBearSystems.TemporaryDatabase.Common;

/// <summary>
/// Abstract base class for temporary database guard implementations.
/// </summary>
public abstract class TemporaryDatabaseGuardBase
{
    /// <summary>
    /// Constructor
    /// </summary>
    protected TemporaryDatabaseGuardBase(CreateDatabaseResult result)
    {
        this.Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    /// <summary>
    /// Gets the create database result.
    /// </summary>
    public CreateDatabaseResult Result { get; }
}
