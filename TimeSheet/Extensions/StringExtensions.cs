using System.Diagnostics;
using System.Text.Json;

namespace TimeSheet.Extensions;

public static class StringExtensions {

    public static T? JsonDeserialize<T>(this string json) {
        try {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (JsonException e) {
            Debug.WriteLine($"Error deserializing JSON: {e}");
            return default;
        }
    }
}