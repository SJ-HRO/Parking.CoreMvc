using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parking.CoreMvc.Data;
using Parking.CoreMvc.Models;

namespace Parking.CoreMvc.Services
{
    public class HoraPicoDto
    {
        public int Hora { get; set; }
        public double OcupacionPromedio { get; set; } // 0-1
    }

    public class AnalyticsSummaryDto
    {
        public double OcupacionPromedioGlobal { get; set; } // 0-1
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

        public AnalyticsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<AnalyticsSummaryDto> GetResumenAsync(double toleranciaOcupacion = 0.85)
        {
            var registros = await _db.HistoricoOcupaciones.ToListAsync();

            if (!registros.Any())
            {
                return new AnalyticsSummaryDto();
            }

            var plazasTotales = registros.Max(r => r.PlazasTotales);
            var promedioOcupacionGlobal = registros.Average(r => (double)r.PlazasOcupadas / plazasTotales);
            var maxOcupadas = registros.Max(r => r.PlazasOcupadas);
            var maxOcupacionRelativa = (double)maxOcupadas / plazasTotales;
            int plazasBloqueables = 0;

            if (maxOcupacionRelativa < toleranciaOcupacion)
            {
                var plazasPermitidas = (int)Math.Round(plazasTotales * toleranciaOcupacion);
                plazasBloqueables = plazasTotales - plazasPermitidas;
                if (plazasBloqueables < 0) plazasBloqueables = 0;
            }
            var horasPico = registros
                .GroupBy(r => r.Hora)
                .Select(g => new HoraPicoDto
                {
                    Hora = g.Key,
                    OcupacionPromedio = g.Average(r => (double)r.PlazasOcupadas / plazasTotales)
                })
                .OrderByDescending(h => h.OcupacionPromedio)
                .Take(5)
                .ToList();

            return new AnalyticsSummaryDto
            {
                PlazasTotales = plazasTotales,
                OcupacionPromedioGlobal = promedioOcupacionGlobal,
                MaximoOcupadas = maxOcupadas,
                PlazasBloqueablesSugeridas = plazasBloqueables,
                HorasPico = horasPico
            };
        }
    }
}
