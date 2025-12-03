using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Parking.CoreMvc.Models;

namespace Parking.CoreMvc.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Ticket> Tickets { get; set; } = null!;
        public DbSet<Plaza> Plazas { get; set; } = null!;
        public DbSet<Zona> Zonas { get; set; } = null!;
        public DbSet<Tarifa> Tarifas { get; set; } = null!;
        public DbSet<HistoricoOcupacion> HistoricoOcupaciones { get; set; } = null!;
    }
}
