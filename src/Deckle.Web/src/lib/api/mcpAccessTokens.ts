import { api } from './client';
import type {
  McpAccessToken,
  CreateMcpAccessTokenRequest,
  CreateMcpAccessTokenResponse,
  UpdateMcpAccessTokenRequest
} from '$lib/types';

/**
 * MCP Access Tokens API
 */
export const mcpAccessTokensApi = {
  /**
   * Get all MCP access tokens for the current user
   */
  list: (fetchFn?: typeof fetch) =>
    api.get<McpAccessToken[]>('/mcp-tokens', undefined, fetchFn),

  /**
   * Get a single MCP access token by ID
   */
  getById: (id: string, fetchFn?: typeof fetch) =>
    api.get<McpAccessToken>(`/mcp-tokens/${id}`, undefined, fetchFn),

  /**
   * Create a new MCP access token
   * Returns the plain token - this is the ONLY time the token is shown!
   */
  create: (data: CreateMcpAccessTokenRequest, fetchFn?: typeof fetch) =>
    api.post<CreateMcpAccessTokenResponse>('/mcp-tokens', data, undefined, fetchFn),

  /**
   * Update MCP access token metadata (name and description)
   */
  update: (id: string, data: UpdateMcpAccessTokenRequest, fetchFn?: typeof fetch) =>
    api.put<void>(`/mcp-tokens/${id}`, data, undefined, fetchFn),

  /**
   * Revoke an MCP access token (soft delete - sets RevokedAt timestamp)
   */
  revoke: (id: string, fetchFn?: typeof fetch) =>
    api.post<void>(`/mcp-tokens/${id}/revoke`, undefined, undefined, fetchFn),

  /**
   * Permanently delete an MCP access token (hard delete)
   */
  delete: (id: string, fetchFn?: typeof fetch) =>
    api.delete<void>(`/mcp-tokens/${id}`, undefined, fetchFn)
};
