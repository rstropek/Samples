<script setup lang="ts">
// Link to a sample file on GitHub: <Src path="src/01_Basics/Program.cs" line="19" />
const props = defineProps<{ path: string; line?: number | string; label?: string }>()

const base = 'https://github.com/rstropek/Samples/blob/master/CSharp15/'
const href = base + props.path + (props.line ? `#L${props.line}` : '')
const text = props.label ?? props.path.replace(/^src\//, '') + (props.line ? `:${props.line}` : '')
</script>

<template>
  <a :href="href" target="_blank" rel="noopener" class="src-link">{{ text }}</a>
</template>
