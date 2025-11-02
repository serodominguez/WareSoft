<template>
  <ModuleList />
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import ModuleList from '@/components/Module/ModuleList.vue';

export default defineComponent({
  name: 'ModuleView',
  
  components: {
    ModuleList,
  },
});
</script>