namespace Domain.Models;

public class Shift
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DayOfWeek { get; set; } = string.Empty;
    public int? UserId { get; set; }

    public User? User { get; set; }
}
