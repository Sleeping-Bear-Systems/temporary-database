using System.Text.RegularExpressions;

namespace SleepingBearSystems.TemporaryDatabase.Common.Tests;

/// <summary>
/// Tests for <see cref="DatabaseHelper"/>.
/// </summary>
internal static class DatabaseHelperTests
{
    [Test]
    public static void GenerateDatabaseName_ValidatesBehavior()
    {
        // use case: null prefix
        {
            var database = DatabaseHelper.GenerateDatabaseName(prefix: null);
            Assert.Multiple(() =>
            {
                Assert.That(database, Has.Length.EqualTo(32 + DatabaseHelper.DefaultPrefix.Length));
                Assert.That(database, Does.StartWith(DatabaseHelper.DefaultPrefix));
                Assert.That(GuidRegex.IsMatch(database), Is.True);
            });
        }
        // use case: empty prefix
        {
            var database = DatabaseHelper.GenerateDatabaseName(prefix: string.Empty);
            Assert.Multiple(() =>
            {
                Assert.That(database, Has.Length.EqualTo(32 + DatabaseHelper.DefaultPrefix.Length));
                Assert.That(database, Does.StartWith(DatabaseHelper.DefaultPrefix));
                Assert.That(GuidRegex.IsMatch(database), Is.True);
            });
        }
        // use case: whitespace prefix
        {
            var database = DatabaseHelper.GenerateDatabaseName(prefix: "    ");
            Assert.Multiple(() =>
            {
                Assert.That(database, Has.Length.EqualTo(32 + DatabaseHelper.DefaultPrefix.Length));
                Assert.That(database, Does.StartWith(DatabaseHelper.DefaultPrefix));
                Assert.That(GuidRegex.IsMatch(database), Is.True);
            });
        }

        // use case: valid prefix
        {
            const string prefix = "prefix_";
            var database = DatabaseHelper.GenerateDatabaseName(prefix);
            Assert.Multiple(() =>
            {
                Assert.That(database, Has.Length.EqualTo(32 + prefix.Length));
                Assert.That(database, Does.StartWith(prefix));
                Assert.That(GuidRegex.IsMatch(database), Is.True);
            });
        }
    }

    private static readonly Regex GuidRegex = new("[a-f0-9]{32}");
}
