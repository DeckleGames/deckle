<script lang="ts">
  import type { Dimensions } from "$lib/types";
  import type { Snippet } from "svelte";
  import Panzoom, { type PanzoomObject } from "@panzoom/panzoom";

  let {
    dimensions,
    zoom = $bindable(100),
    children,
  }: { dimensions: Dimensions; zoom?: number; children: Snippet } = $props();

  let containerElement: HTMLElement;
  let contentElement: HTMLElement;
  let panzoomInstance: PanzoomObject | null = null;

  // Initialize panzoom when the component mounts
  $effect(() => {
    if (!contentElement) return;

    panzoomInstance = Panzoom(contentElement, {
      maxScale: 10,
      minScale: 0.1,
      step: 0.1,
      startScale: zoom / 100,
      canvas: true,
      contain: 'outside',
    });

    // Enable wheel zoom with Ctrl/Cmd modifier
    contentElement.parentElement?.addEventListener('wheel', panzoomInstance.zoomWithWheel);

    return () => {
      if (panzoomInstance) {
        contentElement.parentElement?.removeEventListener('wheel', panzoomInstance.zoomWithWheel);
        panzoomInstance.destroy();
      }
    };
  });

  // Sync external zoom changes to panzoom
  $effect(() => {
    if (panzoomInstance && zoom !== undefined) {
      const currentScale = panzoomInstance.getScale();
      const targetScale = zoom / 100;

      // Only update if there's a meaningful difference (avoid feedback loops)
      if (Math.abs(currentScale - targetScale) > 0.01) {
        panzoomInstance.zoom(targetScale, { animate: false });
      }
    }
  });

  // Sync panzoom changes back to zoom prop
  $effect(() => {
    if (!contentElement) return;

    const handleZoom = (event: CustomEvent) => {
      const newScale = panzoomInstance?.getScale() || 1;
      zoom = Math.round(newScale * 100);
    };

    contentElement.addEventListener('panzoomchange', handleZoom as EventListener);

    return () => {
      contentElement.removeEventListener('panzoomchange', handleZoom as EventListener);
    };
  });
</script>

<div
  class="component-viewer"
  bind:this={containerElement}
>
  <div class="panzoom-container" bind:this={contentElement}>
    {@render children()}
  </div>
</div>

<style>
  .component-viewer {
    background: repeating-conic-gradient(#e5e5e5 0 25%, #fff 0 50%) 50% / 8px
      8px;
    height: 100%;
    width: 100%;
    overflow: hidden;
    position: relative;
  }

  .panzoom-container {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    cursor: grab;
  }

  .panzoom-container:active {
    cursor: grabbing;
  }
</style>
