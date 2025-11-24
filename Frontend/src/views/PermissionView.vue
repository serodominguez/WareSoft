<template>
  <div>
    <v-card elevation="2">
      <v-toolbar>
        <v-toolbar-title>Gestión de Permisos</v-toolbar-title>
      </v-toolbar>
      <v-card-text>
        <v-row>
          <v-col cols="4" md="4" lg="2" xl="2">
            <v-autocomplete color="indigo" variant="underlined" :items="roles" v-model="selectedRoleId"
              item-title="roleName" item-value="idRole" no-data-text="No hay datos disponibles" label="Rol"
              :loading="loadingRoles" />
          </v-col>
          <v-col cols="4" md="4" lg="4" xl="4" class="d-flex align-center">
            <v-btn color="indigo" @click="loadPermissions" :disabled="!selectedRoleId || loading" :loading="loading">
              Cargar
            </v-btn>
            <v-btn color="green" @click="savePermissions" :disabled="!hasChanges || saving" :loading="saving"
              class="ml-2">
              Guardar
            </v-btn>
          </v-col>
        </v-row>
        <PermissionList :permissions="localPermissions" :loading="loading" @permission-changed="markAsChanged" />
      </v-card-text>
    </v-card>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { PermissionsByModule } from '@/interfaces/permissionInterface';
import { handleApiError } from '@/helpers/errorHandler';
import PermissionList from '@/components/Permission/PermissionList.vue';

export default defineComponent({
  name: 'PermissionView',
  components: {
    PermissionList
  },
  setup() {
    const store = useStore();
    const toast = useToast();
    return { store, toast };
  },
  data() {
    return {
      selectedRoleId: null as number | null,
      localPermissions: [] as PermissionsByModule[],
      originalPermissions: [] as PermissionsByModule[],
      hasChanges: false,
      saving: false
    };
  },
  computed: {
    roles() {
      const rolesFromStore = this.store.getters['role/roles'];
      return Array.isArray(rolesFromStore) ? rolesFromStore : [];
    },
    loadingRoles(): boolean {
      return this.store.getters['role/loading'];
    },
    permissionsByModule(): PermissionsByModule[] {
      return this.store.getters['permission/permissionsByModule'];
    },
    permissions() {
      return this.store.getters['permission/permissions'];
    },
    loading(): boolean {
      return this.store.getters['permission/loading'];
    }
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

      try {
        await this.store.dispatch('permission/fetchPermissionsByRole', this.selectedRoleId);
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
        const response = await this.store.dispatch('permission/updatePermissions', updatedPermissions);

        if (response && response.success) {
          this.toast.success('Permisos actualizados correctamente');
          this.hasChanges = false;

          // Recargar permisos para confirmar
          if (this.selectedRoleId) {
            await this.store.dispatch('permission/fetchPermissionsByRole', this.selectedRoleId);
          }
        } else {
          const errorMsg = response?.message || 'Error al actualizar permisos';
          this.toast.error(errorMsg);
        }
      } catch (error) {
        const appError = handleApiError(error, 'Error al guardar los permisos');
      } finally {
        this.saving = false;
      }
    }
  },
  mounted() {
    this.store.dispatch('role/selectRole');
  }
});
</script>