<script lang="ts">
  import FieldWrapper from './FieldWrapper.svelte';

  let {
    label,
    id,
    value,
    min,
    max,
    step,
    placeholder,
    oninput
  }: {
    label: string;
    id: string;
    value: number | string | undefined;
    min?: number;
    max?: number;
    step?: number | string;
    placeholder?: string;
    oninput: (e: Event & { currentTarget: HTMLInputElement }) => void;
  } = $props();
</script>

<FieldWrapper {label} htmlFor={id}>
  <input
    type="number"
    {id}
    {value}
    {min}
    {max}
    {step}
    {placeholder}
    {oninput}
  />
</FieldWrapper>
