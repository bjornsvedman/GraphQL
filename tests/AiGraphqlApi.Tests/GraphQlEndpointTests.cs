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

    [Theory]
    [InlineData("positive", "POSITIVE")]
    [InlineData(" NEUTRAL ", "NEUTRAL")]
    [InlineData("Negative", "NEGATIVE")]
    public async Task ClassifySentimentMutation_ReturnsEnum(string modelResponse, string expectedSentiment)
    {
        var fakeChatClient = new FakeChatClient(modelResponse);
        using var factory = CreateFactory(fakeChatClient);
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            "/graphql",
            new
            {
                query = "mutation ($text: String!) { classifySentiment(text: $text) }",
                variables = new { text = "This is a test message." }
            });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GraphQlResponse>();

        Assert.Equal(expectedSentiment, result?.Data?.ClassifySentiment);
        Assert.Null(result?.Errors);
    }

    [Fact]
    public async Task ClassifySentimentMutation_ReturnsErrorForInvalidModelOutput()
    {
        var fakeChatClient = new FakeChatClient("POSITIVE because it is great.");
        using var factory = CreateFactory(fakeChatClient);
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            "/graphql",
            new
            {
                query = "mutation ($text: String!) { classifySentiment(text: $text) }",
                variables = new { text = "This is a test message." }
            });

        var result = await response.Content.ReadFromJsonAsync<GraphQlResponse>();

        Assert.NotNull(result?.Errors);
        Assert.NotEmpty(result.Errors);
        Assert.Equal(1, fakeChatClient.RequestCount);
    }

    [Fact]
    public async Task ClassifySentimentMutation_RejectsEmptyTextBeforeCallingModel()
    {
        var fakeChatClient = new FakeChatClient("NEUTRAL");
        using var factory = CreateFactory(fakeChatClient);
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            "/graphql",
            new
            {
                query = "mutation ($text: String!) { classifySentiment(text: $text) }",
                variables = new { text = " " }
            });

        var result = await response.Content.ReadFromJsonAsync<GraphQlResponse>();

        Assert.NotNull(result?.Errors);
        Assert.NotEmpty(result.Errors);
        Assert.Equal(0, fakeChatClient.RequestCount);
    }

    private static WebApplicationFactory<Program> CreateFactory(FakeChatClient fakeChatClient)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IChatClient>();
                services.AddSingleton<IChatClient>(fakeChatClient);
            }));
    }

    private sealed class GraphQlResponse
    {
        public GraphQlData? Data { get; init; }
        public List<GraphQlError>? Errors { get; init; }
    }

    private sealed class GraphQlError
    {
        public string? Message { get; init; }
    }

    private sealed class GraphQlData
    {
        public string? Health { get; init; }
        public string? Generate { get; init; }
        public string? ClassifySentiment { get; init; }
    }
}