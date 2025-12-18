# Deckle MCP Integration

This document describes how to use Deckle's Model Context Protocol (MCP) server to interact with Deckle via AI agents like Claude.

## Overview

Deckle implements an MCP server that allows AI assistants to access and interact with your Deckle projects programmatically. This enables AI agents to:
- List your projects
- Get detailed information about projects, components, and data sources
- Provide intelligent assistance with project management

## Authentication

The MCP server uses the MCP access tokens you create in your account settings for authentication.

### Creating an Access Token

1. Go to **Account Settings** → **MCP Access Tokens**
2. Click **Create New Token**
3. Provide a name (e.g., "Claude Desktop")
4. Optionally add a description and expiration date
5. Click **Create Token**
6. **Important**: Copy the token immediately - it will only be shown once!

### Using the Token

Include the token in the `Authorization` header of your MCP requests:

```
Authorization: Bearer mcp_YOUR_TOKEN_HERE
```

## MCP Endpoint

The MCP server is available at:

```
POST /mcp
```

All MCP protocol messages are sent to this endpoint using JSON-RPC 2.0 format.

## Available Tools

### 1. list_projects

Lists all projects accessible to the authenticated user.

**Input**: None

**Output**: Array of project summaries with:
- `Id`: Project GUID
- `Name`: Project name
- `Description`: Project description (optional)
- `Role`: User's role in the project (Owner, Editor, Viewer)
- `CreatedAt`: Creation timestamp
- `UpdatedAt`: Last update timestamp

**Example Request**:
```json
{
  "jsonrpc": "2.0",
  "id": "1",
  "method": "tools/call",
  "params": {
    "name": "list_projects",
    "arguments": {}
  }
}
```

**Example Response**:
```json
{
  "jsonrpc": "2.0",
  "id": "1",
  "result": {
    "content": [
      {
        "type": "text",
        "text": "[{\"Id\":\"...\",\"Name\":\"My Game\",\"Description\":\"A card game\",\"Role\":\"Owner\",\"CreatedAt\":\"2024-01-01T00:00:00Z\",\"UpdatedAt\":\"2024-01-01T00:00:00Z\"}]"
      }
    ],
    "isError": false
  }
}
```

### 2. get_project_details

Gets detailed information about a specific project, including all components and data sources.

**Input**:
- `project_id` (required): The GUID of the project

**Output**: Detailed project information with:
- Basic project info (ID, name, description, role, timestamps)
- `Components`: Array of components with:
  - `Id`: Component GUID
  - `Name`: Component name
  - `Type`: "Card" or "Dice"
  - `Size`: Card size (for cards)
  - `DiceType`: Dice type (for dice)
  - `Number`: Dice number (for dice)
- `DataSources`: Array of data sources with:
  - `Id`: Data source GUID
  - `Name`: Data source name
  - `Type`: Data source type (e.g., "GoogleSheets")
  - `GoogleSheetsUrl`: URL to Google Sheet (if applicable)

**Example Request**:
```json
{
  "jsonrpc": "2.0",
  "id": "2",
  "method": "tools/call",
  "params": {
    "name": "get_project_details",
    "arguments": {
      "project_id": "12345678-1234-1234-1234-123456789012"
    }
  }
}
```

**Example Response**:
```json
{
  "jsonrpc": "2.0",
  "id": "2",
  "result": {
    "content": [
      {
        "type": "text",
        "text": "{\"Id\":\"...\",\"Name\":\"My Game\",\"Description\":\"A card game\",\"Role\":\"Owner\",\"CreatedAt\":\"...\",\"UpdatedAt\":\"...\",\"Components\":[{\"Id\":\"...\",\"Name\":\"Hero Card\",\"Type\":\"Card\",\"Size\":\"Poker\"}],\"DataSources\":[{\"Id\":\"...\",\"Name\":\"Cards Data\",\"Type\":\"GoogleSheets\",\"GoogleSheetsUrl\":\"https://...\"}]}"
      }
    ],
    "isError": false
  }
}
```

## Using with Claude Desktop

To use the Deckle MCP server with Claude Desktop:

1. Create an MCP access token in Deckle
2. Configure Claude Desktop to connect to the Deckle MCP server:

```json
{
  "mcpServers": {
    "deckle": {
      "url": "http://localhost:5000/mcp",
      "headers": {
        "Authorization": "Bearer mcp_YOUR_TOKEN_HERE"
      }
    }
  }
}
```

3. Restart Claude Desktop
4. Claude will now be able to access your Deckle projects!

## Example Conversation with Claude

Once configured, you can ask Claude things like:

- "What projects do I have in Deckle?"
- "Show me the details of my 'Card Game' project"
- "What components are in my project?"
- "List all my Google Sheets data sources"

Claude will use the MCP tools to fetch the information and provide helpful responses.

## Security Best Practices

1. **Token Management**:
   - Create separate tokens for different applications/devices
   - Use descriptive names to identify where tokens are used
   - Set expiration dates for tokens when appropriate
   - Revoke tokens immediately if compromised

2. **Token Storage**:
   - Never commit tokens to version control
   - Store tokens securely using environment variables or secure vaults
   - Don't share tokens publicly

3. **Monitoring**:
   - Check the "Last Used" timestamp in account settings
   - Revoke unused or suspicious tokens
   - Review active tokens regularly

## Troubleshooting

### 401 Unauthorized
- Check that your token is included in the Authorization header
- Verify the token hasn't been revoked
- Check if the token has expired

### 404 Method Not Found
- Verify the method name is correct (e.g., "tools/call")
- Check the JSON-RPC format matches the specification

### Tool Errors
- Verify the tool name is correct (e.g., "list_projects")
- Check that required arguments are provided
- Ensure GUIDs are in valid format

## Future Enhancements

Potential future tools that could be added:
- Create/update/delete projects
- Add components to projects
- Manage data sources
- Export project data
- Generate game assets

## Support

For issues or questions about MCP integration:
1. Check this documentation
2. Review token status in account settings
3. Check API logs for error details
