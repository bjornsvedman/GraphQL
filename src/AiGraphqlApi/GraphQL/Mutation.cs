using AiGraphqlApi.Services;
using HotChocolate;

namespace AiGraphqlApi.GraphQL;

public sealed class Mutation
{
    public string Generate(string prompt, [Service] GenerationService generationService)
    {
        return generationService.Generate(prompt);
    }
}