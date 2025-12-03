namespace Parking.CoreMvc.Models
{
    public class DashboardViewModel
    {
        public int TotalPlazas { get; set; }
        public int Ocupadas { get; set; }
        public int Libres { get; set; }
        public int TicketsActivos { get; set; }
        public int TicketsHoy { get; set; }
    }
}
