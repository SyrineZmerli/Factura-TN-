using System.ComponentModel.DataAnnotations;

namespace FacturaPro.Domain.Entities;

public class Client
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? MatriculeFiscale { get; set; }

    [MaxLength(200)]
    public string? Adresse { get; set; }

    [EmailAddress(ErrorMessage = "Email invalide")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Telephone invalide")]
    public string? Telephone { get; set; }

    public bool Actif { get; set; } = true;

    public DateTime DateCreation { get; set; } = DateTime.Now;

    public ICollection<Facture> Factures { get; set; } = new List<Facture>();
}
