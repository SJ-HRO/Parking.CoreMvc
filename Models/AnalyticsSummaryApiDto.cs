namespace Parking.CoreMvc.Models
{
    public class AnalyticsSummaryApiDto
    {
        public int PlazasTotales { get; set; }
        public double OcupacionPromedioGlobal { get; set; } 
        public int MaximoOcupadas { get; set; }
        public int PlazasBloqueablesSugeridas { get; set; }

        public List<HoraPicoApiDto> HorasPico { get; set; } = new();
    }

    public class HoraPicoApiDto
    {
        public int Hora { get; set; } 
        public double OcupacionPromedio { get; set; } 
    }
}
