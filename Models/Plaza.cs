namespace Parking.CoreMvc.Models
{
    public enum EstadoPlaza
    {
        Libre,
        Ocupada,
        Mantenimiento
    }

    public class Plaza
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public int ZonaId { get; set; }
        public Zona? Zona { get; set; }
        public string Tipo { get; set; } = "Auto";
        public EstadoPlaza Estado { get; set; } = EstadoPlaza.Libre;
    }
}
