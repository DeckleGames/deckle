<script lang="ts">
  import type { ImageElement } from "../types";
  import { templateStore } from "$lib/stores/templateElements";
  import ConfigSection from "./ConfigSection.svelte";
  import VisibilityCheckbox from "./VisibilityCheckbox.svelte";
  import PositionControls from "./PositionControls.svelte";
  import DimensionInput from "./DimensionInput.svelte";
  import ColorPicker from "./ColorPicker.svelte";
  import TextField from "./TextField.svelte";
  import NumberField from "./NumberField.svelte";
  import SelectField from "./SelectField.svelte";
  import Fields from "./Fields.svelte";

  let { element }: { element: ImageElement } = $props();

  function updateElement(updates: Partial<ImageElement>) {
    templateStore.updateElement(element.id, updates);
  }
</script>

<ConfigSection>
  <VisibilityCheckbox
    visible={element.visible}
    onchange={(visible) => updateElement({ visible })}
  />

  {#if element.position === "absolute"}
    <PositionControls
      x={element.x}
      y={element.y}
      onchange={(updates) => updateElement(updates)}
    />
  {/if}

  <TextField
    label="Image URL"
    id="image-url"
    placeholder="https://example.com/image.jpg"
    value={element.imageId}
    oninput={(e) => updateElement({ imageId: e.currentTarget.value })}
  />

  <Fields>
    <DimensionInput
      label="Width"
      id="width"
      value={element.dimensions?.width?.toString()}
      onchange={(width) =>
        updateElement({
          dimensions: {
            ...element.dimensions,
            width,
          },
        })}
    />

    <DimensionInput
      label="Height"
      id="height"
      value={element.dimensions?.height?.toString()}
      onchange={(height) =>
        updateElement({
          dimensions: {
            ...element.dimensions,
            height,
          },
        })}
    />
  </Fields>

  <Fields>
    <SelectField
      label="Object Fit"
      id="object-fit"
      value={element.objectFit || "cover"}
      options={[
        { value: "cover", label: "Cover" },
        { value: "contain", label: "Contain" },
        { value: "fill", label: "Fill" },
        { value: "none", label: "None" },
        { value: "scale-down", label: "Scale Down" },
      ]}
      onchange={(value) => updateElement({ objectFit: value as any })}
    />

    <TextField
      label="Object Position"
      id="object-position"
      placeholder="center, 50% 50%, top left"
      value={element.objectPosition ?? "center"}
      oninput={(e) => updateElement({ objectPosition: e.currentTarget.value })}
    />
  </Fields>

  <Fields>
    <NumberField
      label="Border Radius (px)"
      id="border-radius"
      value={element.borderRadius ?? ""}
      oninput={(e) =>
        updateElement({
          borderRadius: parseInt(e.currentTarget.value) || undefined,
        })}
    />

    <NumberField
      label="Border Width (px)"
      id="border-width"
      value={element.border?.width ?? ""}
      oninput={(e) =>
        updateElement({
          border: {
            ...element.border,
            width: parseInt(e.currentTarget.value) || undefined,
          },
        })}
    />
  </Fields>

  <Fields>
    <ColorPicker
      label="Border Color"
      id="border-color"
      value={element.border?.color || "#000000"}
      onchange={(color) =>
        updateElement({
          border: {
            ...element.border,
            color,
          },
        })}
    />

    <SelectField
      label="Border Style"
      id="border-style"
      value={element.border?.style || "solid"}
      options={[
        { value: "solid", label: "Solid" },
        { value: "dashed", label: "Dashed" },
        { value: "dotted", label: "Dotted" },
        { value: "double", label: "Double" },
        { value: "none", label: "None" },
      ]}
      onchange={(value) =>
        updateElement({
          border: {
            ...element.border,
            style: value as any,
          },
        })}
    />
  </Fields>
</ConfigSection>
