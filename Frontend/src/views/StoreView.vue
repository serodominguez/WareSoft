<template>
  <v-app>
    <StoreList />
  </v-app>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import StoreList from '@/components/Store/StoreList.vue';

export default defineComponent({
  name: 'StoreView',
  
  components: {
    StoreList,
  },
});
</script>