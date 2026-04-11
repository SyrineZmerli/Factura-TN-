using FacturaPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacturaPro.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client>       Clients       { get; set; }
    public DbSet<Produit>      Produits      { get; set; }
    public DbSet<Facture>      Factures      { get; set; }
    public DbSet<LigneFacture> LignesFacture { get; set; }
    public DbSet<ParametreApp> Parametres    { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Client
        modelBuilder.Entity<Client>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Nom).IsRequired().HasMaxLength(100);
            e.Property(c => c.MatriculeFiscale).HasMaxLength(50);
        });

        // Produit
        modelBuilder.Entity<Produit>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Designation).IsRequired().HasMaxLength(150);
            e.Property(p => p.PrixUnitaireHT).HasColumnType("decimal(18,3)");
        });

        // Facture
        modelBuilder.Entity<Facture>(e =>
        {
            e.HasKey(f => f.Id);
            e.Property(f => f.Numero).IsRequired().HasMaxLength(20);
            e.Property(f => f.MontantTimbre).HasColumnType("decimal(18,3)");
            e.Ignore(f => f.TotalHT);
            e.Ignore(f => f.TotalTVA);
            e.Ignore(f => f.TotalTTC);
            e.HasOne(f => f.Client)
             .WithMany(c => c.Factures)
             .HasForeignKey(f => f.ClientId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // LigneFacture
        modelBuilder.Entity<LigneFacture>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.PrixUnitaireHT).HasColumnType("decimal(18,3)");
            e.Property(l => l.TauxTVAApplique).HasColumnType("decimal(5,2)");
            e.Ignore(l => l.MontantHT);
            e.Ignore(l => l.MontantTVA);
            e.Ignore(l => l.MontantTTC);
            e.HasOne(l => l.Produit)
             .WithMany(p => p.LignesFacture)
             .HasForeignKey(l => l.ProduitId)
             .OnDelete(DeleteBehavior.Restrict);
        });
 
        // Seed data
        modelBuilder.Entity<ParametreApp>().HasData(
            new ParametreApp { Id = 1, Cle = "TimbreFiscal", Valeur = "1.000", Description = "Montant du timbre fiscal en TND" },
            new ParametreApp { Id = 2, Cle = "RaisonSociale", Valeur = "Mon Entreprise SARL", Description = "Raison sociale de la société" },
            new ParametreApp { Id = 3, Cle = "MatriculeFiscale", Valeur = "0000000/A/M/000", Description = "Matricule fiscal de la société" }
        );
    }
}
