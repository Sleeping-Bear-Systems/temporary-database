using MySql.Data.MySqlClient;

namespace SleepingBearSystems.TemporaryDatabase.MySql.Tests;

/// <summary>
/// Tests for <see cref="TemporaryDatabaseGuard"/>.
/// </summary>
internal static class TemporaryDatabaseGuardTests
{
    [Test]
    public static void FromEnvironmentVariable_ValidatesBehavior()
    {
        var guard = TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable);
        using (guard)
        {
            Assert.That(MySqlHelper.CheckDatabaseExists(guard.Result.MasterConnectionString, guard.Result.Database),
                Is.True);
        }

        Assert.That(MySqlHelper.CheckDatabaseExists(guard.Result.MasterConnectionString, guard.Result.Database),
            Is.False);
    }

    [Test]
    public static void FromParameters_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable(TestServerEnvironmentVariable);
        var builder = new MySqlConnectionStringBuilder(connectionString);

        var guard = TemporaryDatabaseGuard.FromParameters(
            builder.Server!,
            (ushort)builder.Port,
            builder.UserID!,
            builder.Password!);
        using (guard)
        {
            Assert.That(MySqlHelper.CheckDatabaseExists(guard.Result.MasterConnectionString, guard.Result.Database),
                Is.True);
        }

        Assert.That(MySqlHelper.CheckDatabaseExists(guard.Result.MasterConnectionString, guard.Result.Database),
            Is.False);
    }

    [Test]
    public static void FromConnectionString_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable(TestServerEnvironmentVariable);

        var guard = TemporaryDatabaseGuard.FromConnectionString(connectionString!);
        using (guard)
        {
            Assert.That(MySqlHelper.CheckDatabaseExists(guard.Result.MasterConnectionString, guard.Result.Database),
                Is.True);
        }

        Assert.That(MySqlHelper.CheckDatabaseExists(guard.Result.MasterConnectionString, guard.Result.Database),
            Is.False);
    }

    private const string TestServerEnvironmentVariable = "SBS_TEST_SERVER_MYSQL";
}
