using System;

namespace Parking.CoreMvc.Models
{
    public class HistoricoOcupacion
    {
        public int Id { get; set; }

        public DateTime Momento { get; set; }

        public int PlazasTotales { get; set; }
        public int PlazasOcupadas { get; set; }
        public int PlazasLibres { get; set; }

        public int TicketsActivos { get; set; }

        public int Anio { get; set; }
        public int Mes { get; set; }
        public int Dia { get; set; }
        public int Hora { get; set; }
        public int DiaSemana { get; set; } // 1=Lunes a 7=Domingo
    }
}
