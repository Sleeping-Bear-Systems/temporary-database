using Npgsql;

namespace SleepingBearSystems.TemporaryDatabase.Postgres.Tests;

/// <summary>
///     Tests for <see cref="PostgresHelper" />.
/// </summary>
internal static class PostgresHelperTests
{
    [Test]
    public static void GetMasterConnectionString_ValidatesBehavior()
    {
        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = "local.net",
            Username = "user",
            Password = "password",
            Database = "database"
        }.ToString();
        var masterConnectionString = PostgresHelper.GetMasterConnectionString(connectionString);
        Assert.That(masterConnectionString,
            Is.EqualTo("Host=local.net;Username=user;Password=password;Database=postgres"));
    }
}
