using Npgsql;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace SleepingBear.TemporaryDatabase.Postgres;

/// <summary>
///     Temporary database guard configuration options.
/// </summary>
public sealed class DatabaseOptions
{
    /// <summary>
    ///     Default options.
    /// </summary>
    public static readonly DatabaseOptions Defaults = new();

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

    /// <summary>
    ///     SSL Mode.
    /// </summary>
    public SslMode SslMode { get; init; } = SslMode.Prefer;
}