using System;
using System.Collections.Generic;
using System.Linq;

namespace Parking.CoreMvc.Models
{
    public class InMemoryDataStore : IDataStore
    {
        private readonly List<Ticket> _tickets = new();
        private readonly List<Plaza> _plazas = new();
        private readonly List<Zona> _zonas = new();
        private readonly List<Tarifa> _tarifas = new();
        private int _nextTicketId = 1;

        public InMemoryDataStore()
        {
            //zona ejemplo
            var zonaA = new Zona
            {
                Id = 1,
                Nombre = "Zona A",
                Capacidad = 20
            };
            _zonas.Add(zonaA);

            //plaza ejemplo
            for (int i = 1; i <= 10; i++)
            {
                _plazas.Add(new Plaza
                {
                    Id = i,
                    Codigo = $"A-{i:D2}",
                    ZonaId = zonaA.Id,
                    Estado = EstadoPlaza.Libre,
                    Tipo = "Auto"
                });
            }

            //tarifa ejemplo
            _tarifas.Add(new Tarifa
            {
                Id = 1,
                Nombre = "General Día",
                Unidad = "Fraccion",
                ValorBase = 0.50m,
                FraccionMinutos = 15,
                HoraInicio = new TimeSpan(6, 0, 0),
                HoraFin = new TimeSpan(22, 0, 0),
                AplicaFinDeSemana = true,
                TipoVehiculo = "Auto"
            });

            var ahora = DateTime.Now;

            //ticket abierto
            var plaza1 = _plazas[0];
            plaza1.Estado = EstadoPlaza.Ocupada;
            _tickets.Add(new Ticket
            {
                Id = _nextTicketId++,
                Placa = "ABC1234",
                TipoVehiculo = "Auto",
                HoraEntrada = ahora.AddMinutes(-45),
                Estado = EstadoTicket.Activo,
                PlazaId = plaza1.Id,
                Plaza = plaza1
            });

            //ticket cerrado
            var plaza2 = _plazas[1];
            _tickets.Add(new Ticket
            {
                Id = _nextTicketId++,
                Placa = "XYZ5678",
                TipoVehiculo = "Auto",
                HoraEntrada = ahora.AddHours(-3),
                HoraSalida = ahora.AddHours(-1),
                Estado = EstadoTicket.Cerrado,
                PlazaId = plaza2.Id,
                Plaza = plaza2,
                MontoCalculado = 3.50m
            });

            //espera
            _tickets.Add(new Ticket
            {
                Id = _nextTicketId++,
                Placa = "PSQ9999",
                TipoVehiculo = "Auto",
                HoraEntrada = ahora.AddMinutes(-10),
                Estado = EstadoTicket.EnEspera
            });
        }

        public List<Ticket> Tickets => _tickets;
        public List<Plaza> Plazas => _plazas;
        public List<Zona> Zonas => _zonas;
        public List<Tarifa> Tarifas => _tarifas;

        public Ticket? GetTicket(int id) =>
            _tickets.FirstOrDefault(t => t.Id == id);

        public void AddTicket(Ticket ticket)
        {
            ticket.Id = _nextTicketId++;
            _tickets.Add(ticket);
        }

        public void UpdateTicket(Ticket ticket)
        {
            var existing = GetTicket(ticket.Id);
            if (existing == null) return;

            existing.Placa = ticket.Placa;
            existing.TipoVehiculo = ticket.TipoVehiculo;
            existing.HoraEntrada = ticket.HoraEntrada;
            existing.HoraSalida = ticket.HoraSalida;
            existing.Estado = ticket.Estado;
            existing.PlazaId = ticket.PlazaId;
            existing.Plaza = ticket.Plaza;
            existing.MontoCalculado = ticket.MontoCalculado;
        }
    }
}
