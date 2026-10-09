namespace Domain.Models;

public class Part
{
    public int Id { get; set; }
    public string Article { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int SupplierId { get; set; }

    public Supplier? Supplier { get; set; }
    public ICollection<OrderPart> OrderParts { get; set; } = new List<OrderPart>();
}
