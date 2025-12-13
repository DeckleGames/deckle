<script lang="ts">
  import type { PageData } from './$types';
  import DesignEditor from '$lib/components/editor/DesignEditor.svelte';

  let { data }: { data: PageData } = $props();

  // Placeholder HTML/CSS - will be loaded from API later
  const initialHtml = `<div class="card">
  <p>sample card preview</p>
</div>`;

  const initialCss = `.card {
  font-family: sans-serif;
}`;

  function handleSave(html: string, css: string) {
    console.log('Saving front design:', { html, css });
    // TODO: Implement API call to save design
  }
</script>

<svelte:head>
  <title>Edit Front Design · {data.component.name} · Deckle</title>
  <meta name="description" content="Design the front of {data.component.name}" />
</svelte:head>

<DesignEditor
  componentId={data.component.id}
  projectId={data.project.id}
  designType="front"
  {initialHtml}
  {initialCss}
  onSave={handleSave}
/>
