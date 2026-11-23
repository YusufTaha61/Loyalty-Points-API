namespace LoyaltyPoints.Api.Dtos;

public record CreateCustomerRequest(string Name, string Email);

public record UpdateCustomerRequest(string Name, string Email);

public record CustomerResponse(int Id, string Name, string Email, int PointsBalance, DateTime CreatedAt);