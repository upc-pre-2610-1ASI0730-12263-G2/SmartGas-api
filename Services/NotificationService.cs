using Microsoft.EntityFrameworkCore;
using SmartGas.Api.Data;
using SmartGas.Api.DTOs;

namespace SmartGas.Api.Services;

public class NotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificationResponse>> GetByAccountAsync(int accountId)
    {
        return await _context.Notifications
            .Where(notification => notification.AccountId == accountId)
            .OrderByDescending(notification => notification.CreatedAt)
            .Select(notification => new NotificationResponse
            {
                Id = notification.Id,
                AccountId = notification.AccountId,
                IncidentId = notification.IncidentId,
                AlertId = notification.AlertId,
                Channel = notification.Channel,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                IsConfirmed = notification.IsConfirmed,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt,
                ConfirmedAt = notification.ConfirmedAt
            })
            .ToListAsync();
    }

    public async Task<NotificationResponse?> MarkAsReadAsync(int id)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(notification => notification.Id == id);

        if (notification == null)
        {
            return null;
        }

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new NotificationResponse
        {
            Id = notification.Id,
            AccountId = notification.AccountId,
            IncidentId = notification.IncidentId,
            AlertId = notification.AlertId,
            Channel = notification.Channel,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            IsConfirmed = notification.IsConfirmed,
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt,
            ConfirmedAt = notification.ConfirmedAt
        };
    }

    public async Task<NotificationResponse?> ConfirmAsync(int id)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(notification => notification.Id == id);

        if (notification == null)
        {
            return null;
        }

        notification.IsConfirmed = true;
        notification.ConfirmedAt = DateTime.UtcNow;

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return new NotificationResponse
        {
            Id = notification.Id,
            AccountId = notification.AccountId,
            IncidentId = notification.IncidentId,
            AlertId = notification.AlertId,
            Channel = notification.Channel,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            IsConfirmed = notification.IsConfirmed,
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt,
            ConfirmedAt = notification.ConfirmedAt
        };
    }
}