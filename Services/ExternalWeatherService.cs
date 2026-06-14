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
            "&current_weather=true" +
            "&timezone=auto";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.UserAgent.ParseAdd("SmartGas.Api/1.0");

        using var response = await _httpClient.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var latitudeValue = GetDecimal(root, "latitude");
        var longitudeValue = GetDecimal(root, "longitude");
        var timezone = GetString(root, "timezone");

        if (root.TryGetProperty("current", out var current))
        {
            var currentUnits = root.TryGetProperty("current_units", out var units)
                ? units
                : default;

            return new ExternalWeatherResponse
            {
                Latitude = latitudeValue,
                Longitude = longitudeValue,
                Timezone = timezone,
                CurrentTime = GetString(current, "time"),
                Temperature = GetDecimal(current, "temperature_2m"),
                TemperatureUnit = GetString(currentUnits, "temperature_2m", "°C"),
                RelativeHumidity = GetInt(current, "relative_humidity_2m"),
                RelativeHumidityUnit = GetString(currentUnits, "relative_humidity_2m", "%"),
                WindSpeed = GetDecimal(current, "wind_speed_10m"),
                WindSpeedUnit = GetString(currentUnits, "wind_speed_10m", "km/h"),
                Source = "Open-Meteo"
            };
        }

        if (root.TryGetProperty("current_weather", out var currentWeather))
        {
            return new ExternalWeatherResponse
            {
                Latitude = latitudeValue,
                Longitude = longitudeValue,
                Timezone = timezone,
                CurrentTime = GetString(currentWeather, "time"),
                Temperature = GetDecimal(currentWeather, "temperature"),
                TemperatureUnit = "°C",
                RelativeHumidity = 0,
                RelativeHumidityUnit = "%",
                WindSpeed = GetDecimal(currentWeather, "windspeed"),
                WindSpeedUnit = "km/h",
                Source = "Open-Meteo"
            };
        }

        return null;
    }

    private static decimal GetDecimal(JsonElement element, string propertyName)
    {
        if (element.ValueKind == JsonValueKind.Undefined)
        {
            return 0;
        }

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
        if (element.ValueKind == JsonValueKind.Undefined)
        {
            return 0;
        }

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

    private static string GetString(JsonElement element, string propertyName, string fallback = "")
    {
        if (element.ValueKind == JsonValueKind.Undefined)
        {
            return fallback;
        }

        if (!element.TryGetProperty(propertyName, out var property))
        {
            return fallback;
        }

        return property.ValueKind switch
        {
            JsonValueKind.String => property.GetString() ?? fallback,
            JsonValueKind.Number => property.GetRawText(),
            _ => fallback
        };
    }
}