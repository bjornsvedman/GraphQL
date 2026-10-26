using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AiGraphqlApi.Tests;

public sealed class GraphQlEndpointTests
{
    [Fact]
    public async Task HealthQuery_ReturnsOk()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync("/graphql", new { query = "{ health }" });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GraphQlResponse>();

        Assert.Equal("ok", result?.Data?.Health);
    }

    [Fact]
    public async Task GenerateMutation_ReturnsDeterministicStubResponse()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            "/graphql",
            new
            {
                query = "mutation ($prompt: String!) { generate(prompt: $prompt) }",
                variables = new { prompt = "hello" }
            });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GraphQlResponse>();

        Assert.Equal("Stub response: hello", result?.Data?.Generate);
    }

    private sealed class GraphQlResponse
    {
        public GraphQlData? Data { get; init; }
    }

    private sealed class GraphQlData
    {
        public string? Health { get; init; }
        public string? Generate { get; init; }
    }
}