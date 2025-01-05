using Npgsql;

namespace SleepingBear.TemporaryDatabase.Postgres.Tests;

/// <summary>
///     Tests for <see cref="TemporaryDatabaseGuard" />.
/// </summary>
internal static class TemporaryDatabaseGuardTests
{
    private const string TestServerEnvironmentVariable = "SBS_TEST_SERVER_POSTGRES";

    [Test]
    public static void FromEnvironmentVariable_ValidatesBehavior()
    {
        // use case: default options
        {
            using (TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable))
            {
                Assert.That(
                    TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable).Information
                        .CheckDatabaseExists(TemporaryDatabaseGuard
                            .FromEnvironmentVariable(TestServerEnvironmentVariable).Information.Database),
                    Is.True);
            }

            Assert.That(
                TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable).Information
                    .CheckDatabaseExists(TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable)
                        .Information.Database),
                Is.False);
        }

        // use case: custom options
        {
            var options = new DatabaseOptions
            {
                CType = "en_US.utf8",
                Collation = "en_US.utf8",
                Encoding = "utf8"
            };
            using (TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable, options: options))
            {
                Assert.That(
                    TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable, options: options)
                        .Information.CheckDatabaseExists(TemporaryDatabaseGuard
                            .FromEnvironmentVariable(TestServerEnvironmentVariable, options: options).Information
                            .Database),
                    Is.True);
            }

            Assert.That(
                TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable, options: options)
                    .Information.CheckDatabaseExists(TemporaryDatabaseGuard
                        .FromEnvironmentVariable(TestServerEnvironmentVariable, options: options).Information.Database),
                Is.False);
        }
    }

    [Test]
    public static void FromParameters_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable(TestServerEnvironmentVariable);
        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        // use case: default options
        {
            using (TemporaryDatabaseGuard.FromParameters(
                       builder.Host!,
                       (ushort)builder.Port,
                       builder.Username!,
                       builder.Password!))
            {
                Assert.That(TemporaryDatabaseGuard.FromParameters(
                        builder.Host!,
                        (ushort)builder.Port,
                        builder.Username!,
                        builder.Password!).Information.CheckDatabaseExists(TemporaryDatabaseGuard.FromParameters(
                        builder.Host!,
                        (ushort)builder.Port,
                        builder.Username!,
                        builder.Password!).Information.Database),
                    Is.True);
            }

            Assert.That(TemporaryDatabaseGuard.FromParameters(
                    builder.Host!,
                    (ushort)builder.Port,
                    builder.Username!,
                    builder.Password!).Information.CheckDatabaseExists(TemporaryDatabaseGuard.FromParameters(
                    builder.Host!,
                    (ushort)builder.Port,
                    builder.Username!,
                    builder.Password!).Information.Database),
                Is.False);
        }
    }

    [Test]
    public static void FromConnectionString_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable(TestServerEnvironmentVariable);

        using (TemporaryDatabaseGuard.FromConnectionString(connectionString!))
        {
            Assert.That(
                TemporaryDatabaseGuard.FromConnectionString(connectionString!).Information
                    .CheckDatabaseExists(TemporaryDatabaseGuard.FromConnectionString(connectionString!).Information
                        .Database),
                Is.True);
        }

        Assert.That(
            TemporaryDatabaseGuard.FromConnectionString(connectionString!).Information
                .CheckDatabaseExists(
                    TemporaryDatabaseGuard.FromConnectionString(connectionString!).Information.Database),
            Is.False);
    }
}