using Microsoft.Extensions.AI;

namespace AiGraphqlApi.Tests.Fakes;

public sealed class FakeChatClient(string? responseText = null) : IChatClient
{
    public int RequestCount { get; private set; }

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        RequestCount++;
        var prompt = messages.Last(message => message.Role == ChatRole.User).Text;
        var response = new ChatMessage(ChatRole.Assistant, responseText ?? $"Stub response: {prompt}");

        return Task.FromResult(new ChatResponse(response));
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        return serviceType.IsInstanceOfType(this) ? this : null;
    }

    public void Dispose()
    {
    }
}