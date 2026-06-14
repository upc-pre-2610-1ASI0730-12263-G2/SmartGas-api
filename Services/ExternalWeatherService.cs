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
            $"https://api.open-meteo.com/v1/forecast" +
            $"?latitude={latitudeText}" +
            $"&longitude={longitudeText}" +
            $"&current=temperature_2m,relative_humidity_2m,wind_speed_10m" +
            $"&timezone=auto";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.UserAgent.ParseAdd("SmartGas.Api/1.0");

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var openMeteoResponse = await JsonSerializer.DeserializeAsync<OpenMeteoForecastResponse>(stream, options);

        if (openMeteoResponse?.Current == null || openMeteoResponse.CurrentUnits == null)
        {
            return null;
        }

        return new ExternalWeatherResponse
        {
            Latitude = openMeteoResponse.Latitude,
            Longitude = openMeteoResponse.Longitude,
            Timezone = openMeteoResponse.Timezone,
            CurrentTime = openMeteoResponse.Current.Time,
            Temperature = openMeteoResponse.Current.Temperature2m,
            TemperatureUnit = openMeteoResponse.CurrentUnits.Temperature2m,
            RelativeHumidity = openMeteoResponse.Current.RelativeHumidity2m,
            RelativeHumidityUnit = openMeteoResponse.CurrentUnits.RelativeHumidity2m,
            WindSpeed = openMeteoResponse.Current.WindSpeed10m,
            WindSpeedUnit = openMeteoResponse.CurrentUnits.WindSpeed10m,
            Source = "Open-Meteo"
        };
    }
}