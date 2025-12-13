<script lang="ts">
  import { onMount } from "svelte";
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
  let hasUnsavedChanges = $state(false);

  // Sync initial values
  $effect(() => {
    html = initialHtml;
  });

  $effect(() => {
    css = initialCss;
  });

  function handleHtmlChange(value: string) {
    html = value;
    hasUnsavedChanges = true;
  }

  function handleCssChange(value: string) {
    css = value;
    hasUnsavedChanges = true;
  }

  function handleHorizontalResize(splitPercentage: number) {
    layout.splitSizes.horizontal = [splitPercentage, 100 - splitPercentage];
    savePanelLayout(layout);
  }

  function handleVerticalResize(splitPercentage: number) {
    layout.splitSizes.vertical = [splitPercentage, 100 - splitPercentage];
    savePanelLayout(layout);
  }

  function handleSave() {
    if (onSave) {
      onSave(html, css);
      hasUnsavedChanges = false;
    }
  }

  function handleKeyDown(e: KeyboardEvent) {
    // Ctrl/Cmd + S to save
    if ((e.ctrlKey || e.metaKey) && e.key === "s") {
      e.preventDefault();
      handleSave();
    }
  }

  onMount(() => {
    window.addEventListener("keydown", handleKeyDown);
    return () => {
      window.removeEventListener("keydown", handleKeyDown);
    };
  });
</script>

<div class="design-editor">
  <div class="editor-toolbar">
    <div class="toolbar-left">
      <h1 class="editor-title">
        {designType === "front" ? "Front" : "Back"} Design
      </h1>
    </div>
    <div class="toolbar-right">
      {#if hasUnsavedChanges}
        <span class="unsaved-indicator">Unsaved changes</span>
      {/if}
      <button
        class="save-btn"
        onclick={handleSave}
        disabled={!hasUnsavedChanges}
      >
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
          <path
            d="M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z"
          />
          <polyline points="17 21 17 13 7 13 7 21" />
          <polyline points="7 3 7 8 15 8" />
        </svg>
        Save
      </button>
    </div>
  </div>

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

  .editor-toolbar {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 1rem 1.5rem;
    background: linear-gradient(
      135deg,
      var(--color-dark) 0%,
      var(--color-teal-grey) 100%
    );
    border-bottom: 1px solid var(--color-border);
    flex-shrink: 0;
  }

  .toolbar-left {
    display: flex;
    align-items: center;
    gap: 1rem;
  }

  .editor-title {
    font-size: 1.25rem;
    font-weight: 600;
    color: var(--color-sage);
    margin: 0;
  }

  .toolbar-right {
    display: flex;
    align-items: center;
    gap: 1rem;
  }

  .unsaved-indicator {
    font-size: 0.875rem;
    color: var(--color-muted-teal);
    font-style: italic;
  }

  .save-btn {
    background-color: var(--color-sage);
    color: white;
    border: none;
    padding: 0.625rem 1.25rem;
    border-radius: var(--radius-sm);
    font-size: 0.875rem;
    font-weight: 500;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    transition: all 0.2s ease;
  }

  .save-btn:hover:not(:disabled) {
    background-color: var(--color-muted-teal);
    transform: translateY(-1px);
    box-shadow: var(--shadow-md);
  }

  .save-btn:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .editor-container {
    flex: 1;
    overflow: hidden;
  }
</style>
