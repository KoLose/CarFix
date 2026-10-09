namespace Domain.Models;

public class Car
{
    public int Id { get; set; }
    public string VIN { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public int Mileage { get; set; }
    public int ClientId { get; set; }

    public User? Client { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
