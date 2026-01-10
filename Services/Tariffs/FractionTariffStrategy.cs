using Parking.CoreMvc.Models;

namespace Parking.CoreMvc.Services.Tariffs
{
    public class FractionTariffStrategy : ITariffStrategy
    {
        public string Unidad => "Fraccion";

        public decimal Calcular(Ticket ticket, Tarifa tarifa, double minutos)
        {
            var fraccion = tarifa.FraccionMinutos <= 0 ? 15 : tarifa.FraccionMinutos;
            var fracciones = Math.Ceiling(minutos / fraccion);
            return (decimal)fracciones * tarifa.ValorBase;
        }
    }
}
