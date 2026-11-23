using System.Net;
using System.Net.Http.Json;
using LoyaltyPoints.Api.Dtos;
using LoyaltyPoints.Api.Models;

namespace LoyaltyPoints.Api.Tests;

public class PointsApiTests(LoyaltyApiFactory factory) : IClassFixture<LoyaltyApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task<int> CreateCustomerAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/customers",
            new CreateCustomerRequest("Test Musteri", LoyaltyApiFactory.NewEmail()));
        var customer = await LoyaltyApiFactory.ReadAsync<CustomerResponse>(response);
        return customer.Id;
    }

    private Task<HttpResponseMessage> EarnAsync(int customerId, int points) =>
        _client.PostAsJsonAsync($"/api/customers/{customerId}/points/earn",
            new PointsRequest(points, "Alisveris"));

    private Task<HttpResponseMessage> RedeemAsync(int customerId, int points) =>
        _client.PostAsJsonAsync($"/api/customers/{customerId}/points/redeem",
            new PointsRequest(points, "Indirim"));

    [Fact]
    public async Task Earn_IncreasesBalance()
    {
        var id = await CreateCustomerAsync();

        var response = await EarnAsync(id, 100);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await LoyaltyApiFactory.ReadAsync<PointsResultResponse>(response);
        Assert.Equal(100, result.PointsBalance);
        Assert.Equal(TransactionType.Earn, result.Transaction.Type);
    }

    [Fact]
    public async Task Earn_WithZeroPoints_ReturnsBadRequest()
    {
        var id = await CreateCustomerAsync();

        var response = await EarnAsync(id, 0);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Redeem_DecreasesBalance()
    {
        var id = await CreateCustomerAsync();
        await EarnAsync(id, 100);

        var response = await RedeemAsync(id, 30);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await LoyaltyApiFactory.ReadAsync<PointsResultResponse>(response);
        Assert.Equal(70, result.PointsBalance);
    }

    [Fact]
    public async Task Redeem_MoreThanBalance_ReturnsBadRequest_AndKeepsBalance()
    {
        var id = await CreateCustomerAsync();
        await EarnAsync(id, 20);

        var response = await RedeemAsync(id, 50);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var customer = await _client.GetFromJsonAsync<CustomerResponse>(
            $"/api/customers/{id}", LoyaltyApiFactory.Json);
        Assert.Equal(20, customer!.PointsBalance);
    }

    [Fact]
    public async Task Earn_ForUnknownCustomer_ReturnsNotFound()
    {
        var response = await EarnAsync(999999, 10);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTransactions_ReturnsAllTransactions()
    {
        var id = await CreateCustomerAsync();
        await EarnAsync(id, 100);
        await RedeemAsync(id, 40);

        var transactions = await _client.GetFromJsonAsync<List<TransactionResponse>>(
            $"/api/customers/{id}/points/transactions", LoyaltyApiFactory.Json);

        Assert.NotNull(transactions);
        Assert.Equal(2, transactions.Count);
    }
}