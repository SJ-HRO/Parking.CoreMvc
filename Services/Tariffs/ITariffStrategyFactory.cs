namespace Parking.CoreMvc.Services.Tariffs
{
    public interface ITariffStrategyFactory
    {
        ITariffStrategy Resolve(string? unidad);
    }
}
