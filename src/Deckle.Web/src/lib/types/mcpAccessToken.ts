// MCP Access Token types

export interface McpAccessToken {
  id: string;
  name: string;
  description?: string;
  tokenSuffix: string; // Last 4 characters for identification
  createdAt: string;
  lastUsedAt?: string;
  revokedAt?: string;
  expiresAt?: string;
  isValid: boolean;
}

export interface CreateMcpAccessTokenRequest {
  name: string;
  description?: string;
  expiresAt?: string;
}

export interface CreateMcpAccessTokenResponse {
  id: string;
  name: string;
  description?: string;
  token: string; // Plain token - only shown once!
  tokenSuffix: string;
  createdAt: string;
  expiresAt?: string;
}

export interface UpdateMcpAccessTokenRequest {
  name: string;
  description?: string;
}
