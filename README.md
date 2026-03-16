# AI Mentor

An AI-powered computer networks mentor built with ASP.NET Core 9 and OpenAI API. Supports multiple chat sessions with full message history, served via a cyberpunk-styled web UI.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- An OpenAI-compatible API key and endpoint

## Configuration

Open `src/AIMentor/appsettings.Development.json` and fill in your API credentials:

```json
"Features": {
  "OpenApi": {
    "OpenAiKey": "<your-api-key>",
    "BaseUrl": "https://api.openai.com/v1",
    "OpenAiModel": "<model-name>"
  }
}
```

| Field | Description |
|---|---|
| `OpenAiKey` | Your API key |
| `BaseUrl` | API base URL (change if using a custom or proxy endpoint) |
| `OpenAiModel` | Model name, e.g. `gpt-4o` |

Alternatively, use [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) to avoid storing the key in a file:

```bash
cd src/AIMentor
dotnet user-secrets set "Features:OpenApi:OpenAiKey" "<your-api-key>"
```

## Running

```bash
cd src/AIMentor
dotnet run
```

The application will start on `http://localhost:5285` and open the web UI in your default browser automatically.

## API Endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/sessions` | List all chat sessions |
| `POST` | `/sessions/{sessionId}/messages` | Send a message (use `sessionId=0` to create a new session) |
| `GET` | `/sessions/{sessionId}/messages` | Get all messages for a session |
| `DELETE` | `/sessions/{sessionId}` | Delete a session and all its messages |