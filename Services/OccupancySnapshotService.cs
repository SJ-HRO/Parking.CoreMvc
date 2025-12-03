using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parking.CoreMvc.Data;
using Parking.CoreMvc.Models;

namespace Parking.CoreMvc.Services
{
    public class OccupancySnapshotService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _intervalo = TimeSpan.FromMinutes(5); // cada 5 minutos

        public OccupancySnapshotService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await TomarSnapshotAsync(stoppingToken);
                }
                catch
                {
                    // en un proyecto real se loguearía la excepción
                }

                await Task.Delay(_intervalo, stoppingToken);
            }
        }

        private async Task TomarSnapshotAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var plazasTotales = await db.Plazas.CountAsync(cancellationToken);
            var plazasOcupadas = await db.Plazas.CountAsync(p => p.Estado == EstadoPlaza.Ocupada, cancellationToken);
            var plazasLibres = plazasTotales - plazasOcupadas;

            var ticketsActivos = await db.Tickets.CountAsync(t => t.Estado == EstadoTicket.Activo, cancellationToken);

            var ahora = DateTime.Now;

            var registro = new HistoricoOcupacion
            {
                Momento = ahora,
                PlazasTotales = plazasTotales,
                PlazasOcupadas = plazasOcupadas,
                PlazasLibres = plazasLibres,
                TicketsActivos = ticketsActivos,
                Anio = ahora.Year,
                Mes = ahora.Month,
                Dia = ahora.Day,
                Hora = ahora.Hour,
                DiaSemana = (int)ahora.DayOfWeek 
            };

            db.HistoricoOcupaciones.Add(registro);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
