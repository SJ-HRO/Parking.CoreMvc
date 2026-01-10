using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Parking.CoreMvc.Models;

namespace Parking.CoreMvc.Data
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            string[] roles = { "Administrador", "Operador" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Passwords por config
            var adminPwd = config["Seed:AdminPassword"] ?? Environment.GetEnvironmentVariable("SEED_ADMIN_PASSWORD") ?? "Admin123$";
            var operPwd = config["Seed:OperadorPassword"] ?? Environment.GetEnvironmentVariable("SEED_OPERADOR_PASSWORD") ?? "Operador123$";

            var adminEmail = "admin@parqueadero.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPwd);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
            }

            var operadorEmail = "operador@parqueadero.com";
            var operadorUser = await userManager.FindByEmailAsync(operadorEmail);

            if (operadorUser == null)
            {
                operadorUser = new IdentityUser
                {
                    UserName = operadorEmail,
                    Email = operadorEmail,
                    EmailConfirmed = true
                };

                var resultOperador = await userManager.CreateAsync(operadorUser, operPwd);
                if (resultOperador.Succeeded)
                    await userManager.AddToRoleAsync(operadorUser, "Operador");
            }

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!await db.Zonas.AnyAsync())
            {
                var zonaA = new Zona { Nombre = "Zona A", Capacidad = 20 };
                db.Zonas.Add(zonaA);

                for (int i = 1; i <= 10; i++)
                {
                    db.Plazas.Add(new Plaza
                    {
                        Codigo = $"A-{i:D2}",
                        Zona = zonaA,
                        Tipo = "Auto",
                        Estado = EstadoPlaza.Libre
                    });
                }
            }

            if (!await db.Tarifas.AnyAsync())
            {
                db.Tarifas.Add(new Tarifa
                {
                    Nombre = "General Día",
                    Unidad = "Fraccion",
                    ValorBase = 0.50m,
                    FraccionMinutos = 15,
                    HoraInicio = new TimeSpan(6, 0, 0),
                    HoraFin = new TimeSpan(22, 0, 0),
                    AplicaFinDeSemana = true,
                    TipoVehiculo = "Auto"
                });
            }

            if (!await db.Tickets.AnyAsync())
            {
                var ahora = DateTime.Now;

                var plaza1 = await db.Plazas.OrderBy(p => p.Id).FirstOrDefaultAsync();
                var plaza2 = await db.Plazas.OrderBy(p => p.Id).Skip(1).FirstOrDefaultAsync();

                if (plaza1 != null)
                {
                    plaza1.Estado = EstadoPlaza.Ocupada;

                    db.Tickets.Add(new Ticket
                    {
                        Placa = "ABC1234",
                        TipoVehiculo = "Auto",
                        HoraEntrada = ahora.AddMinutes(-45),
                        Estado = EstadoTicket.Activo,
                        Plaza = plaza1
                    });
                }

                if (plaza2 != null)
                {
                    db.Tickets.Add(new Ticket
                    {
                        Placa = "XYZ5678",
                        TipoVehiculo = "Auto",
                        HoraEntrada = ahora.AddHours(-3),
                        HoraSalida = ahora.AddHours(-1),
                        Estado = EstadoTicket.Cerrado,
                        Plaza = plaza2,
                        MontoCalculado = 3.50m
                    });
                }

                db.Tickets.Add(new Ticket
                {
                    Placa = "PSQ9999",
                    TipoVehiculo = "Auto",
                    HoraEntrada = ahora.AddMinutes(-10),
                    Estado = EstadoTicket.EnEspera
                });
            }

            await db.SaveChangesAsync();
        }
    }
}
