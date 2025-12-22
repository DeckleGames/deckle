<script lang="ts">
  import type { Dimensions } from "$lib/types";
  import { templateStore } from "$lib/stores/templateElements";
  import TemplateRenderer from "./TemplateRenderer.svelte";

  let { dimensions }: { dimensions: Dimensions } = $props();
</script>

<div
  class="component"
  style:width={dimensions.widthPx + "px"}
  style:height={dimensions.heightPx + "px"}
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
</style>
