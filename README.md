# AI GraphQL API

A .NET 10 GraphQL API built with Hot Chocolate. Generation uses a local Ollama model through `Microsoft.Extensions.AI.IChatClient`; no hosted AI service or API key is required.

## Prerequisites

- .NET 10 SDK
- Ollama for Windows
- The configured model, `qwen3:4b`

Check that Ollama is running and the model is installed:

```powershell
ollama list
```

If the model is missing, download it with:

```powershell
ollama pull qwen3:4b
```

If PowerShell cannot find `ollama`, open a new terminal so it picks up the Ollama PATH, or run the executable directly:

```powershell
& "$env:LOCALAPPDATA\Programs\Ollama\ollama.exe" list
```

## Run the API

From the repository root:

```powershell
dotnet run --project src/AiGraphqlApi/AiGraphqlApi.csproj --launch-profile http
```

The HTTP launch profile listens on `http://localhost:5051`. Keep the terminal running while you use the API. The GraphQL endpoint and browser IDE are at:

```text
http://localhost:5051/graphql
```

## Try it in Postman

Create a `POST` request to `http://localhost:5051/graphql`. Choose **Body → raw → JSON** and send:

```json
{
  "query": "mutation ($prompt: String!) { generate(prompt: $prompt) }",
  "variables": {
    "prompt": "Explain GraphQL in one sentence."
  }
}
```

The generated answer is returned in `data.generate`. You can also check the API with this query:

Classify text with the local model:

```json
{
  "query": "mutation ($text: String!) { classifySentiment(text: $text) }",
  "variables": {
    "text": "The app crashes every time I open it."
  }
}
```

The result is one of `POSITIVE`, `NEUTRAL`, or `NEGATIVE` in `data.classifySentiment`.

You can check the API with this query:

```json
{
  "query": "{ health }"
}
```

## Build and test

Run these commands from the repository root:

```powershell
dotnet build AiGraphqlApi.sln
dotnet test AiGraphqlApi.sln
```

The tests inject a fake `IChatClient`, so they do not require Ollama or network access.

## Local observability

Start Docker Desktop, then start Prometheus and Grafana from the repository root:

```powershell
docker compose -f observability/compose.yaml up -d
```

Start the API with the opt-in metrics exporter:

```powershell
dotnet run --project src/AiGraphqlApi/AiGraphqlApi.csproj --launch-profile observability
```

The API sends OpenTelemetry metrics to Prometheus' OTLP receiver. The provisioned Grafana dashboard is at `http://localhost:3000` (first login: `admin` / `admin`; Grafana asks you to change the password). Prometheus is at `http://localhost:9090`.

Metrics may take up to a minute to appear. Both container ports are bound to loopback only. To stop the API, press Ctrl+C; stop the containers with:

```powershell
docker compose -f observability/compose.yaml down
```

Prometheus and Grafana data persist in Docker volumes. To remove the stored local data as well, use `docker compose -f observability/compose.yaml down -v`.

The metrics exporter is disabled in the default launch profiles. Do not expose the Prometheus receiver or Grafana ports publicly without adding appropriate access controls.

## Configuration

Edit `src/AiGraphqlApi/appsettings.json` to change the Ollama endpoint, model, or GraphQL request timeout. Defaults are `http://localhost:11434`, `qwen3:4b`, and 120 seconds.