using MySql.Data.MySqlClient;

namespace SleepingBear.TemporaryDatabase.MySql.Tests;

/// <summary>
///     Tests for <see cref="TemporaryDatabaseGuard" />.
/// </summary>
internal static class TemporaryDatabaseGuardTests
{
    [Test]
    public static async Task FromConnectionStringAsync_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable("SBS_TEST_SERVER_MYSQL");
        var guard = await TemporaryDatabaseGuard
            .FromConnectionStringAsync(
                connectionString,
                new DatabaseOptions
                {
                    SslMode = MySqlSslMode.Disabled
                })
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
            .FromEnvironmentVariableAsync(
                options: new DatabaseOptions
                {
                    SslMode = MySqlSslMode.Disabled
                })
            .ConfigureAwait(false);
        await using (guard.ConfigureAwait(false))
        {
            Assert.That(guard.ConnectionString, Is.Not.Null);
        }
    }
}