# Copilot Instructions

## Project

- This is a learning project for a GraphQL API in .NET 10 using Hot Chocolate.
- Real AI integration is deferred. Keep any initial AI-shaped interface backed by a deterministic local stub; do not call hosted AI services or require API keys.
- Read `docs/plan.md` for scope and `docs/decisions.md` for settled choices before changing architecture.

## Working Style

- Work in small, reviewable vertical slices. State a short plan before a multi-file change, then implement and verify it.
- Prefer the smallest change that satisfies the current plan item. Do not add packages, abstractions, or infrastructure without explaining why.
- Preserve existing user changes and avoid unrelated cleanup.
- Explain relevant trade-offs so the user can learn the reasoning, not just receive code.
- Verify version-sensitive tooling guidance against the installed version or official docs; state uncertainty instead of guessing.

## Engineering

- Use nullable reference types and async APIs; pass `CancellationToken` through asynchronous work.
- Keep GraphQL resolvers thin and put behavior in testable application services.
- Tests must not require network access. Prefer deterministic fakes for external dependencies.
- Do not put secrets in source control.

## Verification

- Run `dotnet build AiGraphqlApi.sln` after implementation changes.
- Run `dotnet test AiGraphqlApi.sln` when behavior or tests change.
- Report what was verified and mention any checks that could not run.