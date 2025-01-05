using System.Diagnostics.CodeAnalysis;

namespace SleepingBear.TemporaryDatabase.MySql.Tests;

/// <summary>
///     Tests for <see cref="TemporaryDatabaseGuard" />.
/// </summary>
[SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task")]
internal static class TemporaryDatabaseGuardTests
{
    [Test]
    public static async Task FromConnectionStringAsync_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable("SBS_TEST_SERVER_MYSQL");
        await using var guard = await TemporaryDatabaseGuard.FromConnectionStringAsync(connectionString);
        Assert.That(guard.ConnectionString, Is.Not.Null);
    }

    [Test]
    public static async Task FromEnvironmentVariableAsync_ValidatesBehavior()
    {
        await using var guard = await TemporaryDatabaseGuard.FromEnvironmentVariableAsync();
        Assert.That(guard.ConnectionString, Is.Not.Null);
    }
}