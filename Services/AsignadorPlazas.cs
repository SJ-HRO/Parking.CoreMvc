using System.Linq;

namespace Parking.CoreMvc.Models
{
    public class AsignadorPlazas : IAsignadorPlazas
    {
        private readonly IDataStore _dataStore;

        public AsignadorPlazas(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public Plaza? AsignarPlaza(string tipoVehiculo)
        {
            var plaza = _dataStore.Plazas
                .FirstOrDefault(p => p.Estado == EstadoPlaza.Libre && p.Tipo == tipoVehiculo);

            if (plaza != null)
            {
                plaza.Estado = EstadoPlaza.Ocupada;
            }

            return plaza;
        }

        public void LiberarPlaza(int plazaId)
        {
            var plaza = _dataStore.Plazas.FirstOrDefault(p => p.Id == plazaId);
            if (plaza != null)
            {
                plaza.Estado = EstadoPlaza.Libre;
            }
        }
    }
}
