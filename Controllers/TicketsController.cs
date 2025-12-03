using Microsoft.AspNetCore.Mvc;
using Parking.CoreMvc.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Parking.CoreMvc.Controllers
{
    [Authorize(Roles = "Administrador,Operador")]
    public class TicketsController : Controller
    {
        private readonly IDataStore _dataStore;
        private readonly IAsignadorPlazas _asignadorPlazas;
        private readonly ITarificador _tarificador;

        public TicketsController(
            IDataStore dataStore,
            IAsignadorPlazas asignadorPlazas,
            ITarificador tarificador)
        {
            _dataStore = dataStore;
            _asignadorPlazas = asignadorPlazas;
            _tarificador = tarificador;
        }

        public IActionResult Index()
        {
            var tickets = _dataStore.Tickets
                .OrderByDescending(t => t.HoraEntrada)
                .ToList();

            return View(tickets);
        }
        public IActionResult EnEspera()
        {
            var enEspera = _dataStore.Tickets
                .Where(t => t.Estado == EstadoTicket.EnEspera)
                .OrderBy(t => t.HoraEntrada)
                .ToList();

            return View(enEspera);
        }

        public IActionResult Reasignar(int id)
        {
            var ticket = _dataStore.GetTicket(id);
            if (ticket == null) return NotFound();

            if (ticket.Estado != EstadoTicket.EnEspera)
                return RedirectToAction(nameof(Details), new { id });

            var plaza = _asignadorPlazas.AsignarPlaza(ticket.TipoVehiculo);
            if (plaza == null)
            {
                TempData["Mensaje"] = "No hay plazas disponibles en este momento.";
                return RedirectToAction(nameof(EnEspera));
            }

            ticket.PlazaId = plaza.Id;
            ticket.Plaza = plaza;
            ticket.Estado = EstadoTicket.Activo;
            _dataStore.UpdateTicket(ticket);

            return RedirectToAction(nameof(Details), new { id = ticket.Id });
        }

        public IActionResult Create()
        {
            var ticket = new Ticket
            {
                HoraEntrada = DateTime.Now
            };
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var plaza = _asignadorPlazas.AsignarPlaza(model.TipoVehiculo);

            if (plaza == null)
            {
                model.Estado = EstadoTicket.EnEspera;
                model.PlazaId = null;
            }
            else
            {
                model.Estado = EstadoTicket.Activo;
                model.PlazaId = plaza.Id;
                model.Plaza = plaza;
            }

            model.HoraEntrada = DateTime.Now;
            _dataStore.AddTicket(model);

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        public IActionResult Details(int id)
        {
            var ticket = _dataStore.GetTicket(id);
            if (ticket == null) return NotFound();
            return View(ticket);
        }

        public IActionResult Close(int id)
        {
            var ticket = _dataStore.GetTicket(id);
            if (ticket == null) return NotFound();

            ticket.HoraSalida = DateTime.Now;
            ticket.MontoCalculado = _tarificador.CalcularMonto(ticket);

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CloseConfirmed(int id)
        {
            var ticket = _dataStore.GetTicket(id);
            if (ticket == null) return NotFound();

            ticket.HoraSalida ??= DateTime.Now;
            ticket.MontoCalculado ??= _tarificador.CalcularMonto(ticket);
            ticket.Estado = EstadoTicket.Cerrado;

            if (ticket.PlazaId.HasValue)
            {
                _asignadorPlazas.LiberarPlaza(ticket.PlazaId.Value);
            }

            _dataStore.UpdateTicket(ticket);

            return RedirectToAction(nameof(Details), new { id = ticket.Id });
        }
    }
}
