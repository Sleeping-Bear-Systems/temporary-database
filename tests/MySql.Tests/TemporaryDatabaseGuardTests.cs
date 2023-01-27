using System.Globalization;
using MySql.Data.MySqlClient;
using SleepingBearSystems.TemporaryDatabase.Common;

namespace SleepingBearSystems.TemporaryDatabase.MySql.Tests;

/// <summary>
/// Tests for <see cref="TemporaryDatabaseGuard"/>.
/// </summary>
internal static class TemporaryDatabaseGuardTests
{
    [Test]
    public static void FromEnvironmentVariable_ValidatesBehavior()
    {
        // use case: default options
        {
            var guard = TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable);
            using (guard)
            {
                Assert.That(
                    MySqlHelper.CheckDatabaseExists(guard.Result.MasterConnectionString, guard.Result.Database),
                    Is.True);
            }

            Assert.That(
                MySqlHelper.CheckDatabaseExists(guard.Result.MasterConnectionString, guard.Result.Database),
                Is.False);
        }

        // use case: custom options
        {
            var options = new CreateDatabaseOptions()
            {
                Collation = "latin1_swedish_ci"
            };
            var guard = TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable, options: options);
            using (guard)
            {
                Assert.That(
                    MySqlHelper.CheckDatabaseExists(guard.Result.MasterConnectionString, guard.Result.Database),
                    Is.True);
                var (characterSet, collation) =
                    QueryCharacterSetCollation(guard.Result.MasterConnectionString, guard.Result.Database);
                Assert.Multiple(() =>
                {
                    Assert.That(characterSet, Is.EqualTo("latin1"));
                    Assert.That(collation, Is.EqualTo(options.Collation));
                });
            }

            Assert.That(
                MySqlHelper.CheckDatabaseExists(guard.Result.MasterConnectionString, guard.Result.Database),
                Is.False);
        }
    }

    [Test]
    public static void FromParameters_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable(TestServerEnvironmentVariable);
        var builder = new MySqlConnectionStringBuilder(connectionString);

        // use case: default options
        {
            var guard = TemporaryDatabaseGuard.FromParameters(
                builder.Server!,
                (ushort)builder.Port,
                builder.UserID!,
                builder.Password!);
            using (guard)
            {
                Assert.That(MySqlHelper.CheckDatabaseExists(guard.Result.MasterConnectionString, guard.Result.Database),
                    Is.True);
            }

            Assert.That(MySqlHelper.CheckDatabaseExists(guard.Result.MasterConnectionString, guard.Result.Database),
                Is.False);
        }
    }

    [Test]
    public static void FromConnectionString_ValidatesBehavior()
    {
        var connectionString = Environment.GetEnvironmentVariable(TestServerEnvironmentVariable);

        var guard = TemporaryDatabaseGuard.FromConnectionString(connectionString!);
        using (guard)
        {
            Assert.That(MySqlHelper.CheckDatabaseExists(guard.Result.MasterConnectionString, guard.Result.Database),
                Is.True);
        }

        Assert.That(MySqlHelper.CheckDatabaseExists(guard.Result.MasterConnectionString, guard.Result.Database),
            Is.False);
    }

    private const string TestServerEnvironmentVariable = "SBS_TEST_SERVER_MYSQL";

    private static (string, string) QueryCharacterSetCollation(string masterConnectionString, string database)
    {
        var connection = new MySqlConnection(masterConnectionString);
        connection.Open();
        var cmdText =
            string.Format(
                CultureInfo.InvariantCulture,
                format:
                "SELECT DEFAULT_CHARACTER_SET_NAME, DEFAULT_COLLATION_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{0}';",
                database);
        var command = new MySqlCommand(cmdText, connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var characterSet = reader.GetString(0);
            var collation = reader.GetString(1);
            return (characterSet, collation);
        }

        return (string.Empty, string.Empty);
    }
}
