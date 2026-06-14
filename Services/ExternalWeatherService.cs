using System.Globalization;
using System.Text.Json;
using SmartGas.Api.DTOs;

namespace SmartGas.Api.Services;

public class ExternalWeatherService
{
    private readonly HttpClient _httpClient;

    public ExternalWeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExternalWeatherResponse?> GetCurrentWeatherAsync(decimal latitude, decimal longitude)
    {
        var latitudeText = latitude.ToString(CultureInfo.InvariantCulture);
        var longitudeText = longitude.ToString(CultureInfo.InvariantCulture);

        var url =
            "https://api.open-meteo.com/v1/forecast" +
            $"?latitude={latitudeText}" +
            $"&longitude={longitudeText}" +
            "&current=temperature_2m,relative_humidity_2m,wind_speed_10m" +
            "&timezone=auto";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.UserAgent.ParseAdd("SmartGas.Api/1.0");

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        if (!root.TryGetProperty("current", out var current))
        {
            return null;
        }

        if (!root.TryGetProperty("current_units", out var currentUnits))
        {
            return null;
        }

        return new ExternalWeatherResponse
        {
            Latitude = GetDecimal(root, "latitude"),
            Longitude = GetDecimal(root, "longitude"),
            Timezone = GetString(root, "timezone"),
            CurrentTime = GetString(current, "time"),
            Temperature = GetDecimal(current, "temperature_2m"),
            TemperatureUnit = GetString(currentUnits, "temperature_2m"),
            RelativeHumidity = GetInt(current, "relative_humidity_2m"),
            RelativeHumidityUnit = GetString(currentUnits, "relative_humidity_2m"),
            WindSpeed = GetDecimal(current, "wind_speed_10m"),
            WindSpeedUnit = GetString(currentUnits, "wind_speed_10m"),
            Source = "Open-Meteo"
        };
    }

    private static decimal GetDecimal(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return 0;
        }

        return property.ValueKind switch
        {
            JsonValueKind.Number => property.GetDecimal(),
            JsonValueKind.String when decimal.TryParse(property.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var value) => value,
            _ => 0
        };
    }
    private static int GetInt(JsonElement element, string propertyName)
{
    if (!element.TryGetProperty(propertyName, out var property))
    {
        return 0;
    }

    return property.ValueKind switch
    {
        JsonValueKind.Number => property.GetInt32(),
        JsonValueKind.String when int.TryParse(property.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var value) => value,
        _ => 0
    };
}

    private static string GetString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return string.Empty;
        }

        return property.ValueKind switch
        {
            JsonValueKind.String => property.GetString() ?? string.Empty,
            JsonValueKind.Number => property.GetRawText(),
            _ => string.Empty
        };
    }
}