namespace SleepingBearSystems.TemporaryDatabase.Common.Tests;

/// <summary>
///     Tests for <see cref="DatabaseInformation" />.
/// </summary>
internal static class CreateDatabaseInformationTests
{
    [Test]
    public static void Ctor_ValidatesBehavior()
    {
        const string connectionString = "connectionString";
        const string database = "database";
        var result = new DatabaseInformation(connectionString, database);
        Assert.Multiple(() =>
        {
            Assert.That(result.ConnectionString, Is.EqualTo(connectionString));
            Assert.That(result.Database, Is.EqualTo(database));
        });
    }
}