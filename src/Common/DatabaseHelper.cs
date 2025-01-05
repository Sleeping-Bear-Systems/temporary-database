namespace SleepingBear.TemporaryDatabase.Common;

/// <summary>
///     Helper methods for databases.
/// </summary>
public static class DatabaseHelper
{
    /// <summary>
    ///     Generates a random database name.
    /// </summary>
    public static string GenerateDatabaseName()
    {
        return $"sbs_tmp_{Guid.NewGuid():N}";
    }
}