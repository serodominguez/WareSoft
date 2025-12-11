<template>
  <v-data-table :headers="headers" :items="permissions" :loading="loading" loading-text="Cargando permisos..."
    no-data-text="Seleccione un rol y presione en Cargar" class="elevation-1 mt-4" :items-per-page="-1" :hide-default-footer="true" > <!--:items-per-page-options="[10, 20, { value: -1, title: 'Todos' }]"-->
    <template v-slot:item.permissions.crear="{ item }">
      <v-checkbox v-model="item.permissions.crear" color="indigo" hide-details @change="$emit('permission-changed')" />
    </template>
    <template v-slot:item.permissions.leer="{ item }">
      <v-checkbox v-model="item.permissions.leer" color="indigo" hide-details @change="$emit('permission-changed')" />
    </template>
    <template v-slot:item.permissions.editar="{ item }">
      <v-checkbox v-model="item.permissions.editar" color="indigo" hide-details
        @change="$emit('permission-changed')" />
    </template>
    <template v-slot:item.permissions.eliminar="{ item }">
      <v-checkbox v-model="item.permissions.eliminar" color="indigo" hide-details
        @change="$emit('permission-changed')" />
    </template>
  </v-data-table>
</template>

<script lang="ts">
import { defineComponent, PropType } from 'vue';
import { PermissionsByModule } from '@/interfaces/permissionInterface';

export default defineComponent({
  name: 'PermissionList',
  props: {
    permissions: {
      type: Array as PropType<PermissionsByModule[]>,
      required: true
    },
    loading: {
      type: Boolean,
      required: true
    }
  },
  emits: ['permission-changed'],
  computed: {
    headers() {
      return [
        { title: 'Módulo', key: 'module', sortable: false },
        { title: 'Crear', key: 'permissions.crear', sortable: false },
        { title: 'Leer', key: 'permissions.leer', sortable: false },
        { title: 'Editar', key: 'permissions.editar', sortable: false },
        { title: 'Eliminar', key: 'permissions.eliminar', sortable: false },
      ];
    }
  }
});
</script>