namespace FacturaPro.Domain.Entities;

public class ParametreApp
{
    public int Id { get; set; }
    public string Cle { get; set; } = string.Empty;
    public string Valeur { get; set; } = string.Empty;
    public string? Description { get; set; }
}
