using Microsoft.EntityFrameworkCore;
using SmartGas.Api.Data;

namespace SmartGas.Api.Services;

public class PlanLimitService
{
    private readonly AppDbContext _context;

    public PlanLimitService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PlanLimitValidationResult> CanCreateZoneAsync(int accountId)
    {
        var subscription = await _context.Subscriptions
            .Include(subscription => subscription.Plan)
            .Where(subscription =>
                subscription.AccountId == accountId &&
                subscription.Status == "Active")
            .OrderByDescending(subscription => subscription.StartDate)
            .FirstOrDefaultAsync();

        if (subscription == null)
        {
            return PlanLimitValidationResult.Fail(
                "The account does not have an active subscription.");
        }

        var currentZones = await _context.Zones
            .CountAsync(zone => zone.AccountId == accountId);

        var maxZones = subscription.Plan.MaxZones;

        if (currentZones >= maxZones)
        {
            return PlanLimitValidationResult.Fail(
                $"Plan limit reached. Your current plan allows up to {maxZones} zones.");
        }

        return PlanLimitValidationResult.Success();
    }

    public async Task<PlanLimitValidationResult> CanCreateSensorAsync(int accountId)
    {
        var subscription = await _context.Subscriptions
            .Include(subscription => subscription.Plan)
            .Where(subscription =>
                subscription.AccountId == accountId &&
                subscription.Status == "Active")
            .OrderByDescending(subscription => subscription.StartDate)
            .FirstOrDefaultAsync();

        if (subscription == null)
        {
            return PlanLimitValidationResult.Fail(
                "The account does not have an active subscription.");
        }

        var currentSensors = await _context.Sensors
            .CountAsync(sensor => sensor.AccountId == accountId);

        var maxSensors = subscription.Plan.MaxSensors;

        if (currentSensors >= maxSensors)
        {
            return PlanLimitValidationResult.Fail(
                $"Plan limit reached. Your current plan allows up to {maxSensors} sensors.");
        }

        return PlanLimitValidationResult.Success();
    }
}

public class PlanLimitValidationResult
{
    public bool IsAllowed { get; set; }

    public string? ErrorMessage { get; set; }

    public static PlanLimitValidationResult Success()
    {
        return new PlanLimitValidationResult
        {
            IsAllowed = true
        };
    }

    public static PlanLimitValidationResult Fail(string errorMessage)
    {
        return new PlanLimitValidationResult
        {
            IsAllowed = false,
            ErrorMessage = errorMessage
        };
    }
}