using System.ComponentModel.DataAnnotations;

namespace LoyaltyPoints.Api.Dtos;

public record CreateCustomerRequest(
    [Required, StringLength(100, MinimumLength = 2)] string Name,
    [Required, EmailAddress, StringLength(200)] string Email);

public record UpdateCustomerRequest(
    [Required, StringLength(100, MinimumLength = 2)] string Name,
    [Required, EmailAddress, StringLength(200)] string Email);

public record CustomerResponse(int Id, string Name, string Email, int PointsBalance, DateTime CreatedAt);