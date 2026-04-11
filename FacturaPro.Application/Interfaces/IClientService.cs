using FacturaPro.Domain.Entities;

namespace FacturaPro.Application.Interfaces;

public interface IClientService
{
    Task<List<Client>> GetAllAsync(string? search = null);
    Task<Client?> GetByIdAsync(int id);
    Task<Client> CreateAsync(Client client);
    Task UpdateAsync(Client client);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
