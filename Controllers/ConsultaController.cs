using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.CoreMvc.Models;
using System.Linq;

namespace Parking.CoreMvc.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ConsultaController : Controller
    {
        private readonly IDataStore _dataStore;

        public ConsultaController(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ConsultaTicketViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ConsultaTicketViewModel model)
        {
            Ticket? ticket = null;

            if (model.TicketId.HasValue)
            {
                ticket = _dataStore.Tickets
                    .FirstOrDefault(t => t.Id == model.TicketId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(model.Placa))
            {
                ticket = _dataStore.Tickets
                    .OrderByDescending(t => t.HoraEntrada)
                    .FirstOrDefault(t => t.Placa == model.Placa);
            }

            if (ticket == null)
            {
                model.Mensaje = "No se encontró ningún ticket con los datos ingresados.";
            }
            else
            {
                model.Ticket = ticket;
            }

            return View(model);
        }
    }
}
