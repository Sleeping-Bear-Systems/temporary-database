using System.Text;

namespace SleepingBear.TemporaryDatabase.Common;

/// <summary>
///     Extension methods for <see cref="StringBuilder" />.
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    ///     Conditionally appends a value to a <see cref="StringBuilder" />.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder" /> instance.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="value">The <see cref="string" /> value to append.</param>
    /// <returns>The <see cref="StringBuilder" /> instance.</returns>
    public static StringBuilder AppendIf(this StringBuilder builder, bool condition, string value)
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (condition)
        {
            builder.Append(value);
        }

        return builder;
    }
}