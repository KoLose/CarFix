namespace Domain.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int RoleId { get; set; }

    public Role? Role { get; set; }
    public ICollection<Car> Cars { get; set; } = new List<Car>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
    public ICollection<UserService> UserServices { get; set; } = new List<UserService>();
    public ICollection<PartRequest> PartRequests { get; set; } = new List<PartRequest>();
}
