using FacturaPro.Application.DTOs;
using FacturaPro.Application.Interfaces;
using FacturaPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FacturaPro.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardFiscalDto> GetFiscalDashboardAsync(int? year = null)
    {
        var annee = year ?? DateTime.Now.Year;

        var factures = await _context.Factures
            .Include(f => f.Lignes)
            .Where(f => f.DateFacture.Year == annee && f.Statut != Domain.Entities.StatutFacture.Annulee)
            .ToListAsync();

        var tvaParTaux = factures
            .SelectMany(f => f.Lignes)
            .GroupBy(l => l.TauxTVAApplique)
            .ToDictionary(
                g => $"TVA {g.Key}%",
                g => g.Sum(l => l.MontantTVA)
            );

        return new DashboardFiscalDto
        {
            TVATotaleCollectee  = factures.SelectMany(f => f.Lignes).Sum(l => l.MontantTVA),
            TVAParTaux          = tvaParTaux,
            MontantTotalTimbre  = factures.Where(f => f.TimbreFiscalApplique).Sum(f => f.MontantTimbre),
            NombreFactures      = factures.Count
        };
    }

    public async Task<DashboardVentesDto> GetVentesDashboardAsync(int? year = null)
    {
        var annee = year ?? DateTime.Now.Year;

        var factures = await _context.Factures
            .Include(f => f.Client)
            .Include(f => f.Lignes).ThenInclude(l => l.Produit)
            .Where(f => f.DateFacture.Year == annee && f.Statut != Domain.Entities.StatutFacture.Annulee)
            .ToListAsync();

        // Ventes par mois
        var ventesParMois = factures
            .GroupBy(f => f.DateFacture.Month)
            .OrderBy(g => g.Key)
            .Select(g => new VentesParPeriodeDto
            {
                Periode    = new DateTime(annee, g.Key, 1).ToString("MMM yyyy"),
                MontantHT  = g.Sum(f => f.TotalHT),
                MontantTTC = g.Sum(f => f.TotalTTC)
            }).ToList();

        // Ventes par client
        var ventesParClient = factures
            .GroupBy(f => f.Client?.Nom ?? "Inconnu")
            .OrderByDescending(g => g.Sum(f => f.TotalHT))
            .Take(10)
            .Select(g => new VentesParClientDto
            {
                ClientNom  = g.Key,
                MontantHT  = g.Sum(f => f.TotalHT),
                MontantTTC = g.Sum(f => f.TotalTTC)
            }).ToList();

        // Ventes par produit
        var ventesParProduit = factures
            .SelectMany(f => f.Lignes)
            .GroupBy(l => l.Produit?.Designation ?? "Inconnu")
            .OrderByDescending(g => g.Sum(l => l.MontantHT))
            .Take(10)
            .Select(g => new VentesParProduitDto
            {
                ProduitDesignation = g.Key,
                MontantHT          = g.Sum(l => l.MontantHT),
                QuantiteVendue     = g.Sum(l => l.Quantite)
            }).ToList();

        return new DashboardVentesDto
        {
            ChiffreAffairesHT  = factures.Sum(f => f.TotalHT),
            ChiffreAffairesTTC = factures.Sum(f => f.TotalTTC),
            VentesParMois      = ventesParMois,
            VentesParClient    = ventesParClient,
            VentesParProduit   = ventesParProduit
        };
    }
}
