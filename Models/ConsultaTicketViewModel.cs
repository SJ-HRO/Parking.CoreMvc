namespace Parking.CoreMvc.Models
{
    public class ConsultaTicketViewModel
    {
        public string? Placa { get; set; }
        public int? TicketId { get; set; }

        public Ticket? Ticket { get; set; }
        public string? Mensaje { get; set; }
    }
}
