using System.Globalization;
using EntryProject.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// PostgreSQL via EF Core (Npgsql).
var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Chybí connection string 'Postgres'.");
builder.Services.AddDbContext<TriageDbContext>(opt => opt.UseNpgsql(connectionString));

builder.Services.AddScoped<ResultRepository>();

var app = builder.Build();

// Czech culture – correct diacritics, number and date formatting.
var czechCulture = new CultureInfo("cs-CZ");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(czechCulture),
    SupportedCultures = new[] { czechCulture },
    SupportedUICultures = new[] { czechCulture }
};
app.UseRequestLocalization(localizationOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// Migrations + idempotent application of procedures and seed on startup.
await StartupSql.InitializeAsync(app.Services, app.Logger);

app.Run();
