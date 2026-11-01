using AiGraphqlApi.GraphQL;
using AiGraphqlApi.Observability;
using AiGraphqlApi.Services;
using Microsoft.Extensions.AI;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);
var ollamaSettings = builder.Configuration.GetSection("Ai:Ollama");
var ollamaEndpoint = ollamaSettings["Endpoint"] ?? "http://localhost:11434";
var ollamaModel = ollamaSettings["Model"] ?? "qwen3:4b";

builder.Services.AddSingleton<IChatClient>(
	_ => new OllamaApiClient(new Uri(ollamaEndpoint), ollamaModel));
builder.Services.AddSingleton<GenerationService>();
builder.Services.AddOpenTelemetry()
	.WithMetrics(metrics =>
	{
		metrics.AddMeter(ApiMetrics.MeterName);

		if (builder.Configuration.GetValue<bool>("Observability:Prometheus:Enabled"))
		{
			var endpoint = builder.Configuration["Observability:Prometheus:Endpoint"]
				?? "http://localhost:9090/api/v1/otlp/v1/metrics";

			metrics.AddOtlpExporter(options =>
			{
				options.Endpoint = new Uri(endpoint);
				options.Protocol = OtlpExportProtocol.HttpProtobuf;
			});
		}
	});
builder.Services
	.AddGraphQLServer()
	.AddQueryType<Query>()
	.AddMutationType<Mutation>()
	.AddType<SentimentType>()
	.AddErrorFilter(error =>
	{
		ApiMetrics.RecordGraphQlError();
		return error;
	})
	.ModifyRequestOptions(options =>
		options.ExecutionTimeout = TimeSpan.FromSeconds(
			builder.Configuration.GetValue<int>("GraphQL:ExecutionTimeoutSeconds", 120)));

var app = builder.Build();

app.Use(async (context, next) =>
{
	if (!context.Request.Path.StartsWithSegments("/graphql"))
	{
		await next(context);
		return;
	}

	var startedAt = System.Diagnostics.Stopwatch.GetTimestamp();
	try
	{
		await next(context);
	}
	catch
	{
		ApiMetrics.RecordHttpRequest(StatusCodes.Status500InternalServerError, System.Diagnostics.Stopwatch.GetElapsedTime(startedAt));
		throw;
	}

	ApiMetrics.RecordHttpRequest(context.Response.StatusCode, System.Diagnostics.Stopwatch.GetElapsedTime(startedAt));
});

app.MapGraphQL();

app.Run();

public partial class Program;
