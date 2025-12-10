<script lang="ts">
  import { config } from '$lib/config';
  import type { PageData } from './$types';
  import Card from '$lib/components/Card.svelte';

  let { data }: { data: PageData } = $props();

  const projectCount = $derived(data.projects.length);

  let showCreateDialog = $state(false);
  let projectName = $state('');
  let projectDescription = $state('');
  let isCreating = $state(false);

  async function createProject() {
    if (!projectName.trim()) return;

    isCreating = true;
    try {
      const response = await fetch(`${config.apiUrl}/projects`, {
        method: 'POST',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          name: projectName,
          description: projectDescription
        })
      });

      if (response.ok) {
        window.location.reload();
      } else {
        alert('Failed to create project. Please try again.');
      }
    } catch (error) {
      console.error('Failed to create project:', error);
      alert('Failed to create project. Please try again.');
    } finally {
      isCreating = false;
    }
  }

  function closeDialog() {
    showCreateDialog = false;
    projectName = '';
    projectDescription = '';
  }
</script>

<svelte:head>
  <title>Projects · Deckle</title>
  <meta name="description" content="Manage your game design projects. Create and organize game components, data sources, and image libraries for your tabletop games." />
</svelte:head>

<div class="page">
  <div class="page-header">
    <div class="header-content">
      <div class="header-text">
        <h1>Projects</h1>
        <p class="subtitle">Manage your game design projects</p>
      </div>
      <button class="create-button" onclick={() => (showCreateDialog = true)}>
        <svg viewBox="0 0 20 20" fill="currentColor">
          <path fill-rule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clip-rule="evenodd" />
        </svg>
        New Project
      </button>
    </div>
  </div>

  <div class="page-content">
    {#if data.projects.length === 0}
      <div class="empty-state">
        <svg viewBox="0 0 20 20" fill="currentColor">
          <path d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
        </svg>
        <p class="empty-message">No projects yet</p>
        <p class="empty-subtitle">Create your first project to get started</p>
        <button class="empty-action" onclick={() => (showCreateDialog = true)}>
          Create Project
        </button>
      </div>
    {:else}
      <div class="projects-grid">
        {#each data.projects as project}
          <Card href="/projects/{project.id}">
            <div class="project-header">
              <h2>{project.name}</h2>
              <span class="role-badge">{project.role}</span>
            </div>
            {#if project.description}
              <p class="project-description">{project.description}</p>
            {/if}
            <div class="project-footer">
              <span class="project-date">
                Created {new Date(project.createdAt).toLocaleDateString()}
              </span>
            </div>
          </Card>
        {/each}
      </div>
    {/if}
  </div>
</div>

{#if showCreateDialog}
  <div class="dialog-overlay" onclick={closeDialog}>
    <div class="dialog" onclick={(e) => e.stopPropagation()}>
      <h2>Create New Project</h2>
      <form onsubmit={(e) => { e.preventDefault(); createProject(); }}>
        <div class="form-group">
          <label for="name">Project Name</label>
          <input
            id="name"
            type="text"
            bind:value={projectName}
            required
            autofocus
            placeholder="My Game Project"
          />
        </div>
        <div class="form-group">
          <label for="description">Description (optional)</label>
          <textarea
            id="description"
            bind:value={projectDescription}
            rows="3"
            placeholder="A brief description of your game..."
          ></textarea>
        </div>
        <div class="dialog-actions">
          <button type="button" class="secondary" onclick={closeDialog}>Cancel</button>
          <button type="submit" class="primary" disabled={isCreating || !projectName.trim()}>
            {isCreating ? 'Creating...' : 'Create Project'}
          </button>
        </div>
      </form>
    </div>
  </div>
{/if}

<style>
  .page {
    min-height: 100%;
  }

  .page-header {
    background: linear-gradient(135deg, var(--color-teal-grey) 0%, var(--color-muted-teal) 100%);
    padding: 2rem;
    border-bottom: 1px solid var(--color-border);
  }

  .header-content {
    max-width: 1600px;
    margin: 0 auto;
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 2rem;
  }

  .header-text h1 {
    font-size: 1.875rem;
    font-weight: 700;
    color: white;
    margin-bottom: 0.25rem;
  }

  .subtitle {
    font-size: 0.9375rem;
    color: rgba(255, 255, 255, 0.9);
  }

  .create-button {
    background-color: white;
    color: var(--color-sage);
    border: none;
    padding: 0.75rem 1.5rem;
    font-size: 0.9375rem;
    font-weight: 600;
    border-radius: var(--radius-md);
    cursor: pointer;
    transition: all 0.2s ease;
    display: flex;
    align-items: center;
    gap: 0.5rem;
    white-space: nowrap;
  }

  .create-button svg {
    width: 18px;
    height: 18px;
  }

  .create-button:hover {
    background-color: rgba(255, 255, 255, 0.95);
    transform: translateY(-2px);
    box-shadow: var(--shadow-md);
  }

  .page-content {
    padding: 2rem;
    max-width: 1600px;
    margin: 0 auto;
  }

  .empty-state {
    background: white;
    border: 2px dashed var(--color-border);
    border-radius: var(--radius-lg);
    padding: 4rem 2rem;
    text-align: center;
  }

  .empty-state svg {
    width: 48px;
    height: 48px;
    color: var(--color-muted-teal);
    margin: 0 auto 1rem;
    opacity: 0.5;
  }

  .empty-message {
    font-size: 1.25rem;
    font-weight: 600;
    color: var(--color-sage);
    margin-bottom: 0.5rem;
  }

  .empty-subtitle {
    font-size: 1rem;
    color: var(--color-text-secondary);
    margin-bottom: 1.5rem;
  }

  .empty-action {
    background-color: var(--color-muted-teal);
    color: white;
    border: none;
    padding: 0.75rem 1.5rem;
    font-size: 0.9375rem;
    font-weight: 600;
    border-radius: var(--radius-md);
    cursor: pointer;
    transition: all 0.2s ease;
  }

  .empty-action:hover {
    background-color: var(--color-sage);
    transform: translateY(-2px);
    box-shadow: var(--shadow-md);
  }

  .projects-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
    gap: 1.25rem;
  }

  .project-header {
    display: flex;
    justify-content: space-between;
    align-items: start;
    margin-bottom: 0.75rem;
    gap: 0.75rem;
  }

  .projects-grid h2 {
    font-size: 1.125rem;
    font-weight: 600;
    color: var(--color-sage);
    margin: 0;
    flex: 1;
    min-width: 0;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .role-badge {
    background-color: rgba(120, 160, 131, 0.1);
    color: var(--color-sage);
    padding: 0.25rem 0.625rem;
    border-radius: 12px;
    font-size: 0.75rem;
    font-weight: 600;
    text-transform: uppercase;
    white-space: nowrap;
  }

  .project-description {
    color: var(--color-text-secondary);
    margin-bottom: 1rem;
    line-height: 1.5;
    font-size: 0.875rem;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }

  .project-footer {
    padding-top: 0.75rem;
    border-top: 1px solid var(--color-border);
  }

  .project-date {
    font-size: 0.8125rem;
    color: var(--color-text-secondary);
  }

  .dialog-overlay {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
  }

  .dialog {
    background-color: white;
    border-radius: 12px;
    padding: 2rem;
    max-width: 500px;
    width: 90%;
    max-height: 90vh;
    overflow-y: auto;
  }

  .dialog h2 {
    font-size: 1.5rem;
    font-weight: 700;
    color: var(--color-sage);
    margin-bottom: 1.5rem;
  }

  .form-group {
    margin-bottom: 1.5rem;
  }

  .form-group label {
    display: block;
    font-weight: 600;
    color: var(--color-sage);
    margin-bottom: 0.5rem;
  }

  .form-group input,
  .form-group textarea {
    width: 100%;
    padding: 0.75rem;
    border: 2px solid var(--color-teal-grey);
    border-radius: 8px;
    font-family: inherit;
    font-size: 1rem;
    transition: border-color 0.2s ease;
  }

  .form-group input:focus,
  .form-group textarea:focus {
    outline: none;
    border-color: var(--color-muted-teal);
  }

  .form-group textarea {
    resize: vertical;
  }

  .dialog-actions {
    display: flex;
    gap: 1rem;
    justify-content: flex-end;
  }

  .dialog-actions button {
    padding: 0.75rem 1.5rem;
    border-radius: 8px;
    font-size: 1rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s ease;
    border: none;
  }

  .dialog-actions button.secondary {
    background-color: var(--color-teal-grey);
    color: var(--color-sage);
  }

  .dialog-actions button.secondary:hover {
    background-color: var(--color-muted-teal);
    color: white;
  }

  .dialog-actions button.primary {
    background-color: var(--color-muted-teal);
    color: white;
  }

  .dialog-actions button.primary:hover {
    background-color: var(--color-sage);
  }

  .dialog-actions button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .dialog-actions button:disabled:hover {
    background-color: var(--color-muted-teal);
  }
</style>
