<script lang="ts">
  import favicon from "$lib/assets/favicon.svg";
  import { page } from '$app/stores';
  import Sidebar from '$lib/components/Sidebar.svelte';
  import TopBar from '$lib/components/TopBar.svelte';
  import type { LayoutData } from './$types';
  import "../app.css";

  let { children, data }: { children: any, data: LayoutData } = $props();

  // Determine if we should show the dashboard layout (sidebar + topbar)
  const isAuthPage = $derived($page.url.pathname === '/' && !data.user);

  // Track sidebar collapsed state
  let sidebarCollapsed = $state(false);
</script>

<svelte:head>
  <link rel="icon" href={favicon} />
</svelte:head>

{#if isAuthPage}
  <!-- Landing page layout (no sidebar) -->
  <main class="landing-content">
    {@render children()}
  </main>
{:else}
  <!-- Dashboard layout (sidebar + topbar + content) -->
  {#if data.user}
    <Sidebar user={data.user} bind:collapsed={sidebarCollapsed} />
  {/if}

  <div class="dashboard-layout" class:with-sidebar={data.user} class:sidebar-collapsed={sidebarCollapsed}>
    {#if data.user}
      <TopBar user={data.user} />
    {/if}

    <main class="main-content">
      {@render children()}
    </main>
  </div>
{/if}

<style>
  .landing-content {
    min-height: 100vh;
  }

  .dashboard-layout {
    min-height: 100vh;
    display: flex;
    flex-direction: column;
  }

  .dashboard-layout.with-sidebar {
    margin-left: 260px;
    transition: margin-left 0.3s ease;
  }

  .dashboard-layout.with-sidebar.sidebar-collapsed {
    margin-left: 72px;
  }

  .main-content {
    flex: 1;
    background-color: #f8f9fa;
  }

  @media (max-width: 768px) {
    .dashboard-layout.with-sidebar {
      margin-left: 72px;
    }
  }
</style>
