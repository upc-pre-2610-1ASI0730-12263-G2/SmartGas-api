using Microsoft.EntityFrameworkCore;
using SmartGas.Api.Data;
using SmartGas.Api.DTOs;

namespace SmartGas.Api.Services;

public class SettingService
{
    private readonly AppDbContext _context;

    public SettingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SettingResponse?> GetByAccountAsync(int accountId)
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(setting => setting.AccountId == accountId);

        if (setting == null)
        {
            return null;
        }

        return new SettingResponse
        {
            Id = setting.Id,
            AccountId = setting.AccountId,
            Language = setting.Language,
            DarkMode = setting.DarkMode,
            NotificationsEnabled = setting.NotificationsEnabled,
            GasThreshold = setting.GasThreshold,
            TemperatureThreshold = setting.TemperatureThreshold,
            CreatedAt = setting.CreatedAt,
            UpdatedAt = setting.UpdatedAt
        };
    }

    public async Task<SettingResponse?> UpdateAsync(int accountId, UpdateSettingRequest request)
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(setting => setting.AccountId == accountId);

        if (setting == null)
        {
            return null;
        }

        setting.Language = request.Language;
        setting.DarkMode = request.DarkMode;
        setting.NotificationsEnabled = request.NotificationsEnabled;
        setting.GasThreshold = request.GasThreshold;
        setting.TemperatureThreshold = request.TemperatureThreshold;
        setting.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new SettingResponse
        {
            Id = setting.Id,
            AccountId = setting.AccountId,
            Language = setting.Language,
            DarkMode = setting.DarkMode,
            NotificationsEnabled = setting.NotificationsEnabled,
            GasThreshold = setting.GasThreshold,
            TemperatureThreshold = setting.TemperatureThreshold,
            CreatedAt = setting.CreatedAt,
            UpdatedAt = setting.UpdatedAt
        };
    }
}
