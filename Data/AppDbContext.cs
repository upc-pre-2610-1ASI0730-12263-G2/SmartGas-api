using Microsoft.EntityFrameworkCore;
using SmartGas.Api.Models;

namespace SmartGas.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Zone> Zones => Set<Zone>();
    public DbSet<Sensor> Sensors => Set<Sensor>();
    public DbSet<SensorReading> SensorReadings => Set<SensorReading>();
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>()
            .HasIndex(account => account.Email)
            .IsUnique();

        modelBuilder.Entity<Sensor>()
            .HasIndex(sensor => sensor.Code)
            .IsUnique();

        modelBuilder.Entity<Account>()
            .HasOne(account => account.Profile)
            .WithOne(profile => profile.Account)
            .HasForeignKey<Profile>(profile => profile.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Account>()
            .HasOne(account => account.Setting)
            .WithOne(setting => setting.Account)
            .HasForeignKey<Setting>(setting => setting.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Subscription>()
            .HasOne(subscription => subscription.Account)
            .WithMany(account => account.Subscriptions)
            .HasForeignKey(subscription => subscription.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Subscription>()
            .HasOne(subscription => subscription.Plan)
            .WithMany(plan => plan.Subscriptions)
            .HasForeignKey(subscription => subscription.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Zone>()
            .HasOne(zone => zone.Account)
            .WithMany(account => account.Zones)
            .HasForeignKey(zone => zone.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Sensor>()
            .HasOne(sensor => sensor.Account)
            .WithMany(account => account.Sensors)
            .HasForeignKey(sensor => sensor.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Sensor>()
            .HasOne(sensor => sensor.Zone)
            .WithMany()
            .HasForeignKey(sensor => sensor.ZoneId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SensorReading>()
            .HasOne(reading => reading.Account)
            .WithMany()
            .HasForeignKey(reading => reading.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SensorReading>()
            .HasOne(reading => reading.Zone)
            .WithMany()
            .HasForeignKey(reading => reading.ZoneId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SensorReading>()
            .HasOne(reading => reading.Sensor)
            .WithMany()
            .HasForeignKey(reading => reading.SensorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Incident>()
            .HasOne(incident => incident.Account)
            .WithMany(account => account.Incidents)
            .HasForeignKey(incident => incident.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Incident>()
            .HasOne(incident => incident.Zone)
            .WithMany()
            .HasForeignKey(incident => incident.ZoneId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Incident>()
            .HasOne(incident => incident.Sensor)
            .WithMany()
            .HasForeignKey(incident => incident.SensorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Incident>()
            .HasOne(incident => incident.SensorReading)
            .WithMany()
            .HasForeignKey(incident => incident.SensorReadingId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Alert>()
            .HasOne(alert => alert.Account)
            .WithMany()
            .HasForeignKey(alert => alert.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Alert>()
            .HasOne(alert => alert.Incident)
            .WithMany()
            .HasForeignKey(alert => alert.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(notification => notification.Account)
            .WithMany(account => account.Notifications)
            .HasForeignKey(notification => notification.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(notification => notification.Incident)
            .WithMany()
            .HasForeignKey(notification => notification.IncidentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Notification>()
            .HasOne(notification => notification.Alert)
            .WithMany()
            .HasForeignKey(notification => notification.AlertId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<EmergencyContact>(entity =>
        {
            entity.HasKey(emergencyContact => emergencyContact.Id);

            entity.Property(emergencyContact => emergencyContact.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(emergencyContact => emergencyContact.Phone)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(emergencyContact => emergencyContact.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasIndex(emergencyContact => emergencyContact.AccountId)
                .IsUnique();

            entity.HasOne(emergencyContact => emergencyContact.Account)
                .WithOne()
                .HasForeignKey<EmergencyContact>(emergencyContact => emergencyContact.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}