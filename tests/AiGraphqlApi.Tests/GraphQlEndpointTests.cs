using System.Net.Http.Json;
using AiGraphqlApi.Tests.Fakes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
    public async Task GenerateMutation_UsesInjectedChatClient()
    {
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IChatClient>();
                services.AddSingleton<IChatClient>(new FakeChatClient());
            }));
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