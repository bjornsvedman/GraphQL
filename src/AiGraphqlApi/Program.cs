using AiGraphqlApi.GraphQL;
using AiGraphqlApi.Services;
using Microsoft.Extensions.AI;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);
var ollamaSettings = builder.Configuration.GetSection("Ai:Ollama");
var ollamaEndpoint = ollamaSettings["Endpoint"] ?? "http://localhost:11434";
var ollamaModel = ollamaSettings["Model"] ?? "qwen3:4b";

builder.Services.AddSingleton<IChatClient>(
	_ => new OllamaApiClient(new Uri(ollamaEndpoint), ollamaModel));
builder.Services.AddSingleton<GenerationService>();
builder.Services
	.AddGraphQLServer()
	.AddQueryType<Query>()
	.AddMutationType<Mutation>()
	.ModifyRequestOptions(options =>
		options.ExecutionTimeout = TimeSpan.FromSeconds(
			builder.Configuration.GetValue<int>("GraphQL:ExecutionTimeoutSeconds", 120)));

var app = builder.Build();

app.MapGraphQL();

app.Run();

public partial class Program;
