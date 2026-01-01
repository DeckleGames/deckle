<script lang="ts">
  type Padding = {
    all?: number;
    top?: number;
    right?: number;
    bottom?: number;
    left?: number;
  };

  let {
    padding,
    onchange,
  }: {
    padding?: Padding;
    onchange: (newPadding: Padding) => void;
  } = $props();

  // Track whether we're in "separate sides" mode
  let separateSides = $state(
    !!(
      padding?.top !== undefined ||
      padding?.right !== undefined ||
      padding?.bottom !== undefined ||
      padding?.left !== undefined
    )
  );

  function toggleSeparateSides() {
    separateSides = !separateSides;

    if (!separateSides) {
      // Switching to "all sides" mode - use first defined side or default
      const allValue =
        padding?.top ??
        padding?.right ??
        padding?.bottom ??
        padding?.left ??
        padding?.all;
      onchange({
        all: allValue,
        top: undefined,
        right: undefined,
        bottom: undefined,
        left: undefined,
      });
    } else {
      // Switching to "separate sides" mode - copy from all to individual sides
      const value = padding?.all;
      onchange({
        all: undefined,
        top: value,
        right: value,
        bottom: value,
        left: value,
      });
    }
  }

  function updateAllSides(value: number | undefined) {
    onchange({
      ...padding,
      all: value,
    });
  }

  function updateSide(
    side: "top" | "right" | "bottom" | "left",
    value: number
  ) {
    onchange({
      ...padding,
      [side]: value,
    });
  }
</script>

<div class="field">
  <div class="header">
    <label class="section-label">Padding:</label>
    <label class="toggle-label">
      <span>Separate sides</span>
      <input
        type="checkbox"
        checked={separateSides}
        onchange={toggleSeparateSides}
      />
    </label>
  </div>

  {#if !separateSides}
    <!-- All sides mode -->
    <div class="padding-input">
      <label for="padding-all">All sides</label>
      <input
        type="number"
        id="padding-all"
        value={padding?.all ?? ""}
        placeholder="0"
        oninput={(e) => {
          const val = e.currentTarget.value;
          updateAllSides(val ? parseInt(val) : undefined);
        }}
      />
      <span class="unit">px</span>
    </div>
  {:else}
    <!-- Separate sides mode -->
    <div class="padding-grid">
      <div class="padding-input">
        <label for="padding-top">Top</label>
        <input
          type="number"
          id="padding-top"
          value={padding?.top ?? ""}
          placeholder="0"
          oninput={(e) =>
            updateSide("top", parseInt(e.currentTarget.value) || 0)}
        />
        <span class="unit">px</span>
      </div>

      <div class="padding-input">
        <label for="padding-right">Right</label>
        <input
          type="number"
          id="padding-right"
          value={padding?.right ?? ""}
          placeholder="0"
          oninput={(e) =>
            updateSide("right", parseInt(e.currentTarget.value) || 0)}
        />
        <span class="unit">px</span>
      </div>

      <div class="padding-input">
        <label for="padding-bottom">Bottom</label>
        <input
          type="number"
          id="padding-bottom"
          value={padding?.bottom ?? ""}
          placeholder="0"
          oninput={(e) =>
            updateSide("bottom", parseInt(e.currentTarget.value) || 0)}
        />
        <span class="unit">px</span>
      </div>

      <div class="padding-input">
        <label for="padding-left">Left</label>
        <input
          type="number"
          id="padding-left"
          value={padding?.left ?? ""}
          placeholder="0"
          oninput={(e) =>
            updateSide("left", parseInt(e.currentTarget.value) || 0)}
        />
        <span class="unit">px</span>
      </div>
    </div>
  {/if}
</div>

<style>
  .field {
    margin-bottom: 1rem;
  }

  .header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 0.5rem;
  }

  .section-label {
    display: block;
    font-size: 0.75rem;
    font-weight: 500;
    color: #666;
  }

  .toggle-label {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-size: 0.75rem;
    color: #666;
    cursor: pointer;
  }

  .toggle-label input[type="checkbox"] {
    cursor: pointer;
    width: 36px;
    height: 20px;
    appearance: none;
    background: #ccc;
    border-radius: 10px;
    position: relative;
    transition: background 0.2s;
  }

  .toggle-label input[type="checkbox"]:checked {
    background: #0066cc;
  }

  .toggle-label input[type="checkbox"]::before {
    content: "";
    position: absolute;
    width: 16px;
    height: 16px;
    border-radius: 50%;
    background: white;
    top: 2px;
    left: 2px;
    transition: left 0.2s;
  }

  .toggle-label input[type="checkbox"]:checked::before {
    left: 18px;
  }

  .padding-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1rem;
    row-gap: 0.5rem;
  }

  .padding-input {
    display: flex;
    align-items: center;
    gap: 0.5rem;
  }

  .padding-input label {
    font-size: 0.75rem;
    color: #666;
    margin: 0;
    min-width: 40px;
  }

  .padding-input input[type="number"] {
    flex: 1;
    min-width: 60px;
    padding: 0.375rem 0.5rem;
    font-size: 0.813rem;
    line-height: 1.25rem;
    height: 2.125rem;
    border: 1px solid #d1d5db;
    border-radius: 4px;
    background: white;
    box-sizing: border-box;
  }

  .padding-input input[type="number"]:focus {
    outline: none;
    border-color: #0066cc;
  }

  .padding-input .unit {
    font-size: 0.75rem;
    color: #666;
  }
</style>
