<script lang="ts">
  import type { PageData } from './$types';

  let { data }: { data: PageData } = $props();
</script>

<svelte:head>
  <title>Edit Back Design · {data.component.name} · Deckle</title>
  <meta name="description" content="Design the back of {data.component.name}" />
</svelte:head>

<div class="design-editor">
  <div class="editor-header">
    <h1>Edit Back Design</h1>
    <p class="card-name">{data.component.name}</p>
  </div>

  <div class="placeholder">
    <div class="placeholder-content">
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
        <rect x="3" y="3" width="18" height="18" rx="2" ry="2" />
        <circle cx="8.5" cy="8.5" r="1.5" />
        <polyline points="21 15 16 10 5 21" />
      </svg>
      <h2>Back Design Editor</h2>
      <p>The card design editor will be available here soon.</p>
      <p class="placeholder-subtitle">
        You'll be able to create layouts, add fields from data sources, and customize the appearance of your cards.
      </p>
    </div>
  </div>
</div>

<style>
  .design-editor {
    max-width: 1400px;
    margin: 0 auto;
    padding: 2rem;
  }

  .editor-header {
    margin-bottom: 2rem;
  }

  h1 {
    font-size: 2rem;
    font-weight: 700;
    color: var(--color-sage);
    margin: 0 0 0.5rem 0;
  }

  .card-name {
    font-size: 1.125rem;
    color: var(--color-muted-teal);
    margin: 0;
  }

  .placeholder {
    background: white;
    border-radius: 12px;
    border: 2px dashed var(--color-teal-grey);
    min-height: 500px;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 3rem;
  }

  .placeholder-content {
    text-align: center;
    max-width: 500px;
  }

  .placeholder-content svg {
    color: var(--color-muted-teal);
    margin-bottom: 1.5rem;
  }

  .placeholder-content h2 {
    font-size: 1.5rem;
    font-weight: 600;
    color: var(--color-sage);
    margin: 0 0 1rem 0;
  }

  .placeholder-content p {
    font-size: 1rem;
    color: var(--color-muted-teal);
    margin: 0.5rem 0;
    line-height: 1.6;
  }

  .placeholder-subtitle {
    font-size: 0.875rem !important;
    color: var(--color-teal-grey) !important;
    margin-top: 1rem !important;
  }
</style>
