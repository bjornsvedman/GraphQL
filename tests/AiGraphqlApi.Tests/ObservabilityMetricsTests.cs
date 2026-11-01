using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Net.Http.Json;
using AiGraphqlApi.Observability;
using AiGraphqlApi.Tests.Fakes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AiGraphqlApi.Tests;

public sealed class ObservabilityMetricsTests
{
    [Fact]
    public async Task RequestsRecordHttpAiGraphQlErrorAndDurationMetrics()
    {
        using var measurements = new MetricMeasurements();
        using var successFactory = CreateFactory(new FakeChatClient());
        using var successClient = successFactory.CreateClient();
        using var successResponse = await successClient.PostAsJsonAsync(
            "/graphql",
            new
            {
                query = "mutation ($prompt: String!) { generate(prompt: $prompt) }",
                variables = new { prompt = "hello" }
            });
        successResponse.EnsureSuccessStatusCode();

        using var errorFactory = CreateFactory(new FakeChatClient("POSITIVE because it is great."));
        using var errorClient = errorFactory.CreateClient();
        using var errorResponse = await errorClient.PostAsJsonAsync(
            "/graphql",
            new
            {
                query = "mutation ($text: String!) { classifySentiment(text: $text) }",
                variables = new { text = "This is a test message." }
            });
        var errorResult = await errorResponse.Content.ReadFromJsonAsync<GraphQlResponse>();

        Assert.NotEmpty(errorResult?.Errors ?? []);
        Assert.True(measurements.CounterCount(ApiMetrics.HttpRequestsName) >= 2);
        Assert.True(measurements.CounterCount(ApiMetrics.GraphQlErrorsName) >= 1);
        Assert.True(measurements.CounterCount(ApiMetrics.AiRequestsName) >= 2);
        Assert.True(measurements.HistogramCount(ApiMetrics.HttpRequestDurationName) >= 2);
        Assert.True(measurements.HistogramCount(ApiMetrics.AiRequestDurationName) >= 2);
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
        public List<GraphQlError>? Errors { get; init; }
    }

    private sealed class GraphQlError
    {
        public string? Message { get; init; }
    }

    private sealed class MetricMeasurements : IDisposable
    {
        private readonly MeterListener _listener = new();
        private readonly ConcurrentDictionary<string, long> _counters = new();
        private readonly ConcurrentDictionary<string, int> _histograms = new();

        public MetricMeasurements()
        {
            _listener.InstrumentPublished = (instrument, listener) =>
            {
                if (instrument.Meter.Name == ApiMetrics.MeterName)
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            };
            _listener.SetMeasurementEventCallback<long>((instrument, measurement, _, _) =>
                _counters.AddOrUpdate(instrument.Name, measurement, (_, current) => current + measurement));
            _listener.SetMeasurementEventCallback<double>((instrument, _, _, _) =>
                _histograms.AddOrUpdate(instrument.Name, 1, (_, current) => current + 1));
            _listener.Start();
        }

        public long CounterCount(string name) => _counters.GetValueOrDefault(name);

        public int HistogramCount(string name) => _histograms.GetValueOrDefault(name);

        public void Dispose() => _listener.Dispose();
    }
}