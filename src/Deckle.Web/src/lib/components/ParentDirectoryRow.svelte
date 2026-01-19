<script lang="ts">
  import LibraryRow, { type DragItemData } from './LibraryRow.svelte';

  let {
    parentDirectoryId,
    onclick,
    onItemDropped
  }: {
    parentDirectoryId: string | null;
    onclick?: () => void;
    onItemDropped?: (data: DragItemData) => void;
  } = $props();
</script>

<LibraryRow
  name=".."
  onRowClick={onclick}
  isDropTarget={true}
  onDrop={onItemDropped}
>
  {#snippet thumbnail()}
    <div class="parent-icon">
      <span class="arrow">↰</span>
    </div>
  {/snippet}
</LibraryRow>

<style>
  .parent-icon {
    width: 64px;
    height: 64px;
    background: linear-gradient(135deg, #d4d8e0 0%, #bfc5d1 100%);
    border-radius: var(--radius-md);
    display: flex;
    align-items: center;
    justify-content: center;
    color: #6b7280;
    transition: transform 0.2s ease;
  }

  .parent-icon:hover {
    transform: scale(1.05);
  }

  .arrow {
    font-size: 2rem;
    line-height: 1;
  }
</style>
