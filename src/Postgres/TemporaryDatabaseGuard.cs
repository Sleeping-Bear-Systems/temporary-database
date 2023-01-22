using SleepingBearSystems.TemporaryDatabase.Core;

namespace SleepingBearSystems.TemporaryDatabase.Postgres;

/// <summary>
/// Concrete implementation of the <see cref="ITemporaryDatabaseGuard"/> interface.
/// </summary>
public sealed class TemporaryDatabaseGuard : TemporaryDatabaseGuardBase, ITemporaryDatabaseGuard
{
    private TemporaryDatabaseGuard(string connectionString) : base(connectionString)
    {
    }

    /// <inheritdoc cref="IDisposable"/>
    public void Dispose()
    {
    }

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static TemporaryDatabaseGuard Create(string connectionString)
    {
        return new TemporaryDatabaseGuard(connectionString);
    }
}
