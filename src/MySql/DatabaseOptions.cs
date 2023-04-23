namespace SleepingBearSystems.TemporaryDatabase.MySql;

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
        Collation = default,
        CharacterSet = default,
        Encrypted = false
    };

    /// <summary>
    ///     Database collation.
    /// </summary>
    public string? Collation { get; init; }

    /// <summary>
    ///     Database character set.
    /// </summary>
    public string? CharacterSet { get; init; }

    /// <summary>
    ///     Database encrypted flag.
    /// </summary>
    public bool Encrypted { get; init; }
}
