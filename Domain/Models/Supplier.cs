namespace Domain.Models;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public ICollection<Part> Parts { get; set; } = new List<Part>();
}
