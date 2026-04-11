using System.ComponentModel.DataAnnotations;

namespace FacturaPro.Domain.Entities;

public class LigneFacture
{
    public int Id { get; set; }

    public int FactureId { get; set; }
    public Facture Facture { get; set; } = null!;

    public int ProduitId { get; set; }
    public Produit Produit { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "La quantite doit être au moins 1")]
    public int Quantite { get; set; } = 1;

    public decimal PrixUnitaireHT { get; set; }

    public decimal TauxTVAApplique { get; set; } //valeur f tounes bl %  (0, 7, 13, 19)

    // Propriétés calculées
    public decimal MontantHT => Quantite * PrixUnitaireHT;
    public decimal MontantTVA => MontantHT * TauxTVAApplique / 100;
    public decimal MontantTTC => MontantHT + MontantTVA;
}
