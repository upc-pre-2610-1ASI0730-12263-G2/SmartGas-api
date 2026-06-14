using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using SmartGas.Api.Data;
using SmartGas.Api.Seeders;
using SmartGas.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var allowedCorsPolicy = "AllowVueApp";

var allowedOrigins = new List<string>
{
    "http://localhost:5173",
    "http://localhost:5174",
    "https://localhost:5173",
    "https://smartgas-app-web-v1.web.app",
    "https://smartgas-app-web-v1.firebaseapp.com"
};

var frontendUrl = builder.Configuration["FRONTEND_URL"];

if (!string.IsNullOrWhiteSpace(frontendUrl))
{
    allowedOrigins.Add(frontendUrl.Trim().TrimEnd('/'));
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(allowedCorsPolicy, policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrWhiteSpace(origin))
                {
                    return false;
                }

                var normalizedOrigin = origin.Trim().TrimEnd('/');

                return allowedOrigins.Contains(normalizedOrigin);
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ZoneService>();
builder.Services.AddScoped<SensorService>();
builder.Services.AddScoped<SensorReadingService>();
builder.Services.AddScoped<IncidentService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<PlanService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<SettingService>();
builder.Services.AddScoped<PlanLimitService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddHttpClient<ExternalWeatherService>();
builder.Services.AddScoped<EmergencyContactService>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("es-419"),
    new CultureInfo("es-ES")
};

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

app.UseRequestLocalization(localizationOptions);

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    DatabaseSeeder.Seed(context);
}

app.UseCors(allowedCorsPolicy);

app.MapControllers();

app.MapGet("/", () => "SmartGas API is running.");

app.Run();
