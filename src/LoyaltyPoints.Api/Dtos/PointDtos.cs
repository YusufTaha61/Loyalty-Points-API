using System.ComponentModel.DataAnnotations;
using LoyaltyPoints.Api.Models;

namespace LoyaltyPoints.Api.Dtos;

public record PointsRequest(
    [Range(1, 100_000)] int Points,
    [StringLength(200)] string? Description);

public record TransactionResponse(int Id, int Points, TransactionType Type, string Description, DateTime CreatedAt);

public record PointsResultResponse(int CustomerId, int PointsBalance, TransactionResponse Transaction);