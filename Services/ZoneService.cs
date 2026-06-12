using Microsoft.EntityFrameworkCore;
using SmartGas.Api.Data;
using SmartGas.Api.DTOs;
using SmartGas.Api.Models;

namespace SmartGas.Api.Services;

public class ZoneService
{
    private readonly AppDbContext _context;
    private readonly PlanLimitService _planLimitService;

    public ZoneService(AppDbContext context, PlanLimitService planLimitService)
    {
        _context = context;
        _planLimitService = planLimitService;
    }

    public async Task<List<ZoneResponse>> GetByAccountAsync(int accountId)
    {
        return await _context.Zones
            .Where(zone => zone.AccountId == accountId)
            .OrderByDescending(zone => zone.CreatedAt)
            .Select(zone => new ZoneResponse
            {
                Id = zone.Id,
                AccountId = zone.AccountId,
                Name = zone.Name,
                Description = zone.Description,
                Status = zone.Status,
                Sensitivity = zone.Sensitivity,
                CreatedAt = zone.CreatedAt,
                UpdatedAt = zone.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<ZoneResponse?> CreateAsync(CreateZoneRequest request)
    {
        var accountExists = await _context.Accounts.AnyAsync(account => account.Id == request.AccountId);

        if (!accountExists)
        {
            return null;
        }

        var planLimitValidation = await _planLimitService.CanCreateZoneAsync(request.AccountId);

        if (!planLimitValidation.IsAllowed)
        {
            throw new InvalidOperationException(planLimitValidation.ErrorMessage);
        }

        var zone = new Zone
        {
            AccountId = request.AccountId,
            Name = request.Name,
            Description = request.Description,
            Sensitivity = request.Sensitivity,
            Status = "Safe"
        };

        _context.Zones.Add(zone);
        await _context.SaveChangesAsync();

        return new ZoneResponse
        {
            Id = zone.Id,
            AccountId = zone.AccountId,
            Name = zone.Name,
            Description = zone.Description,
            Status = zone.Status,
            Sensitivity = zone.Sensitivity,
            CreatedAt = zone.CreatedAt,
            UpdatedAt = zone.UpdatedAt
        };
    }
}
