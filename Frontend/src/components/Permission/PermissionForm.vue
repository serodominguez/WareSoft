<template>
  <v-card elevation="2">
    <v-toolbar>
      <v-toolbar-title>Gestión de Permisos</v-toolbar-title>
    </v-toolbar>
    <v-card-text>
      <v-row>
        <v-col cols="4" md="4" lg="2" xl="2">
          <v-autocomplete color="primary" variant="underlined" :items="roles" v-model="selectedRoleId"
            item-title="roleName" item-value="idRole" no-data-text="No hay datos disponibles" label="Rol" :loading="loadingRoles" />
        </v-col>
        <v-col cols="4" md="4" lg="4" xl="4" class="d-flex align-center">
          <v-btn color="indigo" @click="loadPermissions" :disabled="!selectedRoleId || loading" :loading="loading">
            Cargar </v-btn>
          <v-btn color="success" @click="savePermissions" :disabled="!hasChanges || saving" :loading="saving"
            class="ml-2"> Guardar </v-btn>
        </v-col>
      </v-row>
      <v-alert v-if="alertMessage" :type="alertType" dismissible class="mt-4" @click:close="alertMessage = ''">{{ alertMessage }}
      </v-alert>
      <v-data-table :headers="headers" :items="localPermissions" :loading="loading" loading-text="Cargando permisos..."
        no-data-text="Seleccione un rol y presione en Cargar" class="elevation-1 mt-4" :hide-default-footer="true">
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
import { useToast } from 'vue-toastification';
import { PermissionsByModule } from '@/interfaces/permissionInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';

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
      toast: useToast(),
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
    loadingRoles(): boolean {
      return this.$store.getters['role/loading'];
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
      if (!this.selectedRoleId) {
        this.toast.warning('Por favor selecciona un rol');
        return;
      }

      this.hasChanges = false;
      this.alertMessage = '';

      try {
        await this.$store.dispatch('permission/fetchPermissionsByRole', this.selectedRoleId);
      } catch (error) {
        handleApiError(error, 'Error al cargar los permisos del rol');
      }
    },
    markAsChanged() {
      this.hasChanges = true;
    },
    async savePermissions() {
   if (!this.hasChanges) {
        this.toast.info('No hay cambios para guardar');
        return;
      }

      this.saving = true;
      this.alertMessage = '';

      try {
        // Construir array de permisos actualizados
        const updatedPermissions = this.permissions.map((perm: any) => {
          const localModule = this.localPermissions.find(
            (lp) => lp.module === perm.moduleName
          );

          if (localModule) {
            const actionKey = perm.actionName.toLowerCase() as 'crear' | 'leer' | 'editar' | 'eliminar';
            return {
              idPermission: perm.idPermission,
              status: localModule.permissions[actionKey]
            };
          }

          return {
            idPermission: perm.idPermission,
            status: perm.status
          };
        });

        // Llamar al store
        const response = await this.$store.dispatch('permission/updatePermissions', updatedPermissions);

        if (response && response.success) {
          // Éxito
          this.toast.success('Permisos actualizados correctamente');
          this.hasChanges = false;

          // Recargar permisos para confirmar
          if (this.selectedRoleId) {
            await this.$store.dispatch('permission/fetchPermissionsByRole', this.selectedRoleId);
          }
        } else {
          // Respuesta del servidor indica fallo
          const errorMsg = response?.message || 'Error al actualizar permisos';
          this.toast.error(errorMsg);
          
          this.alertType = 'error';
          this.alertMessage = errorMsg;
        }
      } catch (error) {
        // Error de red/servidor
        const appError = handleApiError(error, 'Error al guardar los permisos');
        
        // Mostrar también en el alert del componente
        this.alertType = 'error';
        this.alertMessage = appError.message;
      } finally {
        this.saving = false;
      }
    },
  },
  mounted() {
    this.$store.dispatch('role/selectRole');
  },
});
</script>