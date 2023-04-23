using Npgsql;
using SleepingBearSystems.TemporaryDatabase.Common;

namespace SleepingBearSystems.TemporaryDatabase.Postgres.Tests;

/// <summary>
/// Tests for <see cref="TemporaryDatabaseGuard"/>.
/// </summary>
internal static class TemporaryDatabaseGuardTests
{
    [Test]
    public static void FromEnvironmentVariable_ValidatesBehavior()
    {
        ITemporaryDatabaseGuard guard = TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable);
        using (guard)
        {
            Assert.That(PostgresHelper.CheckDatabaseExists(guard.Information, guard.Information.Database),
                Is.True);
        }

        Assert.That(PostgresHelper.CheckDatabaseExists(guard.Information, guard.Information.Database),
            Is.False);
    }

    [Test]
    public static void FromParameters_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable(TestServerEnvironmentVariable);
        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        ITemporaryDatabaseGuard guard =
            TemporaryDatabaseGuard.FromParameters(
                builder.Host!,
                (ushort)builder.Port,
                builder.Username!,
                builder.Password!);
        using (guard)
        {
            Assert.That(PostgresHelper.CheckDatabaseExists(guard.Information, guard.Information.Database),
                Is.True);
        }

        Assert.That(PostgresHelper.CheckDatabaseExists(guard.Information, guard.Information.Database),
            Is.False);
    }

    [Test]
    public static void FromConnectionString_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable(TestServerEnvironmentVariable);

        ITemporaryDatabaseGuard guard = TemporaryDatabaseGuard.FromConnectionString(connectionString!);
        using (guard)
        {
            Assert.That(PostgresHelper.CheckDatabaseExists(guard.Information, guard.Information.Database),
                Is.True);
        }

        Assert.That(PostgresHelper.CheckDatabaseExists(guard.Information, guard.Information.Database),
            Is.False);
    }

    private const string TestServerEnvironmentVariable = "SBS_TEST_SERVER_POSTGRES";
}
