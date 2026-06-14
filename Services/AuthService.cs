using Microsoft.EntityFrameworkCore;
using SmartGas.Api.Data;
using SmartGas.Api.DTOs;
using SmartGas.Api.Models;

namespace SmartGas.Api.Services;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AuthResponse?> SignUpAsync(SignUpRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var emailExists = await _context.Accounts.AnyAsync(account => account.Email == email);

        if (emailExists)
        {
            return null;
        }

        var basicPlan = await _context.Plans.FirstOrDefaultAsync(plan => plan.Name == "Basic");

        if (basicPlan == null)
        {
            return null;
        }

        var account = new Account
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            Status = "Active"
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        var profile = new Profile
        {
            AccountId = account.Id,
            FullName = request.FullName,
            BusinessName = request.BusinessName,
            Phone = request.Phone,
            District = request.District
        };

        var setting = new Setting
        {
            AccountId = account.Id,
            Language = "en-US",
            DarkMode = false,
            NotificationsEnabled = true
        };

        var subscription = new Subscription
        {
            AccountId = account.Id,
            PlanId = basicPlan.Id,
            Status = "Active"
        };

        _context.Profiles.Add(profile);
        _context.Settings.Add(setting);
        _context.Subscriptions.Add(subscription);

        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            AccountId = account.Id,
            Email = account.Email,
            FullName = profile.FullName,
            Role = account.Role,
            Status = account.Status,
            PlanId = basicPlan.Id,
            PlanName = basicPlan.Name
        };
    }

    public async Task<AuthResponse?> SignInAsync(SignInRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var account = await _context.Accounts
            .Include(account => account.Profile)
            .Include(account => account.Subscriptions)
                .ThenInclude(subscription => subscription.Plan)
            .FirstOrDefaultAsync(account => account.Email == email);

        if (account == null)
        {
            return null;
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, account.PasswordHash);

        if (!passwordValid)
        {
            return null;
        }

        var activeSubscription = account.Subscriptions
            .FirstOrDefault(subscription => subscription.Status == "Active");

        return new AuthResponse
        {
            AccountId = account.Id,
            Email = account.Email,
            FullName = account.Profile?.FullName ?? string.Empty,
            Role = account.Role,
            Status = account.Status,
            PlanId = activeSubscription?.PlanId ?? 0,
            PlanName = activeSubscription?.Plan?.Name ?? string.Empty
        };
    }
}