namespace Parking.CoreMvc.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Rol { get; set; } = "Operador";
        public string Username { get; set; } = string.Empty;

    }
}
