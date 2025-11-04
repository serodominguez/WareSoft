<template>
  <v-card elevation="2">
    <v-toolbar>
      <v-toolbar-title>Gestión de Permisos</v-toolbar-title>
    </v-toolbar>
    <v-card-text>
      <v-row>
        <v-col cols="4" md="4" lg="2" xl="2">
          <v-autocomplete color="primary" variant="underlined" :items="roles" v-model="selectedRoleId"
            item-title="rolE_NAME" item-value="pK_ROLE" no-data-text="No hay datos disponibles" label="Rol" />
        </v-col>
        <v-col cols="4" md="4" lg="4" xl="4" class="d-flex align-center">
          <v-btn color="indigo" @click="loadPermissions" :disabled="!selectedRoleId || loading" :loading="loading"> Cargar </v-btn>
          <v-btn color="success" @click="savePermissions" :disabled="!hasChanges || saving" :loading="saving" class="ml-2"> Guardar </v-btn>
        </v-col>
      </v-row>
      <v-alert v-if="alertMessage" :type="alertType" dismissible class="mt-4" @click:close="alertMessage = ''">
        {{ alertMessage }}
      </v-alert>
      <v-data-table :headers="headers" :items="localPermissions" :loading="loading" loading-text="Cargando permisos..."
        no-data-text="Seleccione un rol y presione en Cargar" class="elevation-1 mt-4"
        :hide-default-footer="true">
        <template v-slot:item.permissions.crear="{ item }">
          <v-checkbox v-model="item.permissions.crear" color="primary" hide-details @change="markAsChanged" />
        </template>
        <template v-slot:item.permissions.leer="{ item }">
          <v-checkbox v-model="item.permissions.leer" color="primary" hide-details @change="markAsChanged" />
        </template>
        <template v-slot:item.permissions.editar="{ item }">
          <v-checkbox v-model="item.permissions.editar" color="primary" hide-details @change="markAsChanged" />
        </template>
        <template v-slot:item.permissions.eliminar="{ item }">
          <v-checkbox v-model="item.permissions.eliminar" color="primary" hide-details @change="markAsChanged" />
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
      originalPermissions: [] as PermissionsByModule[],
      hasChanges: false,
      saving: false,
      alertMessage: '',
      alertType: 'success' as 'success' | 'error' | 'warning' | 'info',
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

    permissions() {
      return this.$store.getters['permission/permissions'];
    },

    loading(): boolean {
      return this.$store.getters['permission/loading'];
    },
  },

  watch: {
    permissionsByModule: {
      handler(newPermissions) {
        this.localPermissions = JSON.parse(JSON.stringify(newPermissions));
        this.originalPermissions = JSON.parse(JSON.stringify(newPermissions));
        this.hasChanges = false;
      },
      deep: true
    }
  },

  methods: {
    async loadPermissions() {
      if (this.selectedRoleId) {
        this.hasChanges = false;
        this.alertMessage = '';
        await this.$store.dispatch('permission/fetchPermissionsByRole', this.selectedRoleId);
      }
    },

    markAsChanged() {
      this.hasChanges = true;
    },

    async savePermissions() {
      this.saving = true;
      this.alertMessage = '';

      try {
        // Construir el array de permisos actualizados según el formato esperado por el backend
        const updatedPermissions = this.permissions.map((perm: any) => {
          // Encontrar el módulo correspondiente en localPermissions
          const localModule = this.localPermissions.find(
            (lp) => lp.module === perm.modulE_NAME
          );

          if (localModule) {
            const actionKey = perm.actioN_NAME.toLowerCase() as 'crear' | 'leer' | 'editar' | 'eliminar';
            return {
              pK_PERMISSION: perm.pK_PERMISSION,
              state: localModule.permissions[actionKey]
            };
          }

          return {
            pK_PERMISSION: perm.pK_PERMISSION,
            state: perm.state
          };
        });

        const response = await this.$store.dispatch('permission/updatePermissions', updatedPermissions);

        if (response.success) {
          this.alertType = 'success';
          this.alertMessage = 'Permisos actualizados correctamente';
          this.hasChanges = false;

          // Recargar los permisos para asegurar sincronización
          if (this.selectedRoleId) {
            await this.$store.dispatch('permission/fetchPermissionsByRole', this.selectedRoleId);
          }
        } else {
          this.alertType = 'error';
          this.alertMessage = response.message || 'Error al actualizar permisos';
        }
      } catch (error: any) {
        this.alertType = 'error';
        this.alertMessage = error.message || 'Error al guardar los permisos';
      } finally {
        this.saving = false;
      }
    },
  },

  mounted() {
    this.$store.dispatch('role/fetchRoles');
  },
});
</script>