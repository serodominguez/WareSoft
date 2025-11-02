<template>
  <PermissionForm />
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import PermissionForm from '@/components/Permission/PermissionForm.vue';

export default defineComponent({
  name: 'PermissionView',
  
  components: {
    PermissionForm,
  },
});
</script>