<script lang="ts">
  import type { PageData } from "./$types";
  import { setBreadcrumbs } from "$lib/stores/breadcrumb";
  import { buildEditorBreadcrumbs } from "$lib/utils/breadcrumbs";
  import ResizablePanelContainer from "$lib/components/ResizablePanelContainer.svelte";

  let { data }: { data: PageData } = $props();

  // Capitalize the part name for display (e.g., "front" -> "Front")
  const partLabel = data.part.charAt(0).toUpperCase() + data.part.slice(1);

  // Update breadcrumbs for this page
  $effect(() => {
    setBreadcrumbs(
      buildEditorBreadcrumbs(data.project, data.component, partLabel)
    );
  });
</script>

<svelte:head>
  <title>Edit {partLabel} Design · {data.component.name} · Deckle</title>
  <meta
    name="description"
    content="Design the {data.part} of {data.component.name}"
  />
</svelte:head>

<ResizablePanelContainer orientation="vertical" initialSplit={80}>
  {#snippet leftOrTop()}
    <ResizablePanelContainer initialSplit={20}>
      {#snippet leftOrTop()}
        Structure Tree
      {/snippet}
      {#snippet rightOrBottom()}
        <div class="editor-container">
          <p class="todo-message">
            TODO: Implement the component editor interface
          </p>
          <div class="component-info">
            <p><strong>Component:</strong> {data.component.name}</p>
            <p><strong>Part:</strong> {partLabel}</p>
            <p><strong>Project:</strong> {data.project.name}</p>
            <pre>{JSON.stringify(data.component, null, 2)}</pre>
          </div>
        </div>
      {/snippet}
    </ResizablePanelContainer>
  {/snippet}
  {#snippet rightOrBottom()}
    Data Source{/snippet}
</ResizablePanelContainer>

<style>
  .editor-container {
    width: 100%;
    height: 100%;
    background: repeating-conic-gradient(#e5e5e5 0 25%, #fff 0 50%) 50% / 8px
      8px;
    padding: 1rem; /*temporary*/
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
