using Parking.CoreMvc.Models;

namespace Parking.CoreMvc.Services
{
    public interface IComparisonService
    {
        Task<List<HistoricoOcupacion>> GetUltimosAsync(int take = 30);
        Task<ComparisonResultDto> CompareHistoricosAsync(int aId, int bId, double umbral);

        Task<ComparisonResultDto> CompareHistoricoVsActualAsync(int historicoId, double umbral);
    }
}
