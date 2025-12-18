<script lang="ts">
  import ComponentViewer from "$lib/components/editor/ComponentViewer.svelte";
  import type { PageData } from "../$types";

  let { data, partLabel }: { data: PageData; partLabel: string } = $props();
  let dim = data.component.dimensions;
</script>

<ComponentViewer dimensions={dim}>
  <div
    class="card"
    style:width={dim.widthPx + "px"}
    style:height={dim.heightPx + "px"}
  >
    Card {partLabel}
  </div>
  <!-- <p class="todo-message">TODO: Implement the component editor interface</p>
  <div class="component-info">
    <p><strong>Component:</strong> {data.component.name}</p>
    <p><strong>Part:</strong> {partLabel}</p>
    <pre>{JSON.stringify(data.component, null, 2)}</pre>
  </div> -->
</ComponentViewer>

<style>
  .editor-container {
    background: repeating-conic-gradient(#e5e5e5 0 25%, #fff 0 50%) 50% / 8px
      8px;
    padding: 1rem; /*temporary*/
    height: 100%;
  }

  .card {
    background-color: #fff;
    overflow: hidden;
  }

  h1 {
    font-size: 2rem;
    font-weight: 600;
    color: var(--color-text-primary);
    margin-bottom: 1rem;
  }

  .todo-message {
    font-size: 1.125rem;
    color: var(--color-text-secondary);
    margin-bottom: 2rem;
    padding: 1rem;
    background-color: var(--color-bg-secondary);
    border-radius: 8px;
    border-left: 4px solid var(--color-sage);
  }

  .component-info {
    text-align: left;
    background-color: var(--color-bg-secondary);
    padding: 1.5rem;
    border-radius: 8px;
  }

  .component-info p {
    margin: 0.5rem 0;
    color: var(--color-text-secondary);
  }

  .component-info strong {
    color: var(--color-text-primary);
  }
</style>
