using Microsoft.EntityFrameworkCore;
using SmartGas.Api.Data;
using SmartGas.Api.DTOs;

namespace SmartGas.Api.Services;

public class PlanService
{
    private readonly AppDbContext _context;

    public PlanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PlanResponse>> GetAllAsync()
    {
        return await _context.Plans
            .Where(plan => plan.IsActive)
            .OrderBy(plan => plan.Price)
            .Select(plan => new PlanResponse
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                MaxZones = plan.MaxZones,
                MaxSensors = plan.MaxSensors,
                Features = plan.Features,
                IsActive = plan.IsActive
            })
            .ToListAsync();
    }

    public async Task<SubscriptionResponse?> GetCurrentSubscriptionAsync(int accountId)
    {
        var subscription = await _context.Subscriptions
            .Include(subscription => subscription.Plan)
            .Where(subscription => subscription.AccountId == accountId && subscription.Status == "Active")
            .OrderByDescending(subscription => subscription.StartDate)
            .FirstOrDefaultAsync();

        if (subscription == null)
        {
            return null;
        }

        return new SubscriptionResponse
        {
            Id = subscription.Id,
            AccountId = subscription.AccountId,
            PlanId = subscription.PlanId,
            PlanName = subscription.Plan.Name,
            Price = subscription.Plan.Price,
            MaxZones = subscription.Plan.MaxZones,
            MaxSensors = subscription.Plan.MaxSensors,
            Status = subscription.Status,
            StartDate = subscription.StartDate,
            RenewalDate = subscription.RenewalDate
        };
    }
}