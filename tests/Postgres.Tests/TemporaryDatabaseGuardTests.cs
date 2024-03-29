﻿using Npgsql;
using SleepingBearSystems.TemporaryDatabase.Common;

namespace SleepingBearSystems.TemporaryDatabase.Postgres.Tests;

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
            ITemporaryDatabaseGuard guard =
                TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable);
            using (guard)
            {
                Assert.That(
                    guard.Information.CheckDatabaseExists(guard.Information.Database),
                    Is.True);
            }

            Assert.That(
                guard.Information.CheckDatabaseExists(guard.Information.Database),
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
            ITemporaryDatabaseGuard guard =
                TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable, options: options);
            using (guard)
            {
                Assert.That(
                    guard.Information.CheckDatabaseExists(guard.Information.Database),
                    Is.True);
            }

            Assert.That(
                guard.Information.CheckDatabaseExists(guard.Information.Database),
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
            ITemporaryDatabaseGuard guard =
                TemporaryDatabaseGuard.FromParameters(
                    builder.Host!,
                    (ushort)builder.Port,
                    builder.Username!,
                    builder.Password!);
            using (guard)
            {
                Assert.That(guard.Information.CheckDatabaseExists(guard.Information.Database),
                    Is.True);
            }

            Assert.That(guard.Information.CheckDatabaseExists(guard.Information.Database),
                Is.False);
        }
    }

    [Test]
    public static void FromConnectionString_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable(TestServerEnvironmentVariable);

        ITemporaryDatabaseGuard guard = TemporaryDatabaseGuard.FromConnectionString(connectionString!);
        using (guard)
        {
            Assert.That(guard.Information.CheckDatabaseExists(guard.Information.Database),
                Is.True);
        }

        Assert.That(guard.Information.CheckDatabaseExists(guard.Information.Database),
            Is.False);
    }
}
