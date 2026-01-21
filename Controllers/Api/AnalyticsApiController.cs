using Microsoft.AspNetCore.Mvc;
using Parking.CoreMvc.Models;
using Parking.CoreMvc.Services;

namespace Parking.CoreMvc.Controllers.Api
{
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsApiController : ControllerBase
    {
        private readonly IAnalyticsService _analytics;

        public AnalyticsApiController(IAnalyticsService analytics)
        {
            _analytics = analytics;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] double umbral = 0.85)
        {
            if (umbral <= 0 || umbral > 1) umbral = 0.85;

            var vm = await _analytics.GetResumenAsync(umbral);

            var dto = new AnalyticsSummaryApiDto
            {
                PlazasTotales = vm.PlazasTotales,
                OcupacionPromedioGlobal = vm.OcupacionPromedioGlobal,
                MaximoOcupadas = vm.MaximoOcupadas,
                PlazasBloqueablesSugeridas = vm.PlazasBloqueablesSugeridas
            };

            if (vm.HorasPico != null)
            {
                dto.HorasPico = vm.HorasPico.Select(h => new HoraPicoApiDto
                {
                    Hora = h.Hora,
                    OcupacionPromedio = h.OcupacionPromedio
                }).ToList();
            }

            return Ok(dto);
        }
    }
}
