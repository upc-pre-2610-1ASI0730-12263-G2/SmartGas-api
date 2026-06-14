using System.Text.Json.Serialization;

namespace SmartGas.Api.DTOs;

public class ExternalWeatherResponse
{
    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public string Timezone { get; set; } = string.Empty;

    public string CurrentTime { get; set; } = string.Empty;

    public decimal Temperature { get; set; }

    public string TemperatureUnit { get; set; } = string.Empty;

    public int RelativeHumidity { get; set; }

    public string RelativeHumidityUnit { get; set; } = string.Empty;

    public decimal WindSpeed { get; set; }

    public string WindSpeedUnit { get; set; } = string.Empty;

    public string Source { get; set; } = "Open-Meteo";
}

public class OpenMeteoForecastResponse
{
    [JsonPropertyName("latitude")]
    public decimal Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public decimal Longitude { get; set; }

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; } = string.Empty;

    [JsonPropertyName("current_units")]
    public OpenMeteoCurrentUnitsResponse CurrentUnits { get; set; } = new();

    [JsonPropertyName("current")]
    public OpenMeteoCurrentResponse Current { get; set; } = new();
}

public class OpenMeteoCurrentUnitsResponse
{
    [JsonPropertyName("temperature_2m")]
    public string Temperature2m { get; set; } = string.Empty;

    [JsonPropertyName("relative_humidity_2m")]
    public string RelativeHumidity2m { get; set; } = string.Empty;

    [JsonPropertyName("wind_speed_10m")]
    public string WindSpeed10m { get; set; } = string.Empty;
}

public class OpenMeteoCurrentResponse
{
    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;

    [JsonPropertyName("temperature_2m")]
    public decimal Temperature2m { get; set; }

    [JsonPropertyName("relative_humidity_2m")]
    public int RelativeHumidity2m { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public decimal WindSpeed10m { get; set; }
}