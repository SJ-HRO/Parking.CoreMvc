using Parking.CoreMvc.Models;
using Parking.CoreMvc.Services.Tariffs;

namespace Parking.CoreMvc.Models
{
    public class Tarificador : ITarificador
    {
        private readonly IDataStore _dataStore;
        private readonly ITariffStrategyFactory _factory;

        public Tarificador(IDataStore dataStore, ITariffStrategyFactory factory)
        {
            _dataStore = dataStore;
            _factory = factory;
        }

        public decimal CalcularMonto(Ticket ticket)
        {
            if (!ticket.HoraSalida.HasValue)
                throw new InvalidOperationException("El ticket no tiene hora de salida.");

            var tarifa = _dataStore.Tarifas
                .FirstOrDefault(t => t.TipoVehiculo == ticket.TipoVehiculo)
                ?? _dataStore.Tarifas.First();

            var minutos = (ticket.HoraSalida.Value - ticket.HoraEntrada).TotalMinutes;
            if (minutos <= 0) minutos = 1;

            var strategy = _factory.Resolve(tarifa.Unidad);
            return strategy.Calcular(ticket, tarifa, minutos);
        }
    }
}
