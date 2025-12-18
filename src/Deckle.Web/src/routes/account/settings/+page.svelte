<script lang="ts">
  import type { PageData } from './$types';
  import PageLayout from '$lib/components/PageLayout.svelte';
  import { mcpAccessTokensApi, ApiError } from '$lib/api';
  import type { McpAccessToken, CreateMcpAccessTokenResponse } from '$lib/types';

  let { data }: { data: PageData } = $props();

  // MCP Token management state
  let tokens = $state<McpAccessToken[]>(data.tokens || []);
  let showCreateDialog = $state(false);
  let showTokenDialog = $state(false);
  let newTokenName = $state('');
  let newTokenDescription = $state('');
  let newTokenExpiresAt = $state('');
  let createdToken = $state<CreateMcpAccessTokenResponse | null>(null);
  let isCreating = $state(false);
  let createError = $state('');

  async function createToken() {
    if (!newTokenName.trim()) {
      createError = 'Token name is required';
      return;
    }

    isCreating = true;
    createError = '';

    try {
      const expiresAt = newTokenExpiresAt ? new Date(newTokenExpiresAt).toISOString() : undefined;
      createdToken = await mcpAccessTokensApi.create({
        name: newTokenName,
        description: newTokenDescription || undefined,
        expiresAt
      });

      // Refresh tokens list
      tokens = await mcpAccessTokensApi.list();

      // Close create dialog and show token dialog
      showCreateDialog = false;
      showTokenDialog = true;

      // Reset form
      newTokenName = '';
      newTokenDescription = '';
      newTokenExpiresAt = '';
    } catch (err) {
      if (err instanceof ApiError) {
        createError = err.message;
      } else {
        createError = 'Failed to create token';
      }
    } finally {
      isCreating = false;
    }
  }

  async function revokeToken(tokenId: string) {
    if (!confirm('Are you sure you want to revoke this token? This action cannot be undone.')) {
      return;
    }

    try {
      await mcpAccessTokensApi.revoke(tokenId);
      tokens = await mcpAccessTokensApi.list();
    } catch (err) {
      alert('Failed to revoke token');
    }
  }

  async function deleteToken(tokenId: string) {
    if (!confirm('Are you sure you want to permanently delete this token? This action cannot be undone.')) {
      return;
    }

    try {
      await mcpAccessTokensApi.delete(tokenId);
      tokens = await mcpAccessTokensApi.list();
    } catch (err) {
      alert('Failed to delete token');
    }
  }

  function copyToken() {
    if (createdToken) {
      navigator.clipboard.writeText(createdToken.token);
      alert('Token copied to clipboard!');
    }
  }

  function formatDate(dateString: string | undefined) {
    if (!dateString) return 'Never';
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
</script>

<PageLayout>
  {#snippet header()}
    <div class="header-text">
      <h1>Account Settings</h1>
      <p class="subtitle">View and manage your account information</p>
    </div>
  {/snippet}

  <div class="settings-content">
    <div class="settings-section">
      <h2>Profile Information</h2>

      <div class="profile-card">
        <div class="profile-avatar-section">
          {#if data.user.picture}
            <img src={data.user.picture} alt={data.user.name || 'User'} class="profile-avatar" />
          {:else}
            <div class="profile-avatar-placeholder">
              {data.user.name?.charAt(0).toUpperCase() || 'U'}
            </div>
          {/if}
        </div>

        <div class="profile-fields">
          <div class="field-group">
            <label class="field-label">User ID</label>
            <div class="field-value">{data.user.id || 'N/A'}</div>
          </div>

          <div class="field-group">
            <label class="field-label">Name</label>
            <div class="field-value">{data.user.name || 'N/A'}</div>
          </div>

          <div class="field-group">
            <label class="field-label">Email</label>
            <div class="field-value">{data.user.email || 'N/A'}</div>
          </div>
        </div>
      </div>
    </div>

    <div class="settings-section">
      <div class="section-header">
        <h2>MCP Access Tokens</h2>
        <button class="btn-primary" onclick={() => (showCreateDialog = true)}>
          Create New Token
        </button>
      </div>

      <p class="section-description">
        Access tokens allow external applications to access the Deckle API on your behalf. Keep your
        tokens secure and never share them publicly.
      </p>

      {#if tokens.length === 0}
        <div class="empty-state">
          <p>No access tokens yet. Create one to get started.</p>
        </div>
      {:else}
        <div class="tokens-list">
          {#each tokens as token}
            <div class="token-card" class:revoked={!token.isValid}>
              <div class="token-header">
                <div class="token-info">
                  <h3 class="token-name">{token.name}</h3>
                  <span class="token-suffix">...{token.tokenSuffix}</span>
                  {#if !token.isValid}
                    <span class="token-badge revoked">Revoked</span>
                  {:else if token.expiresAt && new Date(token.expiresAt) <= new Date()}
                    <span class="token-badge expired">Expired</span>
                  {:else}
                    <span class="token-badge active">Active</span>
                  {/if}
                </div>
                <div class="token-actions">
                  {#if token.isValid}
                    <button class="btn-secondary btn-small" onclick={() => revokeToken(token.id)}>
                      Revoke
                    </button>
                  {/if}
                  <button class="btn-danger btn-small" onclick={() => deleteToken(token.id)}>
                    Delete
                  </button>
                </div>
              </div>
              {#if token.description}
                <p class="token-description">{token.description}</p>
              {/if}
              <div class="token-meta">
                <div class="meta-item">
                  <span class="meta-label">Created:</span>
                  <span class="meta-value">{formatDate(token.createdAt)}</span>
                </div>
                {#if token.lastUsedAt}
                  <div class="meta-item">
                    <span class="meta-label">Last used:</span>
                    <span class="meta-value">{formatDate(token.lastUsedAt)}</span>
                  </div>
                {/if}
                {#if token.expiresAt}
                  <div class="meta-item">
                    <span class="meta-label">Expires:</span>
                    <span class="meta-value">{formatDate(token.expiresAt)}</span>
                  </div>
                {/if}
              </div>
            </div>
          {/each}
        </div>
      {/if}
    </div>
  </div>
</PageLayout>

<!-- Create Token Dialog -->
{#if showCreateDialog}
  <div class="dialog-overlay" onclick={() => (showCreateDialog = false)}>
    <div class="dialog" onclick={(e) => e.stopPropagation()}>
      <div class="dialog-header">
        <h3>Create New Access Token</h3>
        <button class="dialog-close" onclick={() => (showCreateDialog = false)}>×</button>
      </div>
      <div class="dialog-body">
        {#if createError}
          <div class="error-message">{createError}</div>
        {/if}
        <div class="form-group">
          <label for="token-name">Token Name *</label>
          <input
            id="token-name"
            type="text"
            bind:value={newTokenName}
            placeholder="e.g., Production Server"
            maxlength="255"
            required
          />
        </div>
        <div class="form-group">
          <label for="token-description">Description (optional)</label>
          <textarea
            id="token-description"
            bind:value={newTokenDescription}
            placeholder="What will this token be used for?"
            maxlength="1000"
            rows="3"
          ></textarea>
        </div>
        <div class="form-group">
          <label for="token-expires">Expiration Date (optional)</label>
          <input id="token-expires" type="datetime-local" bind:value={newTokenExpiresAt} />
          <small>Leave empty for a token that never expires</small>
        </div>
      </div>
      <div class="dialog-footer">
        <button class="btn-secondary" onclick={() => (showCreateDialog = false)} disabled={isCreating}>
          Cancel
        </button>
        <button class="btn-primary" onclick={createToken} disabled={isCreating}>
          {isCreating ? 'Creating...' : 'Create Token'}
        </button>
      </div>
    </div>
  </div>
{/if}

<!-- Show Token Dialog (only shown once after creation) -->
{#if showTokenDialog && createdToken}
  <div class="dialog-overlay" onclick={() => (showTokenDialog = false)}>
    <div class="dialog" onclick={(e) => e.stopPropagation()}>
      <div class="dialog-header">
        <h3>Token Created Successfully</h3>
        <button class="dialog-close" onclick={() => (showTokenDialog = false)}>×</button>
      </div>
      <div class="dialog-body">
        <div class="warning-message">
          <strong>Important:</strong> This is the only time you'll see this token. Make sure to copy it
          now and store it securely.
        </div>
        <div class="token-display">
          <code>{createdToken.token}</code>
          <button class="btn-copy" onclick={copyToken}>Copy</button>
        </div>
      </div>
      <div class="dialog-footer">
        <button class="btn-primary" onclick={() => (showTokenDialog = false)}>I've Saved It</button>
      </div>
    </div>
  </div>
{/if}

<style>
  .header-text h1 {
    font-size: 1.875rem;
    font-weight: 700;
    color: white;
    margin-bottom: 0.25rem;
  }

  .subtitle {
    font-size: 0.9375rem;
    color: rgba(255, 255, 255, 0.9);
  }

  .settings-content {
    background-color: white;
    border-radius: 12px;
    border: 1px solid rgba(52, 73, 86, 0.1);
  }

  .settings-section {
    padding: 2rem;
  }

  .settings-section h2 {
    font-size: 1.25rem;
    font-weight: 600;
    color: var(--color-sage);
    margin: 0 0 1.5rem 0;
  }

  .profile-card {
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }

  .profile-avatar-section {
    display: flex;
    justify-content: center;
  }

  .profile-avatar {
    width: 120px;
    height: 120px;
    border-radius: 50%;
    object-fit: cover;
    border: 4px solid var(--color-sage);
  }

  .profile-avatar-placeholder {
    width: 120px;
    height: 120px;
    border-radius: 50%;
    background-color: var(--color-muted-teal);
    color: white;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 600;
    font-size: 3rem;
    border: 4px solid var(--color-sage);
  }

  .profile-fields {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
  }

  .field-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }

  .field-label {
    font-size: 0.875rem;
    font-weight: 600;
    color: var(--color-sage);
    text-transform: uppercase;
    letter-spacing: 0.025em;
  }

  .field-value {
    font-size: 1rem;
    color: var(--color-deep-forest);
    padding: 0.75rem 1rem;
    background-color: rgba(120, 160, 131, 0.05);
    border: 1px solid rgba(120, 160, 131, 0.2);
    border-radius: 8px;
  }

  @media (min-width: 640px) {
    .profile-card {
      flex-direction: row;
      align-items: flex-start;
      gap: 3rem;
    }

    .profile-avatar-section {
      flex-shrink: 0;
    }

    .profile-fields {
      flex: 1;
    }
  }

  /* MCP Tokens Section */
  .settings-section + .settings-section {
    border-top: 1px solid rgba(52, 73, 86, 0.1);
  }

  .section-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1rem;
  }

  .section-description {
    color: var(--color-sage);
    font-size: 0.875rem;
    margin-bottom: 1.5rem;
  }

  .empty-state {
    padding: 3rem;
    text-align: center;
    color: var(--color-sage);
    background-color: rgba(120, 160, 131, 0.05);
    border: 1px dashed rgba(120, 160, 131, 0.3);
    border-radius: 8px;
  }

  .tokens-list {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .token-card {
    padding: 1.5rem;
    background-color: rgba(120, 160, 131, 0.05);
    border: 1px solid rgba(120, 160, 131, 0.2);
    border-radius: 8px;
    transition: all 0.2s;
  }

  .token-card.revoked {
    opacity: 0.6;
    background-color: rgba(0, 0, 0, 0.02);
  }

  .token-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    gap: 1rem;
    margin-bottom: 0.75rem;
  }

  .token-info {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    flex-wrap: wrap;
  }

  .token-name {
    font-size: 1.125rem;
    font-weight: 600;
    color: var(--color-deep-forest);
    margin: 0;
  }

  .token-suffix {
    font-family: monospace;
    font-size: 0.875rem;
    color: var(--color-sage);
    background-color: rgba(120, 160, 131, 0.1);
    padding: 0.25rem 0.5rem;
    border-radius: 4px;
  }

  .token-badge {
    font-size: 0.75rem;
    font-weight: 600;
    padding: 0.25rem 0.5rem;
    border-radius: 4px;
    text-transform: uppercase;
  }

  .token-badge.active {
    background-color: #10b981;
    color: white;
  }

  .token-badge.revoked {
    background-color: #ef4444;
    color: white;
  }

  .token-badge.expired {
    background-color: #f59e0b;
    color: white;
  }

  .token-actions {
    display: flex;
    gap: 0.5rem;
  }

  .token-description {
    color: var(--color-sage);
    font-size: 0.875rem;
    margin: 0 0 1rem 0;
  }

  .token-meta {
    display: flex;
    flex-wrap: wrap;
    gap: 1.5rem;
    font-size: 0.875rem;
  }

  .meta-item {
    display: flex;
    gap: 0.5rem;
  }

  .meta-label {
    color: var(--color-sage);
    font-weight: 600;
  }

  .meta-value {
    color: var(--color-deep-forest);
  }

  /* Button Styles */
  .btn-primary {
    background-color: var(--color-sage);
    color: white;
    border: none;
    padding: 0.625rem 1.25rem;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s;
  }

  .btn-primary:hover {
    background-color: var(--color-deep-forest);
  }

  .btn-primary:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .btn-secondary {
    background-color: white;
    color: var(--color-sage);
    border: 1px solid var(--color-sage);
    padding: 0.625rem 1.25rem;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s;
  }

  .btn-secondary:hover {
    background-color: rgba(120, 160, 131, 0.1);
  }

  .btn-danger {
    background-color: #ef4444;
    color: white;
    border: none;
    padding: 0.625rem 1.25rem;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s;
  }

  .btn-danger:hover {
    background-color: #dc2626;
  }

  .btn-small {
    padding: 0.375rem 0.75rem;
    font-size: 0.8125rem;
  }

  /* Dialog Styles */
  .dialog-overlay {
    position: fixed;
    inset: 0;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
    padding: 1rem;
  }

  .dialog {
    background-color: white;
    border-radius: 12px;
    max-width: 500px;
    width: 100%;
    box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
  }

  .dialog-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1.5rem;
    border-bottom: 1px solid rgba(52, 73, 86, 0.1);
  }

  .dialog-header h3 {
    font-size: 1.25rem;
    font-weight: 600;
    color: var(--color-deep-forest);
    margin: 0;
  }

  .dialog-close {
    background: none;
    border: none;
    font-size: 1.5rem;
    color: var(--color-sage);
    cursor: pointer;
    padding: 0;
    width: 2rem;
    height: 2rem;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 4px;
    transition: background-color 0.2s;
  }

  .dialog-close:hover {
    background-color: rgba(120, 160, 131, 0.1);
  }

  .dialog-body {
    padding: 1.5rem;
  }

  .dialog-footer {
    display: flex;
    justify-content: flex-end;
    gap: 0.75rem;
    padding: 1.5rem;
    border-top: 1px solid rgba(52, 73, 86, 0.1);
  }

  .form-group {
    margin-bottom: 1.25rem;
  }

  .form-group:last-child {
    margin-bottom: 0;
  }

  .form-group label {
    display: block;
    font-size: 0.875rem;
    font-weight: 600;
    color: var(--color-deep-forest);
    margin-bottom: 0.5rem;
  }

  .form-group input,
  .form-group textarea {
    width: 100%;
    padding: 0.625rem;
    border: 1px solid rgba(120, 160, 131, 0.3);
    border-radius: 8px;
    font-size: 0.875rem;
    font-family: inherit;
  }

  .form-group input:focus,
  .form-group textarea:focus {
    outline: none;
    border-color: var(--color-sage);
  }

  .form-group small {
    display: block;
    margin-top: 0.25rem;
    font-size: 0.75rem;
    color: var(--color-sage);
  }

  .error-message {
    background-color: #fee2e2;
    color: #991b1b;
    padding: 0.75rem;
    border-radius: 8px;
    margin-bottom: 1rem;
    font-size: 0.875rem;
  }

  .warning-message {
    background-color: #fef3c7;
    color: #92400e;
    padding: 1rem;
    border-radius: 8px;
    margin-bottom: 1rem;
    font-size: 0.875rem;
  }

  .token-display {
    display: flex;
    gap: 0.5rem;
    align-items: center;
  }

  .token-display code {
    flex: 1;
    background-color: rgba(120, 160, 131, 0.1);
    padding: 0.75rem;
    border-radius: 8px;
    font-size: 0.875rem;
    word-break: break-all;
    font-family: monospace;
  }

  .btn-copy {
    background-color: var(--color-sage);
    color: white;
    border: none;
    padding: 0.75rem 1rem;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 600;
    cursor: pointer;
    white-space: nowrap;
  }

  .btn-copy:hover {
    background-color: var(--color-deep-forest);
  }
</style>
