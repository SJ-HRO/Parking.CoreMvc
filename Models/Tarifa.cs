namespace Parking.CoreMvc.Models
{
    public class Tarifa
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "General";
        // A cambiar por unidades específicas: Hora o Fracción
        public string Unidad { get; set; } = "Hora";
        public decimal ValorBase { get; set; }
        public int FraccionMinutos { get; set; } = 15;
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
        public bool AplicaFinDeSemana { get; set; } = true;
        public string TipoVehiculo { get; set; } = "Auto";
    }
}
