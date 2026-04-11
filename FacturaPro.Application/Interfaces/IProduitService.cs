using FacturaPro.Domain.Entities;

namespace FacturaPro.Application.Interfaces;

public interface IProduitService
{
    Task<List<Produit>> GetAllAsync(string? search = null);
    Task<Produit?> GetByIdAsync(int id);
    Task<Produit> CreateAsync(Produit produit);
    Task UpdateAsync(Produit produit);
    Task DeleteAsync(int id);
}
