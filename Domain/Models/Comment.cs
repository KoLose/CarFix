namespace Domain.Models;

public class Comment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsFinalClientReview { get; set; }

    public Order? Order { get; set; }
}
