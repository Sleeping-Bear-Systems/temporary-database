namespace SleepingBearSystems.TemporaryDatabase.Postgres;

/// <summary>
///     Temporary database guard configuration options.
/// </summary>
public sealed class DatabaseOptions
{
    /// <summary>
    ///     Default options.
    /// </summary>
    public static readonly DatabaseOptions Defaults = new()
    {
        Collation = default
    };

    /// <summary>
    ///     Database collation.
    /// </summary>
    public string? Collation { get; init; }
}
