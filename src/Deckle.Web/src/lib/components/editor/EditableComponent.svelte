<script lang="ts">
  import type { Dimensions } from "$lib/types";
  import { templateStore } from "$lib/stores/templateElements";
  import TemplateRenderer from "./TemplateRenderer.svelte";

  let { dimensions, showBleedSafeArea = false }: { dimensions: Dimensions; showBleedSafeArea?: boolean } = $props();
</script>

<div
  class="component"
  class:show-bleed-safe-area={showBleedSafeArea}
  style:width={dimensions.widthPx + (2 * dimensions.bleedPx) + "px"}
  style:height={dimensions.heightPx + (2 * dimensions.bleedPx) + "px"}
  style:--bleed-px={dimensions.bleedPx + "px"}
  style:--width-px={dimensions.widthPx + "px"}
  style:--height-px={dimensions.heightPx + "px"}
>
  {#each $templateStore.root.children as child (child.id)}
    <TemplateRenderer element={child} />
  {/each}
</div>

<style>
  .component {
    background-color: #fff;
    overflow: hidden;
    position: relative;
  }

  .component.show-bleed-safe-area::before {
    content: '';
    position: absolute;
    top: calc(var(--bleed-px) * 2);
    left: calc(var(--bleed-px) * 2);
    width: calc(var(--width-px) - var(--bleed-px) * 2);
    height: calc(var(--height-px) - var(--bleed-px) * 2);
    border: 1px dotted green;
    pointer-events: none;
    z-index: 1;
  }

  .component.show-bleed-safe-area::after {
    content: '';
    position: absolute;
    top: var(--bleed-px);
    left: var(--bleed-px);
    width: var(--width-px);
    height: var(--height-px);
    border: 1px solid red;
    pointer-events: none;
    z-index: 1;
  }
</style>
