using LoyaltyPoints.Api.Data;
using LoyaltyPoints.Api.Dtos;
using LoyaltyPoints.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPoints.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:int}/points")]
public class PointsController(AppDbContext db) : ControllerBase
{
    // POST api/customers/5/points/earn
    [HttpPost("earn")]
    public async Task<ActionResult<PointsResultResponse>> Earn(int customerId, PointsRequest request)
    {
        if (request.Points <= 0)
            return BadRequest("Puan 0'dan büyük olmalı.");

        var customer = await db.Customers.FindAsync(customerId);
        if (customer is null) return NotFound();

        customer.PointsBalance += request.Points;
        var transaction = AddTransaction(customerId, request, TransactionType.Earn);
        await db.SaveChangesAsync();

        return Ok(new PointsResultResponse(customer.Id, customer.PointsBalance, ToResponse(transaction)));
    }

    // POST api/customers/5/points/redeem
    [HttpPost("redeem")]
    public async Task<ActionResult<PointsResultResponse>> Redeem(int customerId, PointsRequest request)
    {
        if (request.Points <= 0)
            return BadRequest("Puan 0'dan büyük olmalı.");

        var customer = await db.Customers.FindAsync(customerId);
        if (customer is null) return NotFound();

        if (customer.PointsBalance < request.Points)
            return BadRequest($"Yetersiz puan. Mevcut bakiye: {customer.PointsBalance}");

        customer.PointsBalance -= request.Points;
        var transaction = AddTransaction(customerId, request, TransactionType.Redeem);
        await db.SaveChangesAsync();

        return Ok(new PointsResultResponse(customer.Id, customer.PointsBalance, ToResponse(transaction)));
    }

    // GET api/customers/5/points/transactions
    [HttpGet("transactions")]
    public async Task<ActionResult<List<TransactionResponse>>> GetTransactions(int customerId)
    {
        if (!await db.Customers.AnyAsync(c => c.Id == customerId))
            return NotFound();

        var transactions = await db.PointTransactions
            .Where(t => t.CustomerId == customerId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => ToResponse(t))
            .ToListAsync();

        return Ok(transactions);
    }

    private PointTransaction AddTransaction(int customerId, PointsRequest request, TransactionType type)
    {
        var transaction = new PointTransaction
        {
            CustomerId = customerId,
            Points = request.Points,
            Type = type,
            Description = request.Description
        };
        db.PointTransactions.Add(transaction);
        return transaction;
    }

    private static TransactionResponse ToResponse(PointTransaction t) =>
        new(t.Id, t.Points, t.Type, t.Description, t.CreatedAt);
}