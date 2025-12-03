using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Parking.CoreMvc.Data;

namespace Parking.CoreMvc.Models
{
    public class EfDataStore : IDataStore
    {
        private readonly ApplicationDbContext _db;

        public EfDataStore(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Ticket> Tickets =>
            _db.Tickets
               .Include(t => t.Plaza)
               .ToList();

        public List<Plaza> Plazas =>
            _db.Plazas
               .Include(p => p.Zona)
               .ToList();

        public List<Zona> Zonas =>
            _db.Zonas.ToList();

        public List<Tarifa> Tarifas =>
            _db.Tarifas.ToList();

        public Ticket? GetTicket(int id) =>
            _db.Tickets
               .Include(t => t.Plaza)
               .FirstOrDefault(t => t.Id == id);

        public void AddTicket(Ticket ticket)
        {
            _db.Tickets.Add(ticket);
            _db.SaveChanges();
        }

        public void UpdateTicket(Ticket ticket)
        {
            _db.Tickets.Update(ticket);
            _db.SaveChanges();
        }
    }
}
