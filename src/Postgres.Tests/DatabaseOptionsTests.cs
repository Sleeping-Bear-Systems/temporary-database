using Npgsql;

namespace SleepingBear.TemporaryDatabase.Postgres.Tests;

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
            Assert.That(DatabaseOptions.Defaults.Collation, Is.Null);
            Assert.That(DatabaseOptions.Defaults.CType, Is.Null);
            Assert.That(DatabaseOptions.Defaults.Encoding, Is.Null);
            Assert.That(DatabaseOptions.Defaults.SslMode, Is.EqualTo(SslMode.Prefer));
        });
    }
}