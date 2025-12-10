<script lang="ts">
  import { page } from "$app/stores";
  import type { LayoutData } from "./$types";

  let { data, children }: { data: LayoutData; children: any } = $props();

  const tabs = [
    { name: "Components", path: `/projects/${data.project.id}/components` },
    { name: "Data Sources", path: `/projects/${data.project.id}/data-sources` },
    {
      name: "Image Library",
      path: `/projects/${data.project.id}/image-library`,
    },
  ];

  function isActiveTab(tabPath: string): boolean {
    return $page.url.pathname === tabPath;
  }
</script>

<div class="project-page">
  <div class="project-header">
    <div class="header-content">
      <div class="breadcrumb">
        <a href="/projects">Projects</a>
        <svg viewBox="0 0 20 20" fill="currentColor">
          <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd" />
        </svg>
        <span class="current">{data.project.name}</span>
      </div>
      {#if data.project.description}
        <p class="project-description">{data.project.description}</p>
      {/if}
    </div>
  </div>

  <div class="tabs-bar">
    <div class="tabs-content">
      {#each tabs as tab}
        <a
          href={tab.path}
          class="tab-link"
          class:active={isActiveTab(tab.path)}
        >
          {tab.name}
        </a>
      {/each}
    </div>
  </div>

  <div class="page-content">
    {@render children()}
  </div>
</div>

<style>
  .project-page {
    min-height: 100%;
  }

  .project-header {
    background: linear-gradient(135deg, var(--color-teal-grey) 0%, var(--color-muted-teal) 100%);
    padding: 1.5rem 2rem;
    border-bottom: 1px solid var(--color-border);
  }

  .header-content {
    max-width: 1600px;
    margin: 0 auto;
  }

  .breadcrumb {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-size: 0.875rem;
    margin-bottom: 0.5rem;
  }

  .breadcrumb a {
    color: rgba(255, 255, 255, 0.8);
    text-decoration: none;
    transition: color 0.2s ease;
  }

  .breadcrumb a:hover {
    color: white;
  }

  .breadcrumb svg {
    width: 14px;
    height: 14px;
    color: rgba(255, 255, 255, 0.6);
    flex-shrink: 0;
  }

  .breadcrumb .current {
    color: white;
    font-weight: 600;
  }

  .project-description {
    color: rgba(255, 255, 255, 0.9);
    font-size: 0.875rem;
    line-height: 1.5;
    margin-top: 0.25rem;
  }

  .tabs-bar {
    background: white;
    border-bottom: 1px solid var(--color-border);
    position: sticky;
    top: 64px;
    z-index: 50;
  }

  .tabs-content {
    max-width: 1600px;
    margin: 0 auto;
    padding: 0 2rem;
    display: flex;
    gap: 0.25rem;
  }

  .tab-link {
    position: relative;
    display: block;
    padding: 1rem 1.25rem;
    color: var(--color-text-secondary);
    text-decoration: none;
    font-weight: 500;
    font-size: 0.9375rem;
    transition: all 0.2s ease;
    border-bottom: 2px solid transparent;
  }

  .tab-link:hover {
    color: var(--color-sage);
    background-color: rgba(120, 160, 131, 0.05);
  }

  .tab-link.active {
    color: var(--color-sage);
    font-weight: 600;
    border-bottom-color: var(--color-sage);
  }

  .page-content {
    padding: 2rem;
    max-width: 1600px;
    margin: 0 auto;
  }

  @media (max-width: 768px) {
    .project-header {
      padding: 1rem;
    }

    .tabs-content {
      padding: 0 1rem;
      overflow-x: auto;
    }

    .tab-link {
      white-space: nowrap;
      padding: 0.875rem 1rem;
    }

    .page-content {
      padding: 1.5rem 1rem;
    }
  }
</style>
