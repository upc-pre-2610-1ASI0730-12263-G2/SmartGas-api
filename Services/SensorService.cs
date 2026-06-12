using Microsoft.EntityFrameworkCore;
using SmartGas.Api.Data;
using SmartGas.Api.DTOs;
using SmartGas.Api.Models;

namespace SmartGas.Api.Services;

public class SensorService
{
    private readonly AppDbContext _context;
    private readonly PlanLimitService _planLimitService;

    public SensorService(AppDbContext context, PlanLimitService planLimitService)
    {
        _context = context;
        _planLimitService = planLimitService;
    }

    public async Task<List<SensorResponse>> GetByAccountAsync(int accountId)
    {
        return await _context.Sensors
            .Where(sensor => sensor.AccountId == accountId)
            .OrderByDescending(sensor => sensor.CreatedAt)
            .Select(sensor => ToResponse(sensor))
            .ToListAsync();
    }

    public async Task<SensorResponse?> CreateAsync(CreateSensorRequest request)
    {
        if (request.AccountId <= 0)
        {
            throw new ArgumentException("A valid account id is required.");
        }

        if (request.ZoneId <= 0)
        {
            throw new ArgumentException("A valid zone id is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new ArgumentException("A sensor code is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("A sensor name is required.");
        }

        var accountExists = await _context.Accounts
            .AnyAsync(account => account.Id == request.AccountId);

        if (!accountExists)
        {
            return null;
        }

        var zoneExists = await _context.Zones
            .AnyAsync(zone => zone.Id == request.ZoneId && zone.AccountId == request.AccountId);

        if (!zoneExists)
        {
            return null;
        }

        var code = NormalizeCode(request.Code);

        var codeExists = await _context.Sensors
            .AnyAsync(sensor => sensor.Code == code);

        if (codeExists)
        {
            return null;
        }

        var planLimitValidation = await _planLimitService.CanCreateSensorAsync(request.AccountId);

        if (!planLimitValidation.IsAllowed)
        {
            throw new InvalidOperationException(planLimitValidation.ErrorMessage);
        }

        var sensor = new Sensor
        {
            AccountId = request.AccountId,
            ZoneId = request.ZoneId,
            Code = code,
            Name = request.Name.Trim(),
            Type = NormalizeSensorType(request.Type),
            Status = "Online",
            BatteryLevel = 100,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Sensors.Add(sensor);
        await _context.SaveChangesAsync();

        return ToResponse(sensor);
    }

    public async Task<SensorResponse?> UpdateAsync(int id, UpdateSensorRequest request)
    {
        var sensor = await _context.Sensors
            .FirstOrDefaultAsync(item => item.Id == id);

        if (sensor == null)
        {
            return null;
        }

        if (request.ZoneId.HasValue)
        {
            if (request.ZoneId.Value <= 0)
            {
                throw new ArgumentException("A valid zone id is required.");
            }

            var zoneExists = await _context.Zones
                .AnyAsync(zone =>
                    zone.Id == request.ZoneId.Value &&
                    zone.AccountId == sensor.AccountId);

            if (!zoneExists)
            {
                throw new ArgumentException("The selected zone does not belong to this account.");
            }

            sensor.ZoneId = request.ZoneId.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var code = NormalizeCode(request.Code);

            var codeExists = await _context.Sensors
                .AnyAsync(item => item.Id != sensor.Id && item.Code == code);

            if (codeExists)
            {
                throw new InvalidOperationException("A sensor with the same code already exists.");
            }

            sensor.Code = code;
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            sensor.Name = request.Name.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            sensor.Type = NormalizeSensorType(request.Type);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            sensor.Status = NormalizeStatus(request.Status);
        }

        sensor.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ToResponse(sensor);
    }

    private static SensorResponse ToResponse(Sensor sensor)
    {
        return new SensorResponse
        {
            Id = sensor.Id,
            AccountId = sensor.AccountId,
            ZoneId = sensor.ZoneId,
            Code = sensor.Code,
            Name = sensor.Name,
            Type = sensor.Type,
            Status = sensor.Status,
            BatteryLevel = sensor.BatteryLevel,
            CreatedAt = sensor.CreatedAt,
            UpdatedAt = sensor.UpdatedAt
        };
    }

    private static string NormalizeCode(string code)
    {
        return code.Trim().ToUpperInvariant();
    }

    private static string NormalizeSensorType(string type)
    {
        var raw = string.IsNullOrWhiteSpace(type)
            ? "MultiSensor"
            : type.Trim();

        var normalized = raw
            .ToLowerInvariant()
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty)
            .Replace("_", string.Empty);

        return normalized switch
        {
            "gas" => "Gas",
            "gaslp" => "Gas",
            "lpgas" => "Gas",
            "glp" => "Gas",
            "temperature" => "Temperature",
            "temperatura" => "Temperature",
            "temp" => "Temperature",
            "multisensor" => "MultiSensor",
            "multi" => "MultiSensor",
            "co" => "CO",
            "carbonmonoxide" => "CO",
            "monoxidodecarbono" => "CO",
            "smoke" => "Smoke",
            "humo" => "Smoke",
            _ => raw
        };
    }

    private static string NormalizeStatus(string status)
    {
        var normalized = status
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty)
            .Replace("_", string.Empty);

        return normalized switch
        {
            "online" => "Online",
            "offline" => "Offline",
            "warning" => "Warning",
            "critical" => "Critical",
            _ => throw new ArgumentException("The sensor status is invalid.")
        };
    }
}