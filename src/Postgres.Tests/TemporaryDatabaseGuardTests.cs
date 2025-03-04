using SleepingBear.Functional.Errors;
using SleepingBear.Functional.Testing;

namespace SleepingBear.TemporaryDatabase.Postgres.Tests;

/// <summary>
///     Tests for <see cref="TemporaryDatabaseGuard" />.
/// </summary>
internal static class TemporaryDatabaseGuardTests
{
    [Test]
    public static async Task FromConnectionStringAsync_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable("SBS_TEST_SERVER_POSTGRES");
        var guard = await TemporaryDatabaseGuard
            .FromConnectionStringAsync(connectionString)
            .ConfigureAwait(false);
        await using (guard.ConfigureAwait(false))
        {
            Assert.That(guard.ConnectionString, Is.Not.Null);
        }
    }

    [Test]
    public static async Task FromEnvironmentVariableAsync_ValidatesBehavior()
    {
        var guard = await TemporaryDatabaseGuard
            .FromEnvironmentVariableAsync()
            .ConfigureAwait(false);
        await using (guard.ConfigureAwait(false))
        {
            Assert.That(guard.ConnectionString, Is.Not.Null);
        }
    }

    [Test]
    public static void ConvertFromUri_EmptyString_ReturnError()
    {
        var result = TemporaryDatabaseGuard.ConvertFromUri("");
        TestResult.IsError<string, InvalidFormatError>(result);
    }
}