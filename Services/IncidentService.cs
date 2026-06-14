using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SmartGas.Api.Data;
using SmartGas.Api.DTOs;
using SmartGas.Api.Models;

namespace SmartGas.Api.Services;

public class IncidentService
{
    private readonly AppDbContext _context;

    public IncidentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<IncidentResponse>> GetByAccountAsync(int accountId)
    {
        return await _context.Incidents
            .Include(incident => incident.Zone)
            .Include(incident => incident.Sensor)
            .Include(incident => incident.SensorReading)
            .Where(incident => incident.AccountId == accountId)
            .OrderByDescending(incident => incident.CreatedAt)
            .Select(incident => ToResponse(incident))
            .ToListAsync();
    }

    public async Task<IncidentResponse?> ReviewAsync(int id)
    {
        var incident = await GetIncidentEntityAsync(id);

        if (incident == null)
        {
            return null;
        }

        incident.Status = "Reviewed";

        await _context.SaveChangesAsync();

        var updatedIncident = await GetIncidentEntityAsync(id);
        return updatedIncident == null ? null : ToResponse(updatedIncident);
    }

    public async Task<IncidentResponse?> ResolveAsync(int id)
    {
        var incident = await GetIncidentEntityAsync(id);

        if (incident == null)
        {
            return null;
        }

        var resolvedAt = DateTime.UtcNow;

        incident.Status = "Resolved";
        incident.ResolvedAt = resolvedAt;

        await CloseRelatedAlertsAsync(incident.Id, "Resolved", resolvedAt);

        await _context.SaveChangesAsync();

        var updatedIncident = await GetIncidentEntityAsync(id);
        return updatedIncident == null ? null : ToResponse(updatedIncident);
    }

    public async Task<IncidentResponse?> FalseAlarmAsync(int id)
    {
        var incident = await GetIncidentEntityAsync(id);

        if (incident == null)
        {
            return null;
        }

        var resolvedAt = DateTime.UtcNow;

        incident.Status = "FalseAlarm";
        incident.ResolvedAt = resolvedAt;

        await CloseRelatedAlertsAsync(incident.Id, "FalseAlarm", resolvedAt);

        await _context.SaveChangesAsync();

        var updatedIncident = await GetIncidentEntityAsync(id);
        return updatedIncident == null ? null : ToResponse(updatedIncident);
    }

    public async Task<IncidentResponse?> MarkFalseAlarmAsync(int id)
    {
        return await FalseAlarmAsync(id);
    }

    public async Task<IncidentResponse?> MarkAsFalseAlarmAsync(int id)
    {
        return await FalseAlarmAsync(id);
    }

    private async Task<Incident?> GetIncidentEntityAsync(int id)
    {
        return await _context.Incidents
            .Include(incident => incident.Zone)
            .Include(incident => incident.Sensor)
            .Include(incident => incident.SensorReading)
            .FirstOrDefaultAsync(incident => incident.Id == id);
    }

    private async Task CloseRelatedAlertsAsync(int incidentId, string status, DateTime resolvedAt)
    {
        var relatedAlerts = await _context.Alerts
            .Where(alert => EF.Property<int>(alert, "IncidentId") == incidentId)
            .ToListAsync();

        foreach (var alert in relatedAlerts)
        {
            SetStringProperty(alert, "Status", status);
            SetDateTimePropertyIfExists(alert, "ResolvedAt", resolvedAt);
            SetDateTimePropertyIfExists(alert, "UpdatedAt", resolvedAt);
        }
    }

    private static IncidentResponse ToResponse(Incident incident)
    {
        var type = GetStringProperty(incident, "Type", "IncidentType");

        if (string.IsNullOrWhiteSpace(type))
        {
            type = "SafetyIncident";
        }

        var zoneName = incident.Zone?.Name ?? $"Zone {incident.ZoneId}";
        var sensorCode = incident.Sensor?.Code ?? $"Sensor {incident.SensorId}";
        var detectedValue = BuildDetectedValue(incident.SensorReading);

        return new IncidentResponse
        {
            Id = incident.Id,
            AccountId = incident.AccountId,
            IncidentId = incident.Id,
            Code = $"INC-{incident.Id.ToString().PadLeft(3, '0')}",
            Type = type,
            Title = $"{incident.Severity} {type} detected",
            Message = $"Sensor {sensorCode} detected a {type} event in zone {zoneName}.",
            ZoneId = incident.ZoneId,
            ZoneName = zoneName,
            SensorId = incident.SensorId,
            SensorCode = sensorCode,
            DetectedValue = detectedValue,
            Severity = incident.Severity,
            Status = incident.Status,
            CreatedAt = incident.CreatedAt,
            DetectedAt = incident.CreatedAt,
            ResolvedAt = incident.ResolvedAt
        };
    }

    private static string BuildDetectedValue(SensorReading? reading)
    {
        if (reading == null)
        {
            return "—";
        }

        var gas = FormatValue(GetPropertyValue(reading, "GasValue", "GasLevel"));
        var temperature = FormatValue(GetPropertyValue(reading, "TemperatureValue", "Temperature"));

        if (!string.IsNullOrWhiteSpace(gas) && !string.IsNullOrWhiteSpace(temperature))
        {
            return $"Gas: {gas} ppm / Temp: {temperature} °C";
        }

        if (!string.IsNullOrWhiteSpace(gas))
        {
            return $"Gas: {gas} ppm";
        }

        if (!string.IsNullOrWhiteSpace(temperature))
        {
            return $"Temp: {temperature} °C";
        }

        return "—";
    }

    private static object? GetPropertyValue(object source, params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            var property = source.GetType().GetProperty(propertyName);

            if (property != null)
            {
                return property.GetValue(source);
            }
        }

        return null;
    }

    private static string GetStringProperty(object source, params string[] propertyNames)
    {
        var value = GetPropertyValue(source, propertyNames);
        return value?.ToString() ?? string.Empty;
    }

    private static void SetStringProperty(object source, string propertyName, string value)
    {
        var property = source.GetType().GetProperty(propertyName);

        if (property != null && property.CanWrite)
        {
            property.SetValue(source, value);
        }
    }

    private static void SetDateTimePropertyIfExists(object source, string propertyName, DateTime value)
    {
        var property = source.GetType().GetProperty(propertyName);

        if (property != null && property.CanWrite)
        {
            property.SetValue(source, value);
        }
    }

    private static string FormatValue(object? value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        if (value is IFormattable formattable)
        {
            return formattable.ToString(null, CultureInfo.InvariantCulture);
        }

        return value.ToString() ?? string.Empty;
    }
}