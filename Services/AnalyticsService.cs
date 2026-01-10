using Microsoft.EntityFrameworkCore;
using Parking.CoreMvc.Data;

namespace Parking.CoreMvc.Services
{
    public class HoraPicoDto
    {
        public int Hora { get; set; }                 
        public double OcupacionPromedio { get; set; } 
    }

    public class AnalyticsSummaryDto
    {
        public double OcupacionPromedioGlobal { get; set; } 
        public int MaximoOcupadas { get; set; }
        public int PlazasTotales { get; set; }
        public int PlazasBloqueablesSugeridas { get; set; }
        public List<HoraPicoDto> HorasPico { get; set; } = new();
    }

    public interface IAnalyticsService
    {
        Task<AnalyticsSummaryDto> GetResumenAsync(double toleranciaOcupacion = 0.85);
    }

    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(ApplicationDbContext db, ILogger<AnalyticsService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<AnalyticsSummaryDto> GetResumenAsync(double toleranciaOcupacion = 0.85)
        {
            toleranciaOcupacion = NormalizeTolerancia(toleranciaOcupacion);

            _logger.LogInformation("Generando resumen analítico con tolerancia={tol}", toleranciaOcupacion);

            var plazasTotales = await _db.Plazas.AsNoTracking().CountAsync();

            var historico = await _db.HistoricoOcupaciones
                .AsNoTracking()
                .OrderBy(h => h.Momento)
                .ToListAsync();

            if (plazasTotales == 0 || historico.Count == 0)
            {
                _logger.LogWarning("No hay datos suficientes. PlazasTotales={plazas}, HistoricoCount={count}",
                    plazasTotales, historico.Count);

                return new AnalyticsSummaryDto
                {
                    PlazasTotales = plazasTotales,
                    OcupacionPromedioGlobal = 0,
                    MaximoOcupadas = 0,
                    PlazasBloqueablesSugeridas = 0,
                    HorasPico = new List<HoraPicoDto>()
                };
            }

            var ocupPromGlobal = historico.Average(h =>
                h.PlazasTotales == 0 ? 0 : (double)h.PlazasOcupadas / h.PlazasTotales);

            var maxOcupadas = historico.Max(h => h.PlazasOcupadas);

            //Horas pico top 5 por ocupación promedio
            var horasPico = historico
                .GroupBy(h => h.Momento.Hour)
                .Select(g => new HoraPicoDto
                {
                    Hora = g.Key,
                    OcupacionPromedio = g.Average(x =>
                        x.PlazasTotales == 0 ? 0 : (double)x.PlazasOcupadas / x.PlazasTotales)
                })
                .OrderByDescending(x => x.OcupacionPromedio)
                .Take(5)
                .ToList();

            var plazasMinimasParaTol = (int)Math.Ceiling(toleranciaOcupacion * plazasTotales);
            var bloqueables = Math.Max(0, plazasTotales - plazasMinimasParaTol);

            _logger.LogInformation(
                "Resumen: PlazasTotales={plazas}, OcupPromGlobal={prom:0.000}, MaxOcupadas={max}, Bloqueables={blk}",
                plazasTotales, ocupPromGlobal, maxOcupadas, bloqueables);

            return new AnalyticsSummaryDto
            {
                PlazasTotales = plazasTotales,
                OcupacionPromedioGlobal = ocupPromGlobal,
                MaximoOcupadas = maxOcupadas,
                PlazasBloqueablesSugeridas = bloqueables,
                HorasPico = horasPico
            };
        }

        private double NormalizeTolerancia(double tol)
        {
            if (double.IsNaN(tol) || double.IsInfinity(tol) || tol <= 0 || tol > 1)
            {
                _logger.LogWarning("Tolerancia inválida ({tol}). Se usará 0.85.", tol);
                return 0.85;
            }
            return tol;
        }
    }
}
