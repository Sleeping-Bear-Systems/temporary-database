﻿using MySql.Data.MySqlClient;
using SleepingBearSystems.TemporaryDatabase.Common;

namespace SleepingBearSystems.TemporaryDatabase.MySql;

/// <summary>
/// Temporary database guard for Postgres databases.
/// </summary>
public sealed class TemporaryDatabaseGuard : TemporaryDatabaseGuardBase, ITemporaryDatabaseGuard
{
    private TemporaryDatabaseGuard(CreateDatabaseResult result)
        : base(result)
    {
    }

    /// <inheritdoc cref="IDisposable"/>
    public void Dispose()
    {
        MySqlHelper.DropDatabase(this.Result);
    }

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard FromEnvironmentVariable(
        string variable,
        string? prefix = default,
        CreateDatabaseOptions? options = default) =>
        TemporaryDatabaseGuard.FromConnectionString(
            Environment.GetEnvironmentVariable(variable) ?? string.Empty,
            prefix,
            options);

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard FromParameters(
        string server,
        string userId,
        string password,
        string? prefix = default,
        CreateDatabaseOptions? options = default) =>
        TemporaryDatabaseGuard.FromParameters(server, null, userId, password, prefix, options);

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard FromParameters(
        string server,
        ushort? port,
        string userId,
        string password,
        string? prefix = default,
        CreateDatabaseOptions? options = default)
    {
        var builder = new MySqlConnectionStringBuilder()
        {
            Server = server,
            UserID = userId,
            Password = password
        };
        if (port.HasValue)
        {
            builder.Port = port.Value;
        }

        return FromConnectionString(builder.ToString(), prefix, options);
    }

    /// <summary>
    /// Factory method for creating a <see cref="TemporaryDatabaseGuard"/> instance.
    /// </summary>
    public static TemporaryDatabaseGuard FromConnectionString(
        string connectionString,
        string? prefix = default,
        CreateDatabaseOptions? options = default)
    {
        var result = MySqlHelper.CreateDatabase(
            connectionString,
            DatabaseHelper.GenerateDatabaseName(prefix),
            options ?? CreateDatabaseOptions.Defaults);

        return new TemporaryDatabaseGuard(result);
    }
}
