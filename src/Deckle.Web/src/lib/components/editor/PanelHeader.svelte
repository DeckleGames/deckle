<script lang="ts">
  import type { Snippet } from 'svelte';

  interface Props {
    title: string;
    actions?: Snippet;
  }

  let { title, actions }: Props = $props();
</script>

<div class="panel-header">
  <h2 class="panel-title">{title}</h2>
  {#if actions}
    <div class="panel-actions">
      {@render actions()}
    </div>
  {/if}
</div>

<style>
  .panel-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0.4rem 0.75rem;
    background: rgba(52, 73, 86, 0.03);
    border-bottom: 1px solid rgba(52, 73, 86, 0.08);
    flex-shrink: 0;
  }

  .panel-title {
    font-size: 0.6875rem;
    font-weight: 500;
    color: var(--color-muted-teal);
    margin: 0;
    text-transform: uppercase;
    letter-spacing: 0.75px;
    opacity: 0.8;
  }

  .panel-actions {
    display: flex;
    align-items: center;
    gap: 0.5rem;
  }
</style>
