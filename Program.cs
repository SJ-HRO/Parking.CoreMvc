using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Parking.CoreMvc.Data;
using Parking.CoreMvc.Models;
using Parking.CoreMvc.Services;

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
builder.Services.AddScoped<IDataStore, EfDataStore>();
builder.Services.AddScoped<IAsignadorPlazas, AsignadorPlazas>();
builder.Services.AddScoped<ITarificador, Tarificador>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

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

