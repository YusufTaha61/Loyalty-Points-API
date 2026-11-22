namespace LoyaltyPoints.Api.Models;

public enum TransactionType
{
    Earn,
    Redeem
}

public class PointTransaction
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int Points { get; set; }
    public TransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}