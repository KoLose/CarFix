namespace Domain.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateFinished { get; set; }
    public int StatusId { get; set; }
    public int AutomobileId { get; set; }
    public int MechanicId { get; set; }

    public Status? Status { get; set; }
    public Car? Automobile { get; set; }
    public User? Mechanic { get; set; }
    public ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();
    public ICollection<OrderPart> OrderParts { get; set; } = new List<OrderPart>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
