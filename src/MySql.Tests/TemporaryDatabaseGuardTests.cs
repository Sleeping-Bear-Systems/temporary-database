using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MySql.Data.MySqlClient;
using SleepingBear.TemporaryDatabase.Common;

namespace SleepingBear.TemporaryDatabase.MySql.Tests;

/// <summary>
///     Tests for <see cref="TemporaryDatabaseGuard" />.
/// </summary>
internal static class TemporaryDatabaseGuardTests
{
    private const string TestServerEnvironmentVariable = "SBS_TEST_SERVER_MYSQL";

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
                CharacterSet = "latin1",
                Collation = "latin1_swedish_ci",
                SslMode = MySqlSslMode.Disabled
            };
            using (TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable, options: options))
            {
                Assert.That(
                    TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable, options: options)
                        .Information.CheckDatabaseExists(TemporaryDatabaseGuard
                            .FromEnvironmentVariable(TestServerEnvironmentVariable, options: options).Information
                            .Database),
                    Is.True);
                var (characterSet, collation) =
                    QueryCharacterSetCollation(
                        TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable, options: options)
                            .Information,
                        TemporaryDatabaseGuard.FromEnvironmentVariable(TestServerEnvironmentVariable, options: options)
                            .Information.Database);
                Assert.Multiple(() =>
                {
                    Assert.That(characterSet, Is.EqualTo("latin1"));
                    Assert.That(collation, Is.EqualTo(options.Collation));
                });
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
        var builder = new MySqlConnectionStringBuilder(connectionString);

        // use case: default options
        {
            using (TemporaryDatabaseGuard.FromParameters(
                       builder.Server!,
                       (ushort)builder.Port,
                       builder.UserID!,
                       builder.Password!))
            {
                Assert.That(TemporaryDatabaseGuard.FromParameters(
                        builder.Server!,
                        (ushort)builder.Port,
                        builder.UserID!,
                        builder.Password!).Information.CheckDatabaseExists(TemporaryDatabaseGuard.FromParameters(
                        builder.Server!,
                        (ushort)builder.Port,
                        builder.UserID!,
                        builder.Password!).Information.Database),
                    Is.True);
            }

            Assert.That(TemporaryDatabaseGuard.FromParameters(
                    builder.Server!,
                    (ushort)builder.Port,
                    builder.UserID!,
                    builder.Password!).Information.CheckDatabaseExists(TemporaryDatabaseGuard.FromParameters(
                    builder.Server!,
                    (ushort)builder.Port,
                    builder.UserID!,
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

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
    [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities")]
    private static (string, string) QueryCharacterSetCollation(DatabaseInformation information, string database)
    {
        var masterConnectionString = new MySqlConnectionStringBuilder(information.ConnectionString)
        {
            Database = "mysql"
        }.ToString();
        var connection = new MySqlConnection(masterConnectionString);
        connection.Open();
        var cmdText =
            string.Format(
                CultureInfo.InvariantCulture,
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