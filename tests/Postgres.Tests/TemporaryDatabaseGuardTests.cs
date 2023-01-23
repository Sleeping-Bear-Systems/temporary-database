using Npgsql;

namespace SleepingBearSystems.TemporaryDatabase.Postgres.Tests;

/// <summary>
/// Tests for <see cref="TemporaryDatabaseGuard"/>.
/// </summary>
internal static class TemporaryDatabaseGuardTests
{
    [Test]
    public static void FromEnvironmentVariable_ValidatesBehavior()
    {
        using var guard = TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable);
        CheckDatabaseExists(guard.Result.ConnectionString);
    }

    [Test]
    public static void FromParameters_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable(TestServerEnvironmentVariable);
        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        using var guard =
            TemporaryDatabaseGuard.FromParameters(
                builder.Host!,
                (ushort)builder.Port,
                builder.Username!,
                builder.Password!);
        CheckDatabaseExists(guard.Result.ConnectionString);
    }

    [Test]
    public static void FromConnectionString_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable(TestServerEnvironmentVariable);

        using var guard = TemporaryDatabaseGuard.FromConnectionString(connectionString!);
        CheckDatabaseExists(guard.Result.ConnectionString);
    }

    private static void CheckDatabaseExists(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
    }

    private const string TestServerEnvironmentVariable = "SBS_TEST_SERVER_POSTGRES";
}
