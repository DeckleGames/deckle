<script lang="ts">
  import type { PageData } from "./$types";
  import { setBreadcrumbs } from "$lib/stores/breadcrumb";
  import { buildEditorBreadcrumbs } from "$lib/utils/breadcrumbs";
  import ResizablePanelContainer from "$lib/components/ResizablePanelContainer.svelte";
  import PreviewPanel from "./panels/PreviewPanel.svelte";

  let { data }: { data: PageData } = $props();

  // Capitalize the part name for display (e.g., "front" -> "Front")
  const partLabel = data.part.charAt(0).toUpperCase() + data.part.slice(1);

  // Update breadcrumbs for this page
  $effect(() => {
    setBreadcrumbs(
      buildEditorBreadcrumbs(data.project, data.component, partLabel)
    );
  });

  const sidebarWidth = 20;
  // todo: move editor component into $/components/editor
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
    <ResizablePanelContainer initialSplit={sidebarWidth}>
      {#snippet leftOrTop()}
        Structure Tree
      {/snippet}
      {#snippet rightOrBottom()}
        <ResizablePanelContainer
          initialSplit={100 - (sidebarWidth / (100 - sidebarWidth)) * 100}
        >
          {#snippet leftOrTop()}
            <PreviewPanel {data} {partLabel} />
          {/snippet}
          {#snippet rightOrBottom()}
            Config
          {/snippet}
        </ResizablePanelContainer>
      {/snippet}
    </ResizablePanelContainer>
  {/snippet}
  {#snippet rightOrBottom()}
    Data Source{/snippet}
</ResizablePanelContainer>
