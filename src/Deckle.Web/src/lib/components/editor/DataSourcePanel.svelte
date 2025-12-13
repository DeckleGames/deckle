<script lang="ts">
  import PanelHeader from './PanelHeader.svelte';
  import type { DataSource } from '$lib/types';

  interface Props {
    dataSource?: DataSource;
    componentId?: string;
    projectId?: string;
  }

  let { dataSource, componentId, projectId }: Props = $props();
</script>

<div class="data-source-panel">
  <PanelHeader title="Linked Data Source">
    {#snippet actions()}
      {#if dataSource}
        <button class="action-btn" title="Refresh data">
          <svg
            width="16"
            height="16"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <polyline points="23 4 23 10 17 10" />
            <polyline points="1 20 1 14 7 14" />
            <path d="M3.51 9a9 9 0 0 1 14.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0 0 20.49 15" />
          </svg>
        </button>
        <button class="action-btn" title="Unlink data source">
          <svg
            width="16"
            height="16"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <line x1="18" y1="6" x2="6" y2="18" />
            <line x1="6" y1="6" x2="18" y2="18" />
          </svg>
        </button>
      {:else}
        <button class="link-btn">
          <svg
            width="16"
            height="16"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <path d="M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71" />
            <path d="M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71" />
          </svg>
          Link Data Source
        </button>
      {/if}
    {/snippet}
  </PanelHeader>

  <div class="panel-content">
    {#if dataSource}
      <div class="data-source-info">
        <div class="info-row">
          <span class="info-label">Name:</span>
          <span class="info-value">{dataSource.name}</span>
        </div>
        <div class="info-row">
          <span class="info-label">Type:</span>
          <span class="info-value">{dataSource.type}</span>
        </div>
      </div>

      <div class="spreadsheet-container">
        <div class="placeholder-content">
          <svg
            width="48"
            height="48"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <rect x="3" y="3" width="18" height="18" rx="2" ry="2" />
            <line x1="3" y1="9" x2="21" y2="9" />
            <line x1="3" y1="15" x2="21" y2="15" />
            <line x1="9" y1="3" x2="9" y2="21" />
            <line x1="15" y1="3" x2="15" y2="21" />
          </svg>
          <p>Spreadsheet editor coming soon</p>
        </div>
      </div>
    {:else}
      <div class="empty-state">
        <svg
          width="64"
          height="64"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        >
          <circle cx="12" cy="12" r="10" />
          <line x1="12" y1="16" x2="12" y2="12" />
          <line x1="12" y1="8" x2="12.01" y2="8" />
        </svg>
        <h3>No Data Source Linked</h3>
        <p>You have not yet linked a data source to this component</p>
        <button class="link-btn primary">
          <svg
            width="16"
            height="16"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <path d="M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71" />
            <path d="M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71" />
          </svg>
          Link a Data Source
        </button>
      </div>
    {/if}
  </div>
</div>

<style>
  .data-source-panel {
    display: flex;
    flex-direction: column;
    height: 100%;
    background-color: var(--color-bg-secondary);
  }

  .panel-content {
    flex: 1;
    overflow: auto;
    padding: 1.5rem;
  }

  .data-source-info {
    background-color: rgba(120, 160, 131, 0.05);
    border-radius: var(--radius-md);
    padding: 1rem;
    margin-bottom: 1rem;
  }

  .info-row {
    display: flex;
    gap: 0.5rem;
    padding: 0.25rem 0;
  }

  .info-label {
    font-weight: 600;
    color: var(--color-muted-teal);
    min-width: 80px;
  }

  .info-value {
    color: var(--color-dark);
  }

  .spreadsheet-container {
    background-color: white;
    border: 1px solid var(--color-border);
    border-radius: var(--radius-md);
    min-height: 300px;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .placeholder-content {
    text-align: center;
    color: var(--color-muted-teal);
  }

  .placeholder-content svg {
    margin-bottom: 1rem;
  }

  .placeholder-content p {
    margin: 0;
    font-size: 0.875rem;
  }

  .empty-state {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    text-align: center;
    min-height: 200px;
    padding: 2rem;
  }

  .empty-state svg {
    color: var(--color-muted-teal);
    margin-bottom: 1.5rem;
  }

  .empty-state h3 {
    font-size: 1.25rem;
    font-weight: 600;
    color: var(--color-dark);
    margin: 0 0 0.5rem 0;
  }

  .empty-state p {
    font-size: 0.875rem;
    color: var(--color-muted-teal);
    margin: 0 0 1.5rem 0;
  }

  .action-btn {
    background: transparent;
    border: none;
    color: var(--color-sage);
    padding: 0.25rem;
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: var(--radius-sm);
    transition: background-color 0.2s ease;
  }

  .action-btn:hover {
    background-color: rgba(120, 160, 131, 0.1);
  }

  .link-btn {
    background-color: var(--color-sage);
    color: white;
    border: none;
    padding: 0.5rem 1rem;
    border-radius: var(--radius-sm);
    font-size: 0.875rem;
    font-weight: 500;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    transition: all 0.2s ease;
  }

  .link-btn:hover {
    background-color: var(--color-muted-teal);
    transform: translateY(-1px);
    box-shadow: var(--shadow-md);
  }

  .link-btn.primary {
    padding: 0.75rem 1.5rem;
    font-size: 1rem;
  }
</style>
