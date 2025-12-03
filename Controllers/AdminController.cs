using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.CoreMvc.Services;

namespace Parking.CoreMvc.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly IAnalyticsService _analytics;

        public AdminController(IAnalyticsService analytics)
        {
            _analytics = analytics;
        }

        public async Task<IActionResult> Analytics()
        {
            var resumen = await _analytics.GetResumenAsync(0.85); // 85% tolerancia
            return View(resumen);
        }
    }
}
