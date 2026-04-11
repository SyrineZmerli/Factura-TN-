using System.ComponentModel.DataAnnotations;

namespace FacturaPro.Domain.Entities;

public class Facture
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Numero { get; set; } = string.Empty;

    public DateTime DateFacture { get; set; } = DateTime.Now;

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public ICollection<LigneFacture> Lignes { get; set; } = new List<LigneFacture>();

    public bool TimbreFiscalApplique { get; set; } = true;

    [Range(0, double.MaxValue)]
    public decimal MontantTimbre { get; set; } = 1.000m;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public StatutFacture Statut { get; set; } = StatutFacture.Brouillon;

    // Propriétés calculées
    public decimal TotalHT    => Lignes.Sum(l => l.MontantHT);
    public decimal TotalTVA   => Lignes.Sum(l => l.MontantTVA);
    public decimal TotalTTC   => TotalHT + TotalTVA + (TimbreFiscalApplique ? MontantTimbre : 0);
}

public enum StatutFacture
{
    Brouillon,
    Validee,
    Annulee
}
