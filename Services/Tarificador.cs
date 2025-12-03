using System;

namespace Parking.CoreMvc.Models
{
    public class Tarificador : ITarificador
    {
        private readonly IDataStore _dataStore;

        public Tarificador(IDataStore dataStore)
        {
            _dataStore = dataStore;
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

            if (tarifa.Unidad == "Fraccion")
            {
                var fracciones = Math.Ceiling(minutos / tarifa.FraccionMinutos);
                return (decimal)fracciones * tarifa.ValorBase;
            }

            var horas = Math.Ceiling(minutos / 60.0);
            return (decimal)horas * tarifa.ValorBase;
        }
    }
}
