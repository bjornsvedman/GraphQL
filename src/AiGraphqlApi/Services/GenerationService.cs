using Microsoft.Extensions.AI;

namespace AiGraphqlApi.Services;

public sealed class GenerationService(IChatClient chatClient)
{
    public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken)
    {
        var response = await chatClient.GetResponseAsync(
            [new ChatMessage(ChatRole.User, prompt)],
            cancellationToken: cancellationToken);

        return response.Text ?? string.Empty;
    }
}