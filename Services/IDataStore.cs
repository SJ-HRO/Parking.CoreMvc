using System.Collections.Generic;

namespace Parking.CoreMvc.Models
{
    public interface IDataStore
    {
        List<Ticket> Tickets { get; }
        List<Plaza> Plazas { get; }
        List<Zona> Zonas { get; }
        List<Tarifa> Tarifas { get; }

        Ticket? GetTicket(int id);
        void AddTicket(Ticket ticket);
        void UpdateTicket(Ticket ticket);
    }
}
