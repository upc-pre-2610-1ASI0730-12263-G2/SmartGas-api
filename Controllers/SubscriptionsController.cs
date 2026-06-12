using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SmartGas.Api.Data;
using SmartGas.Api.DTOs;
using SmartGas.Api.Models;

namespace SmartGas.Api.Controllers;

[ApiController]
[Route("api/v1/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IStringLocalizer<SubscriptionsController> _localizer;

    public SubscriptionsController(AppDbContext context, IStringLocalizer<SubscriptionsController> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    [HttpGet("current/{accountId:int}")]
    public async Task<IActionResult> GetCurrent(int accountId)
    {
        var subscription = await GetCurrentSubscriptionAsync(accountId);

        if (subscription == null)
        {
            return NotFound(new
            {
                message = _localizer["SubscriptionNotFound"].Value
            });
        }

        return Ok(ToResponse(subscription));
    }

    [HttpPatch("current/{accountId:int}/change-plan")]
    public async Task<IActionResult> ChangePlan(
        int accountId,
        [FromBody] ChangeSubscriptionPlanRequest request)
    {
        if (request.PlanId <= 0)
        {
            return BadRequest(new
            {
                message = _localizer["ValidPlanIdRequired"].Value
            });
        }

        var accountExists = await _context.Accounts
            .AnyAsync(account => account.Id == accountId);

        if (!accountExists)
        {
            return NotFound(new
            {
                message = _localizer["AccountNotFound"].Value
            });
        }

        var plan = await _context.Plans
            .FirstOrDefaultAsync(item => item.Id == request.PlanId && item.IsActive);

        if (plan == null)
        {
            return NotFound(new
            {
                message = _localizer["SelectedPlanNotFound"].Value
            });
        }

        var now = DateTime.UtcNow;

        var subscription = await GetCurrentSubscriptionAsync(accountId);

        if (subscription == null)
        {
            subscription = new Subscription
            {
                AccountId = accountId,
                PlanId = plan.Id,
                Status = "Active",
                StartDate = now,
                RenewalDate = now.AddMonths(1),
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Subscriptions.Add(subscription);
        }
        else
        {
            subscription.PlanId = plan.Id;
            subscription.Status = "Active";
            subscription.UpdatedAt = now;
            subscription.RenewalDate = now.AddMonths(1);
        }

        await _context.SaveChangesAsync();

        var updatedSubscription = await GetCurrentSubscriptionAsync(accountId);

        return Ok(ToResponse(updatedSubscription!));
    }

    private async Task<Subscription?> GetCurrentSubscriptionAsync(int accountId)
    {
        return await _context.Subscriptions
            .Include(subscription => subscription.Plan)
            .Where(subscription => subscription.AccountId == accountId)
            .Where(subscription => subscription.Status == "Active")
            .OrderByDescending(subscription => subscription.UpdatedAt)
            .ThenByDescending(subscription => subscription.CreatedAt)
            .FirstOrDefaultAsync();
    }

    private static object ToResponse(Subscription subscription)
    {
        var plan = subscription.Plan;

        return new
        {
            id = subscription.Id,
            subscriptionId = subscription.Id,
            accountId = subscription.AccountId,
            planId = subscription.PlanId,
            planName = plan?.Name ?? "Basic",
            currentPlan = plan?.Name ?? "Basic",
            status = subscription.Status,
            startDate = subscription.StartDate,
            renewalDate = subscription.RenewalDate,
            createdAt = subscription.CreatedAt,
            updatedAt = subscription.UpdatedAt,
            price = plan?.Price ?? 0,
            maxZones = plan?.MaxZones ?? 0,
            maxSensors = plan?.MaxSensors ?? 0,
            features = plan?.Features ?? string.Empty
        };
    }
}
