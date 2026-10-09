namespace Domain.Models;

public class OrderService
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ServiceId { get; set; }
    public decimal ActualPrice { get; set; }
    public bool IsCompleted { get; set; }

    public Order? Order { get; set; }
    public Service? Service { get; set; }
}
