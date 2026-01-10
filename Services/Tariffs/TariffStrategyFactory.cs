namespace Parking.CoreMvc.Services.Tariffs
{
    public class TariffStrategyFactory : ITariffStrategyFactory
    {
        private readonly IEnumerable<ITariffStrategy> _strategies;

        public TariffStrategyFactory(IEnumerable<ITariffStrategy> strategies)
        {
            _strategies = strategies;
        }

        public ITariffStrategy Resolve(string? unidad)
        {
            unidad ??= "Fraccion";
            var found = _strategies.FirstOrDefault(s => string.Equals(s.Unidad, unidad, StringComparison.OrdinalIgnoreCase));
            return found ?? _strategies.First(s => s.Unidad == "Fraccion");
        }
    }
}
