using SmartGas.Api.Data;
using SmartGas.Api.Models;

namespace SmartGas.Api.Seeders;

public static class DatabaseSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (!context.Plans.Any())
        {
            context.Plans.AddRange(
                new Plan
                {
                    Name = "Basic",
                    Price = 30,
                    MaxZones = 2,
                    MaxSensors = 4,
                    Features = "Basic monitoring, Web notifications, Incident history"
                },
                new Plan
                {
                    Name = "Professional",
                    Price = 70,
                    MaxZones = 5,
                    MaxSensors = 12,
                    Features = "Advanced monitoring, Reports, Priority alerts"
                },
                new Plan
                {
                    Name = "Corporate",
                    Price = 100,
                    MaxZones = 10,
                    MaxSensors = 30,
                    Features = "Multi-zone monitoring, Advanced reports, Extended support"
                }
            );

            context.SaveChanges();
        }
    }
}