using System.ComponentModel.DataAnnotations;
using FacturaPro.Domain.Enums;

namespace FacturaPro.Domain.Entities;

public class Produit
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La designation est obligatoire")]
    [MaxLength(150)]
    public string Designation { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Reference { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Le prix doit etre positif")]
    public decimal PrixUnitaireHT { get; set; }

    public TauxTVA TauxTVA { get; set; } = TauxTVA.Pct19;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool Actif { get; set; } = true;

    public ICollection<LigneFacture> LignesFacture { get; set; } = new List<LigneFacture>();

    // Propriété calculée
    public decimal TauxTVAValeur => (decimal)TauxTVA;
}
