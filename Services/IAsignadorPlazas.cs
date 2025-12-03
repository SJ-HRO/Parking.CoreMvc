using Parking.CoreMvc.Models;

namespace Parking.CoreMvc.Models
{
    public interface IAsignadorPlazas
    {
        Plaza? AsignarPlaza(string tipoVehiculo);
        void LiberarPlaza(int plazaId);
    }
}
