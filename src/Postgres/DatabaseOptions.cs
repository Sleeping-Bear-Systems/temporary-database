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
        Collation = null,
        CType = null,
        Encoding = null
    };

    /// <summary>
    ///     Database character set encoding.
    /// </summary>
    public string? Encoding { get; init; }

    /// <summary>
    ///     Database collation order.
    /// </summary>
    public string? Collation { get; init; }

    /// <summary>
    ///     Database character classification.
    /// </summary>
    public string? CType { get; init; }
}