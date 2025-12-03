using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.CoreMvc.Models;
using System;
using System.Linq;

namespace Parking.CoreMvc.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IDataStore _dataStore;

        public HomeController(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalPlazas = _dataStore.Plazas.Count,
                Ocupadas = _dataStore.Plazas.Count(p => p.Estado == EstadoPlaza.Ocupada),
                Libres = _dataStore.Plazas.Count(p => p.Estado == EstadoPlaza.Libre),
                TicketsActivos = _dataStore.Tickets.Count(t => t.Estado == EstadoTicket.Activo),
                TicketsHoy = _dataStore.Tickets.Count(t => t.HoraEntrada.Date == DateTime.Today)
            };

            return View(model);
        }
    }
}
