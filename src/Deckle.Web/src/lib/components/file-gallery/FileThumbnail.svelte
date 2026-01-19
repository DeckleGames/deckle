<script lang="ts">
  import { config } from '$lib/config';

  let {
    projectId,
    path,
    fileName
  }: {
    projectId: string;
    path: string;
    fileName: string;
  } = $props();

  // Generate direct URL to the file redirect endpoint
  const thumbnailUrl = `${config.apiUrl}/file/${projectId}?filename=${encodeURIComponent(path)}`;
</script>

<div class="thumbnail-container">
  <img src={thumbnailUrl} alt={fileName} class="thumbnail-image" />
</div>

<style>
  .thumbnail-container {
    width: 64px;
    height: 64px;
    overflow: hidden;
    border-radius: var(--radius-md);
    background-color: var(--color-background);
  }

  .thumbnail-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
    transition: transform 0.2s ease;
  }

  .thumbnail-container:hover .thumbnail-image {
    transform: scale(1.05);
  }
</style>
