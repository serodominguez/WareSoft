<template>
  <BrandList />
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import BrandList from '@/components/Brand/BrandList.vue';

export default defineComponent({
  name: 'BrandView',
  
  components: {
    BrandList,
  },
});
</script>