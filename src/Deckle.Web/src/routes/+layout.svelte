<script lang="ts">
  import favicon from "$lib/assets/favicon.svg";
  import { page } from "$app/stores";
  import TopBar from "$lib/components/TopBar.svelte";
  import type { LayoutData } from "./$types";
  import "../app.css";

  let { children, data }: { children: any; data: LayoutData } = $props();

  // Determine if we should show the dashboard layout (topbar)
  const isAuthPage = $derived($page.url.pathname === "/" && !data.user);
</script>

<svelte:head>
  <link rel="icon" href={favicon} />
</svelte:head>

{#if isAuthPage}
  <!-- Landing page layout (no topbar) -->
  <main class="landing-content">
    {@render children()}
  </main>
{:else}
  <!-- Dashboard layout (topbar + content) -->
  {#if data.user}
    <TopBar user={data.user} />
  {/if}

  <div class="dashboard-layout" class:with-topbar={data.user}>
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

  .dashboard-layout.with-topbar {
    padding-top: 60px;
  }

  .main-content {
    flex: 1;
    background-color: #f8f9fa;
    display: flex;
    flex-direction: column;
  }
</style>
