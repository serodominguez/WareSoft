<template>
  <UserList />
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import UserList from '@/components/User/UserList.vue';

export default defineComponent({
  name: 'UserView',
  
  components: {
    UserList,
  },
});
</script>