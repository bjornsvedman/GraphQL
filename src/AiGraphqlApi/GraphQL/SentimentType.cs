using AiGraphqlApi.Services;
using HotChocolate.Types;

namespace AiGraphqlApi.GraphQL;

public sealed class SentimentType : EnumType<Sentiment>
{
    protected override void Configure(IEnumTypeDescriptor<Sentiment> descriptor)
    {
        descriptor.Name(nameof(Sentiment));
        descriptor.Value(Sentiment.Positive).Name("POSITIVE");
        descriptor.Value(Sentiment.Neutral).Name("NEUTRAL");
        descriptor.Value(Sentiment.Negative).Name("NEGATIVE");
    }
}