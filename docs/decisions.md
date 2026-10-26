# Decisions

## 2026-10-26: Target .NET 10 LTS

Use .NET 10 even if it must be installed. It is the requested latest LTS target.

## 2026-10-26: Defer real AI integration

The project is AI-assisted in how it is developed, but the application will not call an AI model yet. Use a deterministic stub for the initial API workflow.

## 2026-10-26: Keep the first architecture lightweight

Use a small service boundary between GraphQL resolvers and behavior so it can be tested without a server or network. Avoid adding infrastructure beyond the learning goal.

## 2026-10-26: Keep project guidance in the workspace

Store Copilot customization under `.github/` so it is versioned with the project and available to future contributors.