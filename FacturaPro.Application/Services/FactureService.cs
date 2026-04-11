using FacturaPro.Application.Interfaces;
using FacturaPro.Domain.Entities;
using FacturaPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FacturaPro.Application.Services;

public class FactureService : IFactureService
{
    private readonly AppDbContext _context;

    public FactureService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Facture>> GetAllAsync(DateTime? from = null, DateTime? to = null, int? clientId = null)
    {
        var query = _context.Factures
            .Include(f => f.Client)
            .Include(f => f.Lignes).ThenInclude(l => l.Produit)
            .AsQueryable();

        if (from.HasValue)
            query = query.Where(f => f.DateFacture >= from.Value);
        if (to.HasValue)
            query = query.Where(f => f.DateFacture <= to.Value);
        if (clientId.HasValue)
            query = query.Where(f => f.ClientId == clientId.Value);

        return await query.OrderByDescending(f => f.DateFacture).ToListAsync();
    }

    public async Task<Facture?> GetByIdAsync(int id)
        => await _context.Factures
            .Include(f => f.Client)
            .Include(f => f.Lignes).ThenInclude(l => l.Produit)
            .FirstOrDefaultAsync(f => f.Id == id);

    public async Task<Facture> CreateAsync(Facture facture)
    {
        facture.Numero = await GenerateNumeroAsync();
        _context.Factures.Add(facture);
        await _context.SaveChangesAsync();
        return facture;
    }

    public async Task UpdateAsync(Facture facture)
    {
        var existing = await _context.Factures
            .Include(f => f.Lignes)
            .FirstOrDefaultAsync(f => f.Id == facture.Id);

        if (existing == null) return;

        // Update fields
        existing.ClientId = facture.ClientId;
        existing.DateFacture = facture.DateFacture;
        existing.TimbreFiscalApplique = facture.TimbreFiscalApplique;
        existing.MontantTimbre = facture.MontantTimbre;
        existing.Notes = facture.Notes;

        // Replace lines
        _context.LignesFacture.RemoveRange(existing.Lignes);
        foreach (var ligne in facture.Lignes)
        {
            ligne.FactureId = facture.Id;
            existing.Lignes.Add(ligne);
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var facture = await _context.Factures.Include(f => f.Lignes).FirstOrDefaultAsync(f => f.Id == id);
        if (facture != null)
        {
            _context.Factures.Remove(facture);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string> GenerateNumeroAsync()
    {
        var year = DateTime.Now.Year;
        var count = await _context.Factures.CountAsync(f => f.DateFacture.Year == year);
        return $"FAC-{year}-{(count + 1):D4}";
    }

    public async Task ValiderAsync(int id)
    {
        var facture = await _context.Factures.FindAsync(id);
        if (facture != null)
        {
            facture.Statut = StatutFacture.Validee;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AnnulerAsync(int id)
    {
        var facture = await _context.Factures.FindAsync(id);
        if (facture != null)
        {
            facture.Statut = StatutFacture.Annulee;
            await _context.SaveChangesAsync();
        }
    }
}
