<script lang="ts">
  import ComponentViewer from "$lib/components/editor/ComponentViewer.svelte";
  import Panel from "$lib/components/editor/Panel.svelte";
  import ZoomControls from "$lib/components/editor/ZoomControls.svelte";
  import type { PageData } from "../../../../routes/projects/[projectId]/components/[componentId]/[part]/$types";
  import EditableComponent from "../EditableComponent.svelte";

  let { data }: { data: PageData } = $props();
  let dimensions = data.component.dimensions;

  let zoom = $state(100);
</script>

<Panel title="Preview">
  {#snippet toolbar()}
    <ZoomControls bind:zoom />
  {/snippet}
  <ComponentViewer {dimensions} {zoom}>
    <EditableComponent {dimensions} />
  </ComponentViewer>
</Panel>

<style>
</style>
