using Microsoft.EntityFrameworkCore;
using SmartGas.Api.Data;
using SmartGas.Api.DTOs;
using SmartGas.Api.Models;

namespace SmartGas.Api.Services;

public class EmergencyContactService
{
    private readonly AppDbContext _context;

    public EmergencyContactService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EmergencyContactResponse?> GetByAccountAsync(int accountId)
    {
        var accountExists = await _context.Accounts
            .AnyAsync(account => account.Id == accountId);

        if (!accountExists)
        {
            return null;
        }

        var emergencyContact = await _context.EmergencyContacts
            .FirstOrDefaultAsync(contact => contact.AccountId == accountId);

        if (emergencyContact == null)
        {
            return new EmergencyContactResponse
            {
                Id = 0,
                AccountId = accountId,
                Name = string.Empty,
                Phone = string.Empty,
                Email = string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        return ToResponse(emergencyContact);
    }

    public async Task<EmergencyContactResponse?> UpdateAsync(
        int accountId,
        UpdateEmergencyContactRequest request)
    {
        var accountExists = await _context.Accounts
            .AnyAsync(account => account.Id == accountId);

        if (!accountExists)
        {
            return null;
        }

        var name = request.Name.Trim();
        var phone = request.Phone.Trim();
        var email = request.Email.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Emergency contact name is required.");
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("Emergency contact phone is required.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Emergency contact email is required.");
        }

        var emergencyContact = await _context.EmergencyContacts
            .FirstOrDefaultAsync(contact => contact.AccountId == accountId);

        if (emergencyContact == null)
        {
            emergencyContact = new EmergencyContact
            {
                AccountId = accountId,
                Name = name,
                Phone = phone,
                Email = email
            };

            _context.EmergencyContacts.Add(emergencyContact);
        }
        else
        {
            emergencyContact.Name = name;
            emergencyContact.Phone = phone;
            emergencyContact.Email = email;
            emergencyContact.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return ToResponse(emergencyContact);
    }

    private static EmergencyContactResponse ToResponse(EmergencyContact emergencyContact)
    {
        return new EmergencyContactResponse
        {
            Id = emergencyContact.Id,
            AccountId = emergencyContact.AccountId,
            Name = emergencyContact.Name,
            Phone = emergencyContact.Phone,
            Email = emergencyContact.Email,
            CreatedAt = emergencyContact.CreatedAt,
            UpdatedAt = emergencyContact.UpdatedAt
        };
    }
}