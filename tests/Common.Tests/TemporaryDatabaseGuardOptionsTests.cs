namespace SleepingBearSystems.TemporaryDatabase.Common.Tests;

/// <summary>
/// Tests for <see cref="CreateDatabaseOptions"/>.
/// </summary>
internal static class TemporaryDatabaseGuardOptionsTests
{
    [Test]
    public static void Defaults_ValidatesBehavior()
    {
        Assert.That(CreateDatabaseOptions.Defaults, Is.Not.Null);
        Assert.That(CreateDatabaseOptions.Defaults.DatabasePrefix, Is.EqualTo("sbs_"));
    }
}
