using Microsoft.EntityFrameworkCore;
using SmartGas.Api.Data;
using SmartGas.Api.DTOs;

namespace SmartGas.Api.Services;

public class DashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryResponse?> GetSummaryAsync(int accountId)
    {
        var accountExists = await _context.Accounts
            .AnyAsync(account => account.Id == accountId);

        if (!accountExists)
        {
            return null;
        }

        var currentPlan = await _context.Subscriptions
            .Include(subscription => subscription.Plan)
            .Where(subscription =>
                subscription.AccountId == accountId &&
                subscription.Status == "Active")
            .OrderByDescending(subscription => subscription.StartDate)
            .Select(subscription => subscription.Plan.Name)
            .FirstOrDefaultAsync();

        var totalZones = await _context.Zones
            .CountAsync(zone => zone.AccountId == accountId);

        var totalSensors = await _context.Sensors
            .CountAsync(sensor => sensor.AccountId == accountId);

        var totalReadings = await _context.SensorReadings
            .CountAsync(reading => reading.AccountId == accountId);

        var activeIncidents = await _context.Incidents
            .CountAsync(incident =>
                incident.AccountId == accountId &&
                incident.Status == "Active");

        var criticalIncidents = await _context.Incidents
            .CountAsync(incident =>
                incident.AccountId == accountId &&
                incident.Severity == "Critical" &&
                incident.Status == "Active");

        var unreadNotifications = await _context.Notifications
            .CountAsync(notification =>
                notification.AccountId == accountId &&
                !notification.IsRead);

        var latestReadings = await _context.SensorReadings
            .Include(reading => reading.Sensor)
            .Include(reading => reading.Zone)
            .Where(reading => reading.AccountId == accountId)
            .OrderByDescending(reading => reading.CreatedAt)
            .Take(5)
            .Select(reading => new DashboardLatestReadingResponse
            {
                Id = reading.Id,
                SensorId = reading.SensorId,
                SensorCode = reading.Sensor.Code,
                ZoneId = reading.ZoneId,
                ZoneName = reading.Zone.Name,
                GasLevel = reading.GasLevel,
                Temperature = reading.Temperature,
                CreatedAt = reading.CreatedAt
            })
            .ToListAsync();

        var latestIncidents = await _context.Incidents
            .Include(incident => incident.Sensor)
            .Include(incident => incident.Zone)
            .Where(incident => incident.AccountId == accountId)
            .OrderByDescending(incident => incident.DetectedAt)
            .Take(5)
            .Select(incident => new DashboardLatestIncidentResponse
            {
                Id = incident.Id,
                SensorId = incident.SensorId,
                SensorCode = incident.Sensor.Code,
                ZoneId = incident.ZoneId,
                ZoneName = incident.Zone.Name,
                Type = incident.Type,
                Severity = incident.Severity,
                Status = incident.Status,
                DetectedAt = incident.DetectedAt
            })
            .ToListAsync();

        return new DashboardSummaryResponse
        {
            AccountId = accountId,
            TotalZones = totalZones,
            TotalSensors = totalSensors,
            TotalReadings = totalReadings,
            ActiveIncidents = activeIncidents,
            CriticalIncidents = criticalIncidents,
            UnreadNotifications = unreadNotifications,
            CurrentPlan = currentPlan ?? "No active plan",
            LatestReadings = latestReadings,
            LatestIncidents = latestIncidents
        };
    }
}