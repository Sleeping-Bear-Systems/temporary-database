namespace SleepingBearSystems.TemporaryDatabase.Postgres;

/// <summary>
///     Temporary database guard configuration options.
/// </summary>
public sealed class CreateDatabaseOptions
{
    /// <summary>
    ///     Default options.
    /// </summary>
    public static readonly CreateDatabaseOptions Defaults = new()
    {
        Collation = default
    };

    /// <summary>
    ///     Database collation.
    /// </summary>
    public string? Collation { get; init; }
}
