using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace AiGraphqlApi.Observability;

public static class ApiMetrics
{
    public const string MeterName = "AiGraphqlApi";
    public const string HttpRequestsName = "graphql.http.requests";
    public const string HttpRequestDurationName = "graphql.http.request.duration";
    public const string GraphQlErrorsName = "graphql.execution.errors";
    public const string AiRequestsName = "ai.client.requests";
    public const string AiRequestDurationName = "ai.client.request.duration";

    private static readonly Meter Meter = new(MeterName);
    private static readonly Counter<long> HttpRequests = Meter.CreateCounter<long>(HttpRequestsName, "{request}");
    private static readonly Histogram<double> HttpRequestDuration = Meter.CreateHistogram<double>(HttpRequestDurationName, "s");
    private static readonly Counter<long> GraphQlErrors = Meter.CreateCounter<long>(GraphQlErrorsName, "{error}");
    private static readonly Counter<long> AiRequests = Meter.CreateCounter<long>(AiRequestsName, "{request}");
    private static readonly Histogram<double> AiRequestDuration = Meter.CreateHistogram<double>(AiRequestDurationName, "s");

    public static void RecordHttpRequest(int statusCode, TimeSpan duration)
    {
        var tags = new TagList { { "status_code", statusCode } };
        HttpRequests.Add(1, tags);
        HttpRequestDuration.Record(duration.TotalSeconds, tags);
    }

    public static void RecordGraphQlError() => GraphQlErrors.Add(1);

    public static void RecordAiRequest(string operation, string outcome, TimeSpan duration)
    {
        var tags = new TagList
        {
            { "operation", operation },
            { "outcome", outcome }
        };

        AiRequests.Add(1, tags);
        AiRequestDuration.Record(duration.TotalSeconds, tags);
    }
}