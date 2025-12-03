namespace Parking.CoreMvc.Models
{
    public interface ITarificador
    {
        decimal CalcularMonto(Ticket ticket);
    }
}
