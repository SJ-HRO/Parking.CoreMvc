using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Parking.CoreMvc.Data;
using Parking.CoreMvc.Models;
using Parking.CoreMvc.Services;
using Parking.CoreMvc.Services.Tariffs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddHostedService<OccupancySnapshotService>();

// DataStore (como está)
builder.Services.AddScoped<IDataStore, EfDataStore>();

// Lógica de negocio
builder.Services.AddScoped<IAsignadorPlazas, AsignadorPlazas>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IComparisonService, ComparisonService>();

// Tarificación con Strategy + Factory
builder.Services.AddScoped<ITariffStrategy, FractionTariffStrategy>();
builder.Services.AddScoped<ITariffStrategy, HourlyTariffStrategy>();
builder.Services.AddScoped<ITariffStrategyFactory, TariffStrategyFactory>();
builder.Services.AddScoped<ITarificador, Tarificador>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

await IdentitySeed.SeedAsync(app.Services);

app.Run();
