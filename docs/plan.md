# Project Plan

## Goal

Learn to build a small GraphQL API in .NET 10 with GitHub Copilot as a development partner. The first AI integration uses local Ollama; hosted AI providers remain out of scope until explicitly requested.

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
- [x] Review the workflow and refine instructions based on where Copilot needed correction. Added guidance to verify version-sensitive tooling details; kept one-off Postman and VS Code UI troubleshooting out of project coding rules.
- [x] Replace the echo implementation with async `IChatClient` generation, configure the local Ollama adapter, and keep automated tests provider-neutral and offline.
- [x] Install Ollama, pull the configured model, and verify live generation against the local server.

## Local Ollama Milestone

Status: complete. Provider-neutral async service, Ollama configuration, offline fake-client coverage, and a live GraphQL request against `qwen3:4b` are verified.

### Prerequisites

- Install Ollama and verify it is available with `ollama --version`.
- Pull a small local model that fits the machine. Check its license and hardware requirements before choosing it; do not assume one model suits every machine.
- Confirm the Ollama server is reachable at `http://localhost:11434`.

### Implementation

1. Verify the current stable `OllamaSharp` and `Microsoft.Extensions.AI` APIs before selecting package versions.
2. Register `IChatClient` with the OllamaSharp adapter. Read the endpoint and model name from configuration; keep provider-specific types in startup/DI.
3. Make the generation service and GraphQL mutation asynchronous, passing `CancellationToken` through.
4. Preserve the existing GraphQL operation shape unless the result contract needs a deliberate change.
5. Replace the exact echo assertion with an offline fake `IChatClient`; automated tests must pass without Ollama running or network access.
6. Manually call `generate` against the local model after the API and Ollama are running.

### Acceptance

- `dotnet build AiGraphqlApi.sln` and `dotnet test AiGraphqlApi.sln` pass without a running Ollama server.
- The local Ollama-backed mutation returns generated text when Ollama is running with the configured model.
- No cloud provider, API key, or hosted AI service is introduced.

### Later

Add Azure or Google only when requested. Each should provide an `IChatClient` adapter selected through configuration; verify feature support and pricing before enabling it. Model-specific capabilities may prevent a pure configuration-only swap.

## Development Loop

1. Pick one unchecked milestone or a small subtask.
2. Ask Copilot for a brief implementation plan and review it before code changes.
3. Implement one vertical slice with a focused test.
4. Run `dotnet build AiGraphqlApi.sln` and `dotnet test AiGraphqlApi.sln`.
5. Review the diff for behavior, unnecessary complexity, and learning notes; then commit the completed slice.

## Out of Scope For Now

- Hosted AI providers, embeddings, retrieval, and model tool calling.
- Authentication, persistence, Docker, CI, and cloud deployment.