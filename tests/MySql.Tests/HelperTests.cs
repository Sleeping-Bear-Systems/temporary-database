
using MySql.Data.MySqlClient;

namespace SleepingBearSystems.TemporaryDatabase.MySql.Tests;

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
        var builder = new MySqlConnectionStringBuilder(connectionString);

        using var guard = Helper.FromParameters(builder.Server!, (ushort)builder.Port, builder.UserID!, builder.Password!);
        CheckDatabaseExists(guard.ConnectionString);
    }

    private static void CheckDatabaseExists(string connectionString)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();
    }

    private const string TestServerEnvironmentVariable = "SBS_TEST_SERVER_MYSQL";
}
