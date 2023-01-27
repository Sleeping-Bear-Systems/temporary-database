namespace SleepingBearSystems.TemporaryDatabase.Common.Tests;

/// <summary>
/// Tests for <see cref="CreateDatabaseResult"/>.
/// </summary>
internal static class CreateDatabaseResultTests
{
    [Test]
    public static void Ctor_ValidatesBehavior()
    {
        const string masterConnectionString = "masterConnectionString";
        const string connectionString = "connectionString";
        const string database = "database";
        var result = new CreateDatabaseResult(masterConnectionString, connectionString, database);
        Assert.Multiple(() =>
        {
            Assert.That(result.MasterConnectionString, Is.EqualTo(masterConnectionString));
            Assert.That(result.ConnectionString, Is.EqualTo(connectionString));
            Assert.That(result.Database, Is.EqualTo(database));
        });
    }
}
