<script lang="ts">
  import type { Dimensions } from "$lib/types";
  import type { Snippet } from "svelte";

  let { dimensions, children }: { dimensions: Dimensions; children: Snippet } =
    $props();

  let containerWidth = $state(0);
  let containerHeight = $state(0);

  const scale = $derived(() => {
    if (!containerWidth || !containerHeight) return 1;

    const padding = 40; // padding in pixels
    const availableWidth = containerWidth - padding * 2;
    const availableHeight = containerHeight - padding * 2;

    const scaleX = availableWidth / dimensions.widthPx;
    const scaleY = availableHeight / dimensions.heightPx;

    // Use the smaller scale to ensure it fits both dimensions
    return Math.min(scaleX, scaleY, 1); // Don't scale up beyond 100%
  });
</script>

<div
  class="component-viewer"
  bind:clientWidth={containerWidth}
  bind:clientHeight={containerHeight}
>
  <div class="scaler" style:transform="scale({scale()})">
    {@render children()}
  </div>
</div>

<style>
  .component-viewer {
    background: repeating-conic-gradient(#e5e5e5 0 25%, #fff 0 50%) 50% / 8px
      8px;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .scaler {
    transform-origin: center center;
    display: block;
  }
</style>
