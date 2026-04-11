using FacturaPro.Domain.Entities;

namespace FacturaPro.Application.Interfaces;

public interface IFactureService
{
    Task<List<Facture>> GetAllAsync(DateTime? from = null, DateTime? to = null, int? clientId = null);
    Task<Facture?> GetByIdAsync(int id);
    Task<Facture> CreateAsync(Facture facture);
    Task UpdateAsync(Facture facture);
    Task DeleteAsync(int id);
    Task<string> GenerateNumeroAsync();
    Task ValiderAsync(int id);
    Task AnnulerAsync(int id);
}
