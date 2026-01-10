using Parking.CoreMvc.Models;

namespace Parking.CoreMvc.Services.Tariffs
{
    public class HourlyTariffStrategy : ITariffStrategy
    {
        public string Unidad => "Hora";

        public decimal Calcular(Ticket ticket, Tarifa tarifa, double minutos)
        {
            var horas = Math.Ceiling(minutos / 60.0);
            return (decimal)horas * tarifa.ValorBase;
        }
    }
}
