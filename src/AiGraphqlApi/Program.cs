using AiGraphqlApi.GraphQL;
using AiGraphqlApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<GenerationService>();
builder.Services
	.AddGraphQLServer()
	.AddQueryType<Query>()
	.AddMutationType<Mutation>();

var app = builder.Build();

app.MapGraphQL();

app.Run();

public partial class Program;
