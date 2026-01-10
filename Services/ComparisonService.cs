using Microsoft.EntityFrameworkCore;
using Parking.CoreMvc.Data;
using Parking.CoreMvc.Models;

namespace Parking.CoreMvc.Services
{
    public class ComparisonService : IComparisonService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ComparisonService> _logger;

        public ComparisonService(ApplicationDbContext db, ILogger<ComparisonService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<HistoricoOcupacion>> GetUltimosAsync(int take = 30)
        {
            if (take <= 0) take = 30;

            return await _db.HistoricoOcupaciones
                .AsNoTracking()
                .OrderByDescending(h => h.Momento)
                .Take(take)
                .ToListAsync();
        }

        public async Task<ComparisonResultDto> CompareHistoricosAsync(int aId, int bId, double umbral)
        {
            if (aId == bId)
                throw new InvalidOperationException("Selecciona dos snapshots distintos.");

            _logger.LogInformation(
                "Comparando históricos (dbo.HistoricoOcupaciones) A={aId} vs B={bId} con umbral={umbral}",
                aId, bId, umbral);

            var a = await _db.HistoricoOcupaciones
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == aId);

            var b = await _db.HistoricoOcupaciones
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == bId);

            if (a == null || b == null)
            {
                _logger.LogWarning(
                    "No se encontraron ambos snapshots. A={aId} existe={aOk}, B={bId} existe={bOk}",
                    aId, a != null, bId, b != null);

                throw new InvalidOperationException("No se encontraron ambos snapshots.");
            }

            // Orden temporal para evitar confusión
            var first = a.Momento <= b.Momento ? a : b;
            var second = a.Momento <= b.Momento ? b : a;

            return new ComparisonResultDto
            {
                Umbral = umbral,
                A = Map(first),
                B = Map(second)
            };
        }

        public async Task<ComparisonResultDto> CompareHistoricoVsActualAsync(int historicoId, double umbral)
        {
            _logger.LogInformation(
                "Comparando histórico vs actual. A=dbo.HistoricoOcupaciones Id={historicoId}, B=dbo.Plazas+dbo.Tickets, umbral={umbral}",
                historicoId, umbral);

            var h = await _db.HistoricoOcupaciones
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == historicoId);

            if (h == null)
            {
                _logger.LogWarning("Snapshot histórico no encontrado. Id={historicoId}", historicoId);
                throw new InvalidOperationException("No se encontró el snapshot histórico.");
            }

            // Estado actual (otras tablas)
            var plazasTotales = await _db.Plazas.AsNoTracking().CountAsync();
            var plazasOcupadas = await _db.Plazas.AsNoTracking().CountAsync(p => p.Estado == EstadoPlaza.Ocupada);
            var plazasLibres = plazasTotales - plazasOcupadas;
            var ticketsActivos = await _db.Tickets.AsNoTracking().CountAsync(t => t.Estado == EstadoTicket.Activo);

            var actual = new SnapshotDto
            {
                Id = 0, // no existe como registro en histórico
                Momento = DateTime.Now,
                PlazasTotales = plazasTotales,
                PlazasOcupadas = plazasOcupadas,
                PlazasLibres = plazasLibres,
                TicketsActivos = ticketsActivos
            };

            _logger.LogInformation(
                "Actual: Totales={tot}, Ocupadas={occ}, Libres={lib}, Activos={act}",
                plazasTotales, plazasOcupadas, plazasLibres, ticketsActivos);

            return new ComparisonResultDto
            {
                Umbral = umbral,
                A = Map(h),
                B = actual
            };
        }

        private static SnapshotDto Map(HistoricoOcupacion h) => new SnapshotDto
        {
            Id = h.Id,
            Momento = h.Momento,
            PlazasTotales = h.PlazasTotales,
            PlazasOcupadas = h.PlazasOcupadas,
            PlazasLibres = h.PlazasLibres,
            TicketsActivos = h.TicketsActivos
        };
    }
}