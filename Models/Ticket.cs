using System.ComponentModel.DataAnnotations;

namespace Parking.CoreMvc.Models
{
    public enum EstadoTicket
    {
        EnEspera,
        Activo,
        Cerrado
    }

    public class Ticket
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(10)]
        public string Placa { get; set; } = string.Empty;

        [Required]
        public string TipoVehiculo { get; set; } = "Auto";

        [Display(Name = "Hora de entrada")]
        public DateTime HoraEntrada { get; set; }
        public DateTime? HoraSalida { get; set; }
        public EstadoTicket Estado { get; set; }
        public int? PlazaId { get; set; }
        public Plaza? Plaza { get; set; }

        public TimeSpan? Duracion =>
            HoraSalida.HasValue ? HoraSalida.Value - HoraEntrada : null;

        public decimal? MontoCalculado { get; set; }
    }
}
