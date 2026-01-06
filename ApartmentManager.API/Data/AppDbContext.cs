using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManager.API.Data;

public class ApplicationUser : IdentityUser
{
    public int ApartamentID { get; set; }
    public string? Nume { get; set; }
    public string? Prenume { get; set; }
}

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Apartament> Apartamente { get; set; }
    public DbSet<FacturaIndividuala> FacturiIndividuale { get; set; }

    public DbSet<Sesizare> Sesizari { get; set; }

    public DbSet<CheltuialaAsociatie> CheltuieliAsociatie { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<FacturaIndividuala>().Ignore(f => f.DetaliiAfisare);
        builder.Entity<FacturaIndividuala>().Ignore(f => f.EsteNeplatita);

        builder.Entity<Sesizare>().Ignore(s => s.NumeAutor);
        builder.Entity<Sesizare>().Ignore(s => s.EsteAdmin);

        builder.Entity<Apartament>().Ignore(a => a.DetaliiApartament);
    }
}