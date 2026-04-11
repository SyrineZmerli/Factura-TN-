using FacturaPro.Application.Interfaces;
using FacturaPro.Domain.Entities;
using FacturaPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FacturaPro.Application.Services;

public class ProduitService : IProduitService
{
    private readonly AppDbContext _context;

    public ProduitService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Produit>> GetAllAsync(string? search = null)
    {
        var query = _context.Produits.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(p =>
                p.Designation.ToLower().Contains(search) ||
                (p.Reference != null && p.Reference.ToLower().Contains(search)));
        }

        return await query.OrderBy(p => p.Designation).ToListAsync();
    }

    public async Task<Produit?> GetByIdAsync(int id)
        => await _context.Produits.FindAsync(id);

    public async Task<Produit> CreateAsync(Produit produit)
    {
        _context.Produits.Add(produit);
        await _context.SaveChangesAsync();
        return produit;
    }

    public async Task UpdateAsync(Produit produit)
    {
        _context.Produits.Update(produit);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var produit = await _context.Produits.FindAsync(id);
        if (produit != null)
        {
            _context.Produits.Remove(produit);
            await _context.SaveChangesAsync();
        }
    }
}
