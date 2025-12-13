<script lang="ts">
  import PanelHeader from './PanelHeader.svelte';

  interface Props {
    html?: string;
    css?: string;
    onHtmlChange?: (value: string) => void;
    onCssChange?: (value: string) => void;
  }

  let { html = '', css = '', onHtmlChange, onCssChange }: Props = $props();

  let activeTab = $state<'html' | 'css'>('html');
  let htmlValue = $state(html);
  let cssValue = $state(css);

  // Sync with props
  $effect(() => {
    htmlValue = html;
  });

  $effect(() => {
    cssValue = css;
  });

  function handleHtmlInput(e: Event) {
    const target = e.target as HTMLTextAreaElement;
    htmlValue = target.value;
    if (onHtmlChange) {
      onHtmlChange(htmlValue);
    }
  }

  function handleCssInput(e: Event) {
    const target = e.target as HTMLTextAreaElement;
    cssValue = target.value;
    if (onCssChange) {
      onCssChange(cssValue);
    }
  }

  function handleKeyDown(e: KeyboardEvent) {
    const target = e.target as HTMLTextAreaElement;

    // Tab key handling
    if (e.key === 'Tab') {
      e.preventDefault();
      const start = target.selectionStart;
      const end = target.selectionEnd;
      const value = target.value;

      // Insert tab character
      target.value = value.substring(0, start) + '  ' + value.substring(end);
      target.selectionStart = target.selectionEnd = start + 2;

      // Trigger input event
      target.dispatchEvent(new Event('input', { bubbles: true }));
    }
  }
</script>

<div class="editor-panel">
  <PanelHeader title="Editor" />

  <div class="editor-tabs">
    <button
      class="tab"
      class:active={activeTab === 'html'}
      onclick={() => (activeTab = 'html')}
    >
      HTML
    </button>
    <button
      class="tab"
      class:active={activeTab === 'css'}
      onclick={() => (activeTab = 'css')}
    >
      CSS
    </button>
  </div>

  <div class="editor-content">
    {#if activeTab === 'html'}
      <textarea
        class="code-editor"
        value={htmlValue}
        oninput={handleHtmlInput}
        onkeydown={handleKeyDown}
        placeholder="Enter HTML here..."
        spellcheck="false"
      ></textarea>
    {:else}
      <textarea
        class="code-editor"
        value={cssValue}
        oninput={handleCssInput}
        onkeydown={handleKeyDown}
        placeholder="Enter CSS here..."
        spellcheck="false"
      ></textarea>
    {/if}
  </div>
</div>

<style>
  .editor-panel {
    display: flex;
    flex-direction: column;
    height: 100%;
    background-color: var(--color-bg-secondary);
  }

  .editor-tabs {
    display: flex;
    background-color: var(--color-dark);
    border-bottom: 1px solid var(--color-border);
    flex-shrink: 0;
  }

  .tab {
    padding: 0.75rem 1.5rem;
    background: transparent;
    border: none;
    color: var(--color-muted-teal);
    font-size: 0.875rem;
    font-weight: 500;
    cursor: pointer;
    transition: all 0.2s ease;
    border-bottom: 2px solid transparent;
  }

  .tab:hover {
    color: var(--color-sage);
    background-color: rgba(120, 160, 131, 0.05);
  }

  .tab.active {
    color: var(--color-sage);
    border-bottom-color: var(--color-sage);
  }

  .editor-content {
    flex: 1;
    overflow: hidden;
    display: flex;
    flex-direction: column;
  }

  .code-editor {
    flex: 1;
    width: 100%;
    padding: 1rem;
    background-color: #1e1e1e;
    color: #d4d4d4;
    border: none;
    font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
    font-size: 14px;
    line-height: 1.6;
    resize: none;
    outline: none;
    overflow: auto;
  }

  .code-editor::placeholder {
    color: #6a6a6a;
  }

  /* Custom scrollbar for code editor */
  .code-editor::-webkit-scrollbar {
    width: 12px;
    height: 12px;
  }

  .code-editor::-webkit-scrollbar-track {
    background: #1e1e1e;
  }

  .code-editor::-webkit-scrollbar-thumb {
    background: #424242;
    border-radius: 6px;
  }

  .code-editor::-webkit-scrollbar-thumb:hover {
    background: #4e4e4e;
  }
</style>
