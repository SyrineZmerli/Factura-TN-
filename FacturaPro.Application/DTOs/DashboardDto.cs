namespace FacturaPro.Application.DTOs;

public class DashboardFiscalDto
{
    public decimal TVATotaleCollectee { get; set; }
    public Dictionary<string, decimal> TVAParTaux { get; set; } = new();
    public decimal MontantTotalTimbre { get; set; }
    public int NombreFactures { get; set; }
}

public class DashboardVentesDto
{
    public decimal ChiffreAffairesHT { get; set; }
    public decimal ChiffreAffairesTTC { get; set; }
    public List<VentesParPeriodeDto> VentesParMois { get; set; } = new();
    public List<VentesParClientDto> VentesParClient { get; set; } = new();
    public List<VentesParProduitDto> VentesParProduit { get; set; } = new();
}

public class VentesParPeriodeDto
{
    public string Periode { get; set; } = string.Empty;
    public decimal MontantHT { get; set; }
    public decimal MontantTTC { get; set; }
}

public class VentesParClientDto
{
    public string ClientNom { get; set; } = string.Empty;
    public decimal MontantHT { get; set; }
    public decimal MontantTTC { get; set; }
}

public class VentesParProduitDto
{
    public string ProduitDesignation { get; set; } = string.Empty;
    public decimal MontantHT { get; set; }
    public int QuantiteVendue { get; set; }
}
