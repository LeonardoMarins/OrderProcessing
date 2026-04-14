using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OrderProcessing.IntegrationTests.Infrastructure;

namespace OrderProcessing.IntegrationTests.Orders;

public class OrdersControllerTests : IClassFixture<OrderProcessingWebFactory>
{
    private readonly HttpClient _client;

    public OrdersControllerTests(OrderProcessingWebFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostOrder_ValidPayload_Returns201()
    {
        var payload = new { client = "Cliente Teste", value = 100.00 };

        var response = await _client.PostAsJsonAsync("/orders", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PostOrder_EmptyClient_Returns400()
    {
        var payload = new { client = "", value = 100.00 };

        var response = await _client.PostAsJsonAsync("/orders", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostOrder_ZeroValue_Returns400()
    {
        var payload = new { client = "Cliente Teste", value = 0 };

        var response = await _client.PostAsJsonAsync("/orders", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOrders_Returns200WithList()
    {
        var response = await _client.GetAsync("/orders");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task GetOrderById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync($"/orders/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
