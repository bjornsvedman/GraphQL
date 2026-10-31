using AiGraphqlApi.Services;

namespace AiGraphqlApi.Tests;

public sealed record SentimentExample(string Text, Sentiment Expected);

public static class SentimentExamples
{
    public static IReadOnlyList<SentimentExample> All { get; } =
    [
        new("I love this product; it works perfectly.", Sentiment.Positive),
        new("The staff were friendly and helpful.", Sentiment.Positive),
        new("The package arrived on Tuesday.", Sentiment.Neutral),
        new("The device has three USB ports.", Sentiment.Neutral),
        new("The app crashes every time I open it.", Sentiment.Negative),
        new("The food was cold and the service was slow.", Sentiment.Negative)
    ];
}