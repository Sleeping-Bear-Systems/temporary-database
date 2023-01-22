namespace SleepingBearSystems.TemporaryDatabase.Common.Tests;

/// <summary>
/// Tests for <see cref="TemporaryDatabaseGuardOptions"/>.
/// </summary>
internal static class TemporaryDatabaseGuardOptionsTests
{
    [Test]
    public static void Defaults_ValidatesBehavior()
    {
        Assert.That(TemporaryDatabaseGuardOptions.Defaults, Is.Not.Null);
        Assert.That(TemporaryDatabaseGuardOptions.Defaults.DatabasePrefix, Is.EqualTo("sbs_"));
    }
}
