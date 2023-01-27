using System.Text;

namespace SleepingBearSystems.TemporaryDatabase.Common;

/// <summary>
/// Helper methods for databases.
/// </summary>
public static class DatabaseHelper
{
    /// <summary>
    /// Generates a random database name.
    /// </summary>
    public static string GenerateDatabaseName(string? prefix = default) =>
        new StringBuilder()
            .Append(string.IsNullOrWhiteSpace(prefix) ? DefaultPrefix : prefix)
            .Append(Guid.NewGuid().ToString("N"))
            .ToString()
            .ToLowerInvariant();

    /// <summary>
    /// Default database name prefix
    /// </summary>
    public const string DefaultPrefix = "sbs_";
}
