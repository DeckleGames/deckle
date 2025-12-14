<script lang="ts">
	import type { PageData } from './$types';
	import DesignEditor from '$lib/components/editor/DesignEditor.svelte';
	import { getBreadcrumbs } from '$lib/stores/breadcrumb';
	import { buildCardEditorBreadcrumbs } from '$lib/utils/breadcrumbs';

	let { data }: { data: PageData } = $props();

	// Capitalize the part name for display (e.g., "front" -> "Front")
	const partLabel = data.part.charAt(0).toUpperCase() + data.part.slice(1);

	// Update breadcrumbs for this page
	const breadcrumbs = getBreadcrumbs();
	$effect(() => {
		breadcrumbs.set(
			buildCardEditorBreadcrumbs(
				data.project.id,
				data.project.name,
				data.component.id,
				data.component.name,
				partLabel as 'Front' | 'Back'
			)
		);
	});

	// Placeholder HTML/CSS - will be loaded from API later
	const initialHtml = `<div class="card">
  <p>sample card preview</p>
</div>`;

	const initialCss = `.card {
  font-family: sans-serif;
}`;

	function handleSave(html: string, css: string) {
		console.log(`Saving ${data.part} design:`, { html, css });
		// TODO: Implement API call to save design
	}
</script>

<svelte:head>
	<title>Edit {partLabel} Design · {data.component.name} · Deckle</title>
	<meta name="description" content="Design the {data.part} of {data.component.name}" />
</svelte:head>

<DesignEditor
	componentId={data.component.id}
	projectId={data.project.id}
	designType={data.part}
	{initialHtml}
	{initialCss}
	onSave={handleSave}
/>
