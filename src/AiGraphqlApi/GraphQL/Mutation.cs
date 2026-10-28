using AiGraphqlApi.Services;
using HotChocolate;

namespace AiGraphqlApi.GraphQL;

public sealed class Mutation
{
    public Task<string> Generate(
        string prompt,
        [Service] GenerationService generationService,
        CancellationToken cancellationToken)
    {
        return generationService.GenerateAsync(prompt, cancellationToken);
    }
}