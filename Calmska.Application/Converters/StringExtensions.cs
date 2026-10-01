using System.Globalization;

namespace Calmska.Application.Converters;

public static class StringExtensions
{
    public static float? ToFloatOrNull(this string? value) =>
        float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : null;
}