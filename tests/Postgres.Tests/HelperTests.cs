using Npgsql;

namespace SleepingBearSystems.TemporaryDatabase.Postgres.Tests;

/// <summary>
/// Tests for <see cref="Helper"/>.
/// </summary>
internal static class HelperTests
{
    [Test]
    public static void FromEnvironmentVariable_ValidatesBehavior()
    {
        using var guard = Helper.FromEnvironmentVariable(TestServerEnvironmentVariable);
        CheckDatabaseExists(guard.ConnectionString);
    }

    [Test]
    public static void FromParameters_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable(TestServerEnvironmentVariable);
        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        using var guard = Helper.FromParameters(builder.Host!, (ushort)builder.Port, builder.Username!, builder.Password!);
        CheckDatabaseExists(guard.ConnectionString);
    }

    private static void CheckDatabaseExists(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
    }

    private const string TestServerEnvironmentVariable = "SBS_TEST_SERVER_POSTGRES";
}
