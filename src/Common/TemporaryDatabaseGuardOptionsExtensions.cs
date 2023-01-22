using System.Text;

namespace SleepingBearSystems.TemporaryDatabase.Common;

/// <summary>
/// Helper methods.
/// </summary>
public static class TemporaryDatabaseGuardOptionsExtensions
{
    /// <summary>
    /// Generates a random database name.
    /// </summary>
    public static string GenerateDatabaseName(this TemporaryDatabaseGuardOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var builder = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(options.DatabasePrefix))
        {
            builder.Append(options.DatabasePrefix);
        }

        builder.Append(Guid.NewGuid().ToString());
        return builder
            .ToString()
            .Replace("-", "")
            .ToLowerInvariant();
    }
}
