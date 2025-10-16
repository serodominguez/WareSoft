<template>
  <RoleList />
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import RoleList from '@/components/Role/RoleList.vue';

export default defineComponent({
  name: 'BrandView',
  
  components: {
    RoleList,
  },
});
</script>