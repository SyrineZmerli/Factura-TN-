using FacturaPro.Application.DTOs;

namespace FacturaPro.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardFiscalDto> GetFiscalDashboardAsync(int? year = null);
    Task<DashboardVentesDto> GetVentesDashboardAsync(int? year = null);
}
