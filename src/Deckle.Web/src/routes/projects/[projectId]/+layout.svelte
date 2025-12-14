<script lang="ts">
  import type { LayoutData } from "./$types";
  import Tabs from "$lib/components/Tabs.svelte";
  import Breadcrumb from "$lib/components/Breadcrumb.svelte";
  import { initBreadcrumbs } from "$lib/stores/breadcrumb";
  import { buildProjectBreadcrumbs } from "$lib/utils/breadcrumbs";

  let { data, children }: { data: LayoutData; children: any } = $props();

  const tabs = [
    { name: "Components", path: `/projects/${data.project.id}/components` },
    { name: "Data Sources", path: `/projects/${data.project.id}/data-sources` },
    {
      name: "Image Library",
      path: `/projects/${data.project.id}/image-library`,
    },
    {
      name: "Settings",
      path: `/projects/${data.project.id}/settings`,
    },
  ];

  // Initialize breadcrumbs context
  const breadcrumbs = initBreadcrumbs(
    buildProjectBreadcrumbs(data.project.id, data.project.name)
  );
</script>

<div class="project-page">
  <div class="project-header">
    <div class="header-content">
      <Breadcrumb items={$breadcrumbs} />
      {#if data.project.description}
        <p class="project-description">{data.project.description}</p>
      {/if}
    </div>
  </div>

  <Tabs {tabs} />

  <div class="page-content">
    {@render children()}
  </div>
</div>

<style>
  .project-page {
    flex: 1;
    display: flex;
    flex-direction: column;
  }

  .project-header {
    background: linear-gradient(
      135deg,
      var(--color-teal-grey) 0%,
      var(--color-muted-teal) 100%
    );
    padding: 1.2rem 2rem;
    border-bottom: 1px solid var(--color-border);
  }

  .project-description {
    color: rgba(255, 255, 255, 0.9);
    font-size: 0.875rem;
    line-height: 1.5;
    margin-top: 0.25rem;
  }

  .page-content {
    flex: 1;
    display: flex;
    flex-direction: column;
    overflow: auto;
  }

  @media (max-width: 768px) {
    .project-header {
      padding: 1rem;
    }
  }
</style>
