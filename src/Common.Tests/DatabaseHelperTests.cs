using System.Text.RegularExpressions;

namespace SleepingBear.TemporaryDatabase.Common.Tests;

/// <summary>
///     Tests for <see cref="DatabaseHelper" />.
/// </summary>
internal static partial class DatabaseHelperTests
{
    private static readonly Regex GuidRegex = MyRegex();

    [Test]
    public static void GenerateDatabaseName_ValidatesBehavior()
    {
        // use case: null prefix
        {
            var database = DatabaseHelper.GenerateDatabaseName();
            Assert.Multiple(() =>
            {
                Assert.That(database, Has.Length.EqualTo(32 + DatabaseHelper.DefaultPrefix.Length));
                Assert.That(database, Does.StartWith(DatabaseHelper.DefaultPrefix));
                Assert.That(GuidRegex.IsMatch(database), Is.True);
            });
        }
        // use case: empty prefix
        {
            var database = DatabaseHelper.GenerateDatabaseName(string.Empty);
            Assert.Multiple(() =>
            {
                Assert.That(database, Has.Length.EqualTo(32 + DatabaseHelper.DefaultPrefix.Length));
                Assert.That(database, Does.StartWith(DatabaseHelper.DefaultPrefix));
                Assert.That(GuidRegex.IsMatch(database), Is.True);
            });
        }
        // use case: whitespace prefix
        {
            var database = DatabaseHelper.GenerateDatabaseName("    ");
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

    [GeneratedRegex("[a-f0-9]{32}")]
    private static partial Regex MyRegex();
}