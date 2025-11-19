<template>
  <ProductList />
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import ProductList from '@/components/Product/ProductList.vue';

export default defineComponent({
  name: 'ProductView',
  
  components: {
    ProductList,
  },
});
</script>
