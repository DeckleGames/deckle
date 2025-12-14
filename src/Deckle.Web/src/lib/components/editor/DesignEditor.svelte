<script lang="ts">
  import ResizablePanelContainer from "./ResizablePanelContainer.svelte";
  import PreviewPanel from "./PreviewPanel.svelte";
  import EditorPanel from "./EditorPanel.svelte";
  import DataSourcePanel from "./DataSourcePanel.svelte";
  import { loadPanelLayout, savePanelLayout } from "$lib/utils/panelStorage";
  import type { PanelLayout, DataSource } from "$lib/types";

  interface Props {
    componentId: string;
    projectId: string;
    designType: "front" | "back";
    initialHtml?: string;
    initialCss?: string;
    dataSource?: DataSource;
    onSave?: (html: string, css: string) => void;
  }

  let {
    componentId,
    projectId,
    designType,
    initialHtml = "",
    initialCss = "",
    dataSource,
    onSave,
  }: Props = $props();

  let layout = $state<PanelLayout>(loadPanelLayout());
  let html = $state(initialHtml);
  let css = $state(initialCss);

  // Sync initial values
  $effect(() => {
    html = initialHtml;
  });

  $effect(() => {
    css = initialCss;
  });

  function handleHtmlChange(value: string) {
    html = value;
  }

  function handleCssChange(value: string) {
    css = value;
  }

  function handleHorizontalResize(splitPercentage: number) {
    layout.splitSizes.horizontal = [splitPercentage, 100 - splitPercentage];
    savePanelLayout(layout);
  }

  function handleVerticalResize(splitPercentage: number) {
    layout.splitSizes.vertical = [splitPercentage, 100 - splitPercentage];
    savePanelLayout(layout);
  }
</script>

<div class="design-editor">
  <div class="editor-container">
    <ResizablePanelContainer
      orientation="vertical"
      initialSplit={layout.splitSizes.vertical?.[0] ?? 70}
      minSize={40}
      maxSize={85}
      onResize={handleVerticalResize}
    >
      {#snippet leftOrTop()}
        <ResizablePanelContainer
          orientation="horizontal"
          initialSplit={layout.splitSizes.horizontal?.[0] ?? 35}
          minSize={20}
          maxSize={60}
          onResize={handleHorizontalResize}
        >
          {#snippet leftOrTop()}
            <PreviewPanel {html} {css} />
          {/snippet}

          {#snippet rightOrBottom()}
            <EditorPanel
              {html}
              {css}
              onHtmlChange={handleHtmlChange}
              onCssChange={handleCssChange}
            />
          {/snippet}
        </ResizablePanelContainer>
      {/snippet}

      {#snippet rightOrBottom()}
        <DataSourcePanel {dataSource} {componentId} {projectId} />
      {/snippet}
    </ResizablePanelContainer>
  </div>
</div>

<style>
  .design-editor {
    display: flex;
    flex-direction: column;
    width: 100%;
    height: 100vh;
    background-color: var(--color-bg-primary);
    margin: 0;
    padding: 0;
  }

  .editor-container {
    flex: 1;
    overflow: hidden;
  }
</style>
