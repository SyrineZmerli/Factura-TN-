using FacturaPro.Application.Interfaces;
using FacturaPro.Domain.Entities;
using FacturaPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FacturaPro.Application.Services;

public class ClientService : IClientService
{
    private readonly AppDbContext _context;

    public ClientService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Client>> GetAllAsync(string? search = null)
    {
        var query = _context.Clients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(c =>
                c.Nom.ToLower().Contains(search) ||
                (c.MatriculeFiscale != null && c.MatriculeFiscale.ToLower().Contains(search)) ||
                (c.Email != null && c.Email.ToLower().Contains(search)));
        }

        return await query.OrderBy(c => c.Nom).ToListAsync();
    }

    public async Task<Client?> GetByIdAsync(int id)
        => await _context.Clients.FindAsync(id);

    public async Task<Client> CreateAsync(Client client)
    {
        client.DateCreation = DateTime.Now;
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return client;
    }

    public async Task UpdateAsync(Client client)
    {
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client != null)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Clients.AnyAsync(c => c.Id == id);
}
