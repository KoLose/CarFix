namespace Domain.Models;

public class OrderPart
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int PartId { get; set; }
    public int Quantity { get; set; }

    public Order? Order { get; set; }
    public Part? Part { get; set; }
}
