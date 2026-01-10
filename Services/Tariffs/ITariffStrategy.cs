using Parking.CoreMvc.Models;

namespace Parking.CoreMvc.Services.Tariffs
{
    public interface ITariffStrategy
    {
        string Unidad { get; }
        decimal Calcular(Ticket ticket, Tarifa tarifa, double minutos);
    }
}
