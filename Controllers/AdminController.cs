using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.CoreMvc.Services;
using Parking.CoreMvc.Models;

namespace Parking.CoreMvc.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly IAnalyticsService _analytics;
        private readonly IComparisonService _comparison;

        public AdminController(IAnalyticsService analytics, IComparisonService comparison)
        {
            _analytics = analytics;
            _comparison = comparison;
        }
        private static double NormalizeUmbral(double umbral)
        {
            if (double.IsNaN(umbral) || double.IsInfinity(umbral)) return 0.85;
            if (umbral <= 0 || umbral > 1) return 0.85;
            return umbral;
        }

        public async Task<IActionResult> Analytics()
        {
            var resumen = await _analytics.GetResumenAsync(0.85);
            return View(resumen);
        }

        // ===== Compare: Histórico vs Histórico =====
        [HttpGet]
        public async Task<IActionResult> Compare(int? aId, int? bId, double umbral = 0.85)
        {
            umbral = NormalizeUmbral(umbral);

            var vm = new ComparisonPageViewModel
            {
                Ultimos = await _comparison.GetUltimosAsync(40),
                AId = aId,
                BId = bId,
                Umbral = umbral
            };

            if (aId.HasValue && bId.HasValue)
            {
                if (aId.Value == bId.Value)
                {
                    vm.Error = "Selecciona dos snapshots distintos.";
                    return View(vm);
                }

                try
                {
                    vm.Result = await _comparison.CompareHistoricosAsync(aId.Value, bId.Value, umbral);
                }
                catch (Exception ex)
                {
                    vm.Error = ex.Message;
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Compare(ComparisonPageViewModel vm)
        {
            vm.Umbral = NormalizeUmbral(vm.Umbral);
            vm.Ultimos = await _comparison.GetUltimosAsync(40);

            if (!vm.AId.HasValue || !vm.BId.HasValue)
            {
                vm.Error = "Selecciona A y B.";
                return View(vm);
            }

            if (vm.AId.Value == vm.BId.Value)
            {
                vm.Error = "Selecciona dos snapshots distintos.";
                return View(vm);
            }

            try
            {
                vm.Result = await _comparison.CompareHistoricosAsync(vm.AId.Value, vm.BId.Value, vm.Umbral);
            }
            catch (Exception ex)
            {
                vm.Error = ex.Message;
            }

            return View(vm);
        }

        // ===== CompareLive: Histórico vs Actual (dos tablas) =====
        [HttpGet]
        public async Task<IActionResult> CompareLive(int? historicoId, double umbral = 0.85)
        {
            umbral = NormalizeUmbral(umbral);

            var vm = new ComparisonPageViewModel
            {
                Ultimos = await _comparison.GetUltimosAsync(40),
                AId = historicoId,
                Umbral = umbral
            };

            if (historicoId.HasValue)
            {
                if (historicoId.Value <= 0)
                {
                    vm.Error = "Snapshot inválido.";
                    return View(vm);
                }

                try
                {
                    vm.Result = await _comparison.CompareHistoricoVsActualAsync(historicoId.Value, umbral);
                }
                catch (Exception ex)
                {
                    vm.Error = ex.Message;
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompareLive(ComparisonPageViewModel vm)
        {
            vm.Umbral = NormalizeUmbral(vm.Umbral);
            vm.Ultimos = await _comparison.GetUltimosAsync(40);

            if (!vm.AId.HasValue)
            {
                vm.Error = "Selecciona un snapshot histórico (A).";
                return View(vm);
            }

            if (vm.AId.Value <= 0)
            {
                vm.Error = "Snapshot inválido.";
                return View(vm);
            }

            try
            {
                vm.Result = await _comparison.CompareHistoricoVsActualAsync(vm.AId.Value, vm.Umbral);
            }
            catch (Exception ex)
            {
                vm.Error = ex.Message;
            }

            return View(vm);
        }
    }
}
