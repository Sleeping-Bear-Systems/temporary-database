using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SleepingBear.TemporaryDatabase.Common;

/// <summary>
///     Helper methods for databases.
/// </summary>
public static class DatabaseHelper
{
    /// <summary>
    ///     Default database name prefix
    /// </summary>
    public const string DefaultPrefix = "sbs_";

    /// <summary>
    ///     Generates a random database name.
    /// </summary>
    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
    public static string GenerateDatabaseName(string? prefix = null)
    {
        return new StringBuilder()
            .Append(string.IsNullOrWhiteSpace(prefix) ? DefaultPrefix : prefix)
            .Append(Guid.NewGuid().ToString("N"))
            .ToString()
            .ToLowerInvariant();
    }
}