namespace SleepingBearSystems.TemporaryDatabase.Core.Tests;

/// <summary>
/// Tests for <see cref="TemporaryDatabaseGuardBase"/>.
/// </summary>
internal static class TemporaryDatabaseGuardBaseTests
{
    [Test]
    public static void Ctor_ValidatesBehavior()
    {
        const string connectionString = "the quick brown fox jumped over the lazy dogs";
        var guard = new MockTemporaryDatabaseGuard(connectionString);
        Assert.That(guard.ConnectionString, Is.EqualTo(connectionString));
    }

    private sealed class MockTemporaryDatabaseGuard : TemporaryDatabaseGuardBase
    {
        public MockTemporaryDatabaseGuard(string connectionString) : base(connectionString)
        {
        }
    }
}
