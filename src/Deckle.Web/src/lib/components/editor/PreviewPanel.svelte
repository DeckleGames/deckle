<script lang="ts">
  import PanelHeader from './PanelHeader.svelte';

  interface Props {
    html?: string;
    css?: string;
  }

  let { html = '', css = '' }: Props = $props();

  // Combine HTML and CSS for preview
  const previewContent = $derived(`
    <style>${css}</style>
    ${html}
  `);
</script>

<div class="preview-panel">
  <PanelHeader title="Preview" />

  <div class="preview-content">
    <div class="preview-container">
      <div class="preview-card">
        {@html previewContent}
      </div>
    </div>
  </div>
</div>

<style>
  .preview-panel {
    display: flex;
    flex-direction: column;
    height: 100%;
    background-color: var(--color-bg-secondary);
  }

  .preview-content {
    flex: 1;
    overflow: auto;
    padding: 2rem;
    background: linear-gradient(135deg, rgba(52, 73, 86, 0.05) 0%, rgba(80, 114, 123, 0.05) 100%);
  }

  .preview-container {
    display: flex;
    align-items: center;
    justify-content: center;
    min-height: 100%;
  }

  .preview-card {
    background: white;
    border-radius: 8px;
    box-shadow: var(--shadow-lg);
    padding: 2rem;
    min-width: 300px;
    max-width: 500px;
  }

  /* Default card styling if no custom styles are provided */
  .preview-card :global(p) {
    margin: 0.5rem 0;
  }

  .preview-card :global(h1),
  .preview-card :global(h2),
  .preview-card :global(h3) {
    margin: 0.75rem 0;
    color: var(--color-dark);
  }
</style>
