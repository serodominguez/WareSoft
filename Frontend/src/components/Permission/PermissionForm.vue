<template>
  <v-card elevation="2">
    <v-toolbar>
      <v-toolbar-title>Gestión de Permisos</v-toolbar-title>
    </v-toolbar>
    <v-card-text>
      <v-form ref="form">
        <v-row>
          <v-col cols="12" md="6">
            <v-autocomplete color="primary" variant="underlined" :items="roles" v-model="localRole.pK_ROLE"
              item-title="rolE_NAME" item-value="pK_ROLE" no-data-text="No hay datos disponibles" label="Rol"
              required />
          </v-col>
          <v-col cols="12" md="6" class="d-flex align-center">
            <v-btn color="indigo" @click="loadPermissions" :disabled="!localRole.pK_ROLE || loading" :loading="loading"> Cargar Permisos</v-btn>
            <v-btn color="success" @click="savePermissions" :disabled="!hasChanges || saving" :loading="saving" class="ml-2"> Guardar</v-btn>
          </v-col>
        </v-row>
      </v-form>

      <v-data-table :headers="headers" :items="localPermissions" :loading="loading" loading-text="Cargando permisos..."
        no-data-text="Seleccione un rol y presione 'Cargar Permisos'" class="elevation-1" :hide-default-footer="true" >
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
import { defineComponent, PropType } from 'vue';
import { useToast } from 'vue-toastification';
import { Role } from '@/interfaces/roleInterface';
import { PermissionsByModule } from '@/interfaces/permissionInterface';

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $store: VuexStore<any>;
  }
}

export default defineComponent({
  props: {
    role: {
      type: Object as PropType<Role | null>,
      default: () => ({
        pK_ROLE: null,
        rolE_NAME: ''
      }),
    },
  },

  data() {
    return {
      toast: useToast(),
      localRole: { ...this.role } as Role,
      localPermissions: [] as PermissionsByModule[],
      hasChanges: false,
      saving: false,
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
        // Crear una copia profunda para evitar mutar el estado de Vuex
        this.localPermissions = JSON.parse(JSON.stringify(newPermissions));
        this.hasChanges = false;
      },
      deep: true
    }
  },

  methods: {
    async loadPermissions() {
      if (this.localRole.pK_ROLE) {
        await this.$store.dispatch('permission/fetchPermissionsByRole', this.localRole.pK_ROLE);
      }
    },

    markAsChanged() {
      this.hasChanges = true;
    },

    async savePermissions() {
      this.saving = true;
      try {
        // TODO: Implementar el guardado en el backend
        console.log('Permisos a guardar:', this.localPermissions);
        console.log('Rol:', this.localRole.pK_ROLE);
        
        // Ejemplo de cómo podrías estructurar los datos para enviar
        const permissionsToSave = this.localPermissions.flatMap(module => {
          const permissions = [];
          if (module.permissions.crear) permissions.push({ module: module.module, action: 'Crear' });
          if (module.permissions.leer) permissions.push({ module: module.module, action: 'Leer' });
          if (module.permissions.editar) permissions.push({ module: module.module, action: 'Editar' });
          if (module.permissions.eliminar) permissions.push({ module: module.module, action: 'Eliminar' });
          return permissions;
        });

        console.log('Estructura para enviar:', permissionsToSave);

        // await this.$store.dispatch('permission/savePermissions', {
        //   roleId: this.localRole.pK_ROLE,
        //   permissions: permissionsToSave
        // });

        this.toast.success('Permisos actualizados correctamente');
        this.hasChanges = false;
      } catch (error: any) {
        this.toast.error('Error al guardar permisos: ' + error.message);
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