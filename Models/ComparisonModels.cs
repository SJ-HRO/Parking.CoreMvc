namespace Parking.CoreMvc.Models
{
    public class SnapshotDto
    {
        public int Id { get; set; }
        public DateTime Momento { get; set; }
        public int PlazasTotales { get; set; }
        public int PlazasOcupadas { get; set; }
        public int PlazasLibres { get; set; }
        public int TicketsActivos { get; set; }
        public double OcupacionPct => PlazasTotales <= 0 ? 0 : (double)PlazasOcupadas / PlazasTotales;
    }

    public class ComparisonResultDto
    {
        public SnapshotDto A { get; set; } = new();
        public SnapshotDto B { get; set; } = new();

        public int DeltaOcupadas => B.PlazasOcupadas - A.PlazasOcupadas;
        public int DeltaLibres => B.PlazasLibres - A.PlazasLibres;
        public int DeltaTicketsActivos => B.TicketsActivos - A.TicketsActivos;
        public double DeltaOcupacionPct => B.OcupacionPct - A.OcupacionPct;

        public double Umbral { get; set; }
        public bool BExcedeUmbral => B.OcupacionPct >= Umbral;
    }

    public class ComparisonPageViewModel
    {
        public List<HistoricoOcupacion> Ultimos { get; set; } = new();
        public int? AId { get; set; }
        public int? BId { get; set; }
        public double Umbral { get; set; } = 0.85;

        public ComparisonResultDto? Result { get; set; }
        public string? Error { get; set; }
    }
}
