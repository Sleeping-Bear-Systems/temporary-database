namespace SleepingBearSystems.TemporaryDatabase.MySql.Tests;

/// <summary>
/// Tests for <see cref="TemporaryDatabaseGuard"/>.
/// </summary>
internal static class TemporaryDatabaseGuardTests
{
    [Test]
    public static void Create_ValidatesBehavior()
    {
        const string connectionString = "";
        using var temporaryDatabase = TemporaryDatabaseGuard.Create(connectionString);
        Assert.That(temporaryDatabase.ConnectionString, Is.EqualTo(connectionString));
    }
}
