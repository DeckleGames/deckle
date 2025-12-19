<script lang="ts">
  import type { Dimensions } from "$lib/types";

  let { dimensions }: { dimensions: Dimensions } = $props();
</script>

<div
  class="component"
  style:width={dimensions.widthPx + "px"}
  style:height={dimensions.heightPx + "px"}
>
  Component
</div>

<style>
  .component {
    background-color: #fff;
    overflow: hidden;
  }
</style>
