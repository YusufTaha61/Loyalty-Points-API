using LoyaltyPoints.Api.Data;
using LoyaltyPoints.Api.Dtos;
using LoyaltyPoints.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPoints.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController(AppDbContext db) : ControllerBase
{
    // GET api/customers
    [HttpGet]
    public async Task<ActionResult<List<CustomerResponse>>> GetAll()
    {
        var customers = await db.Customers
            .OrderBy(c => c.Id)
            .Select(c => ToResponse(c))
            .ToListAsync();
        return Ok(customers);
    }

    // GET api/customers/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerResponse>> GetById(int id)
    {
        var customer = await db.Customers.FindAsync(id);
        if (customer is null) return NotFound();
        return Ok(ToResponse(customer));
    }

    // POST api/customers
    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create(CreateCustomerRequest request)
    {
        if (await db.Customers.AnyAsync(c => c.Email == request.Email))
            return Conflict("Bu e-posta ile kayıtlı bir müşteri zaten var.");

        var customer = new Customer { Name = request.Name, Email = request.Email };
        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, ToResponse(customer));
    }

    // PUT api/customers/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CustomerResponse>> Update(int id, UpdateCustomerRequest request)
    {
        var customer = await db.Customers.FindAsync(id);
        if (customer is null) return NotFound();

        if (await db.Customers.AnyAsync(c => c.Email == request.Email && c.Id != id))
            return Conflict("Bu e-posta başka bir müşteride kayıtlı.");

        customer.Name = request.Name;
        customer.Email = request.Email;
        await db.SaveChangesAsync();

        return Ok(ToResponse(customer));
    }

    // DELETE api/customers/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await db.Customers.FindAsync(id);
        if (customer is null) return NotFound();

        db.Customers.Remove(customer);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static CustomerResponse ToResponse(Customer c) =>
        new(c.Id, c.Name, c.Email, c.PointsBalance, c.CreatedAt);
}