using LoyaltyPoints.Api.Models;

namespace LoyaltyPoints.Api.Dtos;

public record PointsRequest(int Points, string Description);

public record TransactionResponse(int Id, int Points, TransactionType Type, string Description, DateTime CreatedAt);

public record PointsResultResponse(int CustomerId, int PointsBalance, TransactionResponse Transaction);