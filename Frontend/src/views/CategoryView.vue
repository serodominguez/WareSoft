<template>
  <CategoryList />
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import CategoryList from '@/components/Category/CategoryList.vue';

export default defineComponent({
  name: 'CategoryView',
  
  components: {
    CategoryList,
  },
});
</script>
