# Project Plan

## Goal

Learn to build a small GraphQL API in .NET 10 with GitHub Copilot as a development partner. The API's real AI integration is deliberately deferred; the first AI-shaped workflow uses a deterministic stub.

## Scope

- .NET 10 LTS and Hot Chocolate for the GraphQL API.
- Free tooling and packages; no paid services or API keys.
- A minimal, maintainable design suitable for learning rather than production-scale infrastructure.
- Use project-level Copilot instructions and small, testable tasks to make AI-assisted development reviewable.

## Milestones

- [x] Scaffold solution, API project, and xUnit test project.
- [x] Add workspace-level Copilot instructions and this plan.
- [x] Add Hot Chocolate and expose a health query.
- [x] Add a deterministic stub-backed `generate(prompt)` mutation and focused tests.
- [ ] Review the workflow and refine instructions based on where Copilot needed correction.
- [ ] Later: replace the stub with local Ollama; keep this out of the current implementation.

## Development Loop

1. Pick one unchecked milestone or a small subtask.
2. Ask Copilot for a brief implementation plan and review it before code changes.
3. Implement one vertical slice with a focused test.
4. Run `dotnet build AiGraphqlApi.sln` and `dotnet test AiGraphqlApi.sln`.
5. Review the diff for behavior, unnecessary complexity, and learning notes; then commit the completed slice.

## Out of Scope For Now

- Real AI providers, Ollama, embeddings, retrieval, and model tool calling.
- Authentication, persistence, Docker, CI, and cloud deployment.