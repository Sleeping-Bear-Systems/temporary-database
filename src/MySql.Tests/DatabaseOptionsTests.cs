using MySql.Data.MySqlClient;

namespace SleepingBear.TemporaryDatabase.MySql.Tests;

/// <summary>
///     Tests for <see cref="DatabaseOptions" />.
/// </summary>
internal static class DatabaseOptionsTests
{
    [Test]
    public static void Defaults_VerifiesValues()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DatabaseOptions.Defaults.CharacterSet, Is.Null);
            Assert.That(DatabaseOptions.Defaults.Collation, Is.Null);
            Assert.That(DatabaseOptions.Defaults.SslMode, Is.EqualTo(MySqlSslMode.Preferred));
        });
    }
}