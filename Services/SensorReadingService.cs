using Microsoft.EntityFrameworkCore;
using SmartGas.Api.Data;
using SmartGas.Api.DTOs;
using SmartGas.Api.Models;

namespace SmartGas.Api.Services;

public class SensorReadingService
{
    private readonly AppDbContext _context;

    public SensorReadingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SensorReadingResponse>> GetByAccountAsync(int accountId)
    {
        return await _context.SensorReadings
            .Where(reading => reading.AccountId == accountId)
            .OrderByDescending(reading => reading.CreatedAt)
            .Select(reading => new
            {
                Reading = reading,
                Incident = _context.Incidents
                    .Where(incident => incident.SensorReadingId == reading.Id)
                    .FirstOrDefault()
            })
            .Select(item => new SensorReadingResponse
            {
                Id = item.Reading.Id,
                AccountId = item.Reading.AccountId,
                ZoneId = item.Reading.ZoneId,
                SensorId = item.Reading.SensorId,
                GasLevel = item.Reading.GasLevel,
                Temperature = item.Reading.Temperature,
                CreatedAt = item.Reading.CreatedAt,
                IncidentCreated = item.Incident != null,
                IncidentId = item.Incident != null ? item.Incident.Id : null,
                IncidentType = item.Incident != null ? item.Incident.Type : null,
                Severity = item.Incident != null ? item.Incident.Severity : null,
                SensorStatus = item.Incident == null
                    ? "Online"
                    : item.Incident.Severity == "Critical" ? "Critical" : "Warning",
                ZoneStatus = item.Incident == null
                    ? "Safe"
                    : item.Incident.Severity == "Critical" ? "Critical" : "Warning"
            })
            .ToListAsync();
    }

    public async Task<SensorReadingResponse?> CreateAsync(CreateSensorReadingRequest request)
    {
        var sensorCode = request.SensorCode.Trim().ToUpperInvariant();

        var sensor = await _context.Sensors
            .Include(item => item.Zone)
            .FirstOrDefaultAsync(item => item.Code == sensorCode);

        if (sensor == null)
        {
            return null;
        }

        var setting = await _context.Settings
            .FirstOrDefaultAsync(item => item.AccountId == sensor.AccountId);

        var gasThreshold = setting?.GasThreshold ?? 50;
        var temperatureThreshold = setting?.TemperatureThreshold ?? 45;

        var reading = new SensorReading
        {
            AccountId = sensor.AccountId,
            ZoneId = sensor.ZoneId,
            SensorId = sensor.Id,
            GasLevel = request.GasLevel,
            Temperature = request.Temperature,
            CreatedAt = DateTime.UtcNow
        };

        _context.SensorReadings.Add(reading);
        await _context.SaveChangesAsync();

        var incidentType = GetIncidentType(
            sensor.Type,
            request.GasLevel,
            request.Temperature,
            gasThreshold,
            temperatureThreshold);

        var severity = GetSeverity(
            request.GasLevel,
            request.Temperature,
            gasThreshold,
            temperatureThreshold);

        Incident? incident = null;

        if (incidentType != null)
        {
            sensor.Status = severity == "Critical" ? "Critical" : "Warning";
            sensor.UpdatedAt = DateTime.UtcNow;

            sensor.Zone.Status = severity == "Critical" ? "Critical" : "Warning";
            sensor.Zone.UpdatedAt = DateTime.UtcNow;

            incident = new Incident
            {
                AccountId = sensor.AccountId,
                ZoneId = sensor.ZoneId,
                SensorId = sensor.Id,
                SensorReadingId = reading.Id,
                Type = incidentType,
                Severity = severity,
                Status = "Active",
                Notes = "Incident automatically generated from sensor reading.",
                DetectedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();

            var alert = new Alert
            {
                AccountId = sensor.AccountId,
                IncidentId = incident.Id,
                Title = $"{severity} {incidentType} detected",
                Message = $"Sensor {sensor.Code} detected a {incidentType} event in zone {sensor.Zone.Name}.",
                Severity = severity,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();

            var notification = new Notification
            {
                AccountId = sensor.AccountId,
                IncidentId = incident.Id,
                AlertId = alert.Id,
                Channel = "Web",
                Title = alert.Title,
                Message = alert.Message,
                IsRead = false,
                IsConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
        else
        {
            sensor.Status = "Online";
            sensor.UpdatedAt = DateTime.UtcNow;

            sensor.Zone.Status = "Safe";
            sensor.Zone.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        return new SensorReadingResponse
        {
            Id = reading.Id,
            AccountId = reading.AccountId,
            ZoneId = reading.ZoneId,
            SensorId = reading.SensorId,
            GasLevel = reading.GasLevel,
            Temperature = reading.Temperature,
            CreatedAt = reading.CreatedAt,
            IncidentCreated = incident != null,
            IncidentId = incident?.Id,
            IncidentType = incident?.Type,
            Severity = incident?.Severity,
            SensorStatus = sensor.Status,
            ZoneStatus = sensor.Zone.Status
        };
    }

    private static string? GetIncidentType(
        string sensorType,
        decimal? gasLevel,
        decimal? temperature,
        decimal gasThreshold,
        decimal temperatureThreshold)
    {
        var normalizedType = NormalizeSensorTypeForRules(sensorType);

        var gasDanger = gasLevel.HasValue && gasLevel.Value >= gasThreshold;
        var temperatureDanger = temperature.HasValue && temperature.Value >= temperatureThreshold;

        if ((normalizedType == "gas" || normalizedType == "gaslp") && gasDanger)
        {
            return "GasLeak";
        }

        if (normalizedType == "temperature" && temperatureDanger)
        {
            return "HighTemperature";
        }

        if (normalizedType == "multisensor")
        {
            if (gasDanger && temperatureDanger)
            {
                return "GasLeakAndHighTemperature";
            }

            if (gasDanger)
            {
                return "GasLeak";
            }

            if (temperatureDanger)
            {
                return "HighTemperature";
            }
        }

        if (normalizedType == "co" && gasDanger)
        {
            return "CORisk";
        }

        if (normalizedType == "smoke" && (gasDanger || temperatureDanger))
        {
            return "SmokeRisk";
        }

        return null;
    }

    private static string GetSeverity(
        decimal? gasLevel,
        decimal? temperature,
        decimal gasThreshold,
        decimal temperatureThreshold)
    {
        var criticalGas = gasLevel.HasValue && gasLevel.Value >= gasThreshold * 1.5m;
        var criticalTemperature = temperature.HasValue && temperature.Value >= temperatureThreshold * 1.3m;

        if (criticalGas || criticalTemperature)
        {
            return "Critical";
        }

        return "High";
    }

    private static string NormalizeSensorTypeForRules(string sensorType)
    {
        return string.IsNullOrWhiteSpace(sensorType)
            ? "multisensor"
            : sensorType
                .Trim()
                .ToLowerInvariant()
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty);
    }
}