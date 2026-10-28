# Decisions

## 2026-10-26: Target .NET 10 LTS

Use .NET 10 even if it must be installed. It is the requested latest LTS target.

## 2026-10-26: Initially defer real AI integration (superseded 2026-10-28)

The initial implementation used a deterministic stub while real AI integration was deferred. This decision was superseded when local Ollama was approved as the first provider.

## 2026-10-28: Use local Ollama behind `IChatClient`

The first real model provider is local Ollama, integrated through `Microsoft.Extensions.AI.IChatClient`. Keep provider-specific setup at the dependency-injection boundary so Azure or Google can be considered later without coupling GraphQL resolvers or application behavior to their SDKs. Hosted providers and their costs/credentials remain out of scope.

## 2026-10-26: Keep the first architecture lightweight

Use a small service boundary between GraphQL resolvers and behavior so it can be tested without a server or network. Avoid adding infrastructure beyond the learning goal.

## 2026-10-26: Keep project guidance in the workspace

Store Copilot customization under `.github/` so it is versioned with the project and available to future contributors.