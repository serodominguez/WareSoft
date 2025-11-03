<template>
  <v-card elevation="2">
    <v-toolbar>
      <v-toolbar-title>Gestión de Permisos</v-toolbar-title>
    </v-toolbar>
    <v-card-text>
      <v-row>
        <v-col cols="12" md="6">
          <v-autocomplete color="primary" variant="underlined" :items="roles" v-model="selectedRoleId"
            item-title="rolE_NAME" item-value="pK_ROLE" no-data-text="No hay datos disponibles" label="Rol" />
        </v-col>
        <v-col cols="12" md="6" class="d-flex align-center">
          <v-btn color="indigo" @click="loadPermissions" :disabled="!selectedRoleId || loading" :loading="loading">
            Cargar Permisos
          </v-btn>
        </v-col>
      </v-row>

      <v-data-table :headers="headers" :items="localPermissions" :loading="loading" loading-text="Cargando permisos..."
        no-data-text="Seleccione un rol y presione 'Cargar Permisos'" class="elevation-1 mt-4"
        :hide-default-footer="true">
        <template v-slot:item.permissions.crear="{ item }">
          <v-checkbox v-model="item.permissions.crear" color="primary" hide-details />
        </template>

        <template v-slot:item.permissions.leer="{ item }">
          <v-checkbox v-model="item.permissions.leer" color="primary" hide-details />
        </template>

        <template v-slot:item.permissions.editar="{ item }">
          <v-checkbox v-model="item.permissions.editar" color="primary" hide-details />
        </template>

        <template v-slot:item.permissions.eliminar="{ item }">
          <v-checkbox v-model="item.permissions.eliminar" color="primary" hide-details />
        </template>
      </v-data-table>
    </v-card-text>
  </v-card>
</template>

<script lang="ts">
import { Store as VuexStore } from 'vuex';
import { defineComponent } from 'vue';
import { PermissionsByModule } from '@/interfaces/permissionInterface';

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $store: VuexStore<any>;
  }
}

export default defineComponent({
  data() {
    return {
      selectedRoleId: null as number | null,
      localPermissions: [] as PermissionsByModule[],
      headers: [
        { title: 'Módulo', key: 'module', sortable: false },
        { title: 'Crear', key: 'permissions.crear', sortable: false },
        { title: 'Leer', key: 'permissions.leer', sortable: false },
        { title: 'Editar', key: 'permissions.editar', sortable: false },
        { title: 'Eliminar', key: 'permissions.eliminar', sortable: false },
      ],
    };
  },

  computed: {
    roles() {
      const rolesFromStore = this.$store.getters['role/roles'];
      return Array.isArray(rolesFromStore) ? rolesFromStore : [];
    },

    permissionsByModule(): PermissionsByModule[] {
      return this.$store.getters['permission/permissionsByModule'];
    },

    loading(): boolean {
      return this.$store.getters['permission/loading'];
    },
  },

  watch: {
    permissionsByModule: {
      handler(newPermissions) {
        this.localPermissions = JSON.parse(JSON.stringify(newPermissions));
      },
      deep: true
    }
  },

  methods: {
    async loadPermissions() {
      if (this.selectedRoleId) {
        await this.$store.dispatch('permission/fetchPermissionsByRole', this.selectedRoleId);
      }
    },
  },

  mounted() {
    this.$store.dispatch('role/fetchRoles');
  },
});
</script>