using System.Text.RegularExpressions;

namespace SleepingBearSystems.TemporaryDatabase.Common.Tests;

/// <summary>
/// Tests for <see cref="TemporaryDatabaseGuardOptionsExtensions"/>.
/// </summary>
internal static class TemporaryDatabaseGuardOptionsExtensionsTests
{
    [Test]
    public static void GenerateDatabaseName_ValidatesBehavior()
    {
        // use case: null options
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                default(CreateDatabaseOptions)!.GenerateDatabaseName());
            Assert.That(ex!.ParamName, Is.EqualTo("options"));
        }
        // use case: null prefix
        {
            var options = new CreateDatabaseOptions() { DatabasePrefix = default };
            var database = options.GenerateDatabaseName();
            Assert.Multiple(() =>
            {
                Assert.That(database, Has.Length.EqualTo(32));
                Assert.That(GuidRegex.IsMatch(database), Is.True);
            });
        }
        // use case: empty prefix
        {
            var options = new CreateDatabaseOptions() { DatabasePrefix = string.Empty };
            var database = options.GenerateDatabaseName();
            Assert.Multiple(() =>
            {
                Assert.That(database, Has.Length.EqualTo(32));
                Assert.That(GuidRegex.IsMatch(database), Is.True);
            });
        }
        // use case: whitespace prefix
        {
            var options = new CreateDatabaseOptions() { DatabasePrefix = "   " };
            var database = options.GenerateDatabaseName();
            Assert.Multiple(() =>
            {
                Assert.That(database, Has.Length.EqualTo(32));
                Assert.That(GuidRegex.IsMatch(database), Is.True);
            });
        }

        // use case: valid prefix
        {
            const string prefix = "prefix_";
            var options = new CreateDatabaseOptions() { DatabasePrefix = prefix };
            var database = options.GenerateDatabaseName();
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
