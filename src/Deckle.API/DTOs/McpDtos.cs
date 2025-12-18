using System.Text.Json.Serialization;

namespace Deckle.API.DTOs;

/// <summary>
/// MCP Protocol DTOs following the Model Context Protocol specification
/// </summary>

// Base message types
public record McpRequest
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; init; } = "2.0";

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("method")]
    public required string Method { get; init; }

    [JsonPropertyName("params")]
    public object? Params { get; init; }
}

public record McpResponse
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; init; } = "2.0";

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("result")]
    public object? Result { get; init; }

    [JsonPropertyName("error")]
    public McpError? Error { get; init; }
}

public record McpError
{
    [JsonPropertyName("code")]
    public int Code { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("data")]
    public object? Data { get; init; }
}

// Initialize
public record McpInitializeParams
{
    [JsonPropertyName("protocolVersion")]
    public required string ProtocolVersion { get; init; }

    [JsonPropertyName("clientInfo")]
    public required McpClientInfo ClientInfo { get; init; }

    [JsonPropertyName("capabilities")]
    public McpClientCapabilities? Capabilities { get; init; }
}

public record McpClientInfo
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("version")]
    public required string Version { get; init; }
}

public record McpClientCapabilities
{
    [JsonPropertyName("tools")]
    public object? Tools { get; init; }
}

public record McpInitializeResult
{
    [JsonPropertyName("protocolVersion")]
    public string ProtocolVersion { get; init; } = "2024-11-05";

    [JsonPropertyName("serverInfo")]
    public required McpServerInfo ServerInfo { get; init; }

    [JsonPropertyName("capabilities")]
    public required McpServerCapabilities Capabilities { get; init; }
}

public record McpServerInfo
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("version")]
    public required string Version { get; init; }
}

public record McpServerCapabilities
{
    [JsonPropertyName("tools")]
    public object? Tools { get; init; }
}

// Tools
public record McpToolsListResult
{
    [JsonPropertyName("tools")]
    public required List<McpTool> Tools { get; init; }
}

public record McpTool
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [JsonPropertyName("inputSchema")]
    public required object InputSchema { get; init; }
}

public record McpToolCallParams
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("arguments")]
    public Dictionary<string, object>? Arguments { get; init; }
}

public record McpToolCallResult
{
    [JsonPropertyName("content")]
    public required List<McpContent> Content { get; init; }

    [JsonPropertyName("isError")]
    public bool IsError { get; init; }
}

public record McpContent
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

// Deckle-specific DTOs for MCP tool responses
public record McpProjectSummary
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string Role { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}

public record McpProjectDetails
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string Role { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required List<McpComponentSummary> Components { get; init; }
    public required List<McpDataSourceSummary> DataSources { get; init; }
}

public record McpComponentSummary
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
    public string? Size { get; init; }
    public string? DiceType { get; init; }
    public int? Number { get; init; }
}

public record McpDataSourceSummary
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
    public string? GoogleSheetsUrl { get; init; }
}
