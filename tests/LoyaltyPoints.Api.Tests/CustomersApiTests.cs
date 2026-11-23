using System.Net;
using System.Net.Http.Json;
using LoyaltyPoints.Api.Dtos;

namespace LoyaltyPoints.Api.Tests;

public class CustomersApiTests(LoyaltyApiFactory factory) : IClassFixture<LoyaltyApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateCustomer_ReturnsCreated_WithZeroBalance()
    {
        var response = await _client.PostAsJsonAsync("/api/customers",
            new CreateCustomerRequest("Ayse Yilmaz", LoyaltyApiFactory.NewEmail()));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var customer = await LoyaltyApiFactory.ReadAsync<CustomerResponse>(response);
        Assert.Equal("Ayse Yilmaz", customer.Name);
        Assert.Equal(0, customer.PointsBalance);
    }

    [Fact]
    public async Task CreateCustomer_WithInvalidEmail_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/customers",
            new CreateCustomerRequest("Ayse Yilmaz", "gecersiz-eposta"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCustomer_WithDuplicateEmail_ReturnsConflict()
    {
        var email = LoyaltyApiFactory.NewEmail();
        await _client.PostAsJsonAsync("/api/customers", new CreateCustomerRequest("Ali", email));

        var response = await _client.PostAsJsonAsync("/api/customers",
            new CreateCustomerRequest("Veli", email));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetCustomer_WhenNotFound_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/customers/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}