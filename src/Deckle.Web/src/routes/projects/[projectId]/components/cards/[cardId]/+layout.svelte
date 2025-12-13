<script lang="ts">
  import type { LayoutData } from './$types';

  let { children }: { children: any } = $props();
</script>

<!-- Full-screen layout for card editor -->
<div class="editor-layout">
  {@render children()}
</div>

<style>
  .editor-layout {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    overflow: hidden;
  }
</style>
