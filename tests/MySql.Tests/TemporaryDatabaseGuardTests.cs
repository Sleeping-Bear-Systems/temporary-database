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
        using var guard = TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable);
        CheckDatabaseExists(guard.Result.ConnectionString);
    }

    [Test]
    public static void FromParameters_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable(TestServerEnvironmentVariable);
        var builder = new MySqlConnectionStringBuilder(connectionString);

        using var guard =
            TemporaryDatabaseGuard.FromParameters(builder.Server!, (ushort)builder.Port, builder.UserID!,
                builder.Password!);
        CheckDatabaseExists(guard.Result.ConnectionString);
    }

    private static void CheckDatabaseExists(string connectionString)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();
    }

    private const string TestServerEnvironmentVariable = "SBS_TEST_SERVER_MYSQL";
}
