<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="roles" :search="search || undefined" :items-per-page-text="pages"
        :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage" :items-length="totalRoles"
        :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as Role).roleName }}</td>
            <td>{{ (item as Role).auditCreateDate }}</td>
            <td>{{ (item as Role).statusRole }}</td>
            <td>
              <v-btn v-if="canEdit && (item as Role).statusRole == 'Activo'" color="indigo" icon="edit" variant="text"
                @click="$emit('edit-role', item)" size="small">
              </v-btn>
              <template v-if="canEdit && (item as Role).statusRole == 'Inactivo'">
                <v-btn color="indigo" icon="check" variant="text"
                  @click="$emit('open-modal', { role: item, action: 1 })" size="small">
                </v-btn>
              </template>
              <template v-if="canEdit && (item as Role).statusRole == 'Activo'">
                <v-btn color="indigo" icon="block" variant="text"
                  @click="$emit('open-modal', { role: item, action: 2 })" size="small">
                </v-btn>
              </template>
              <v-btn v-if="canDelete" color="indigo" icon="delete" variant="text"
                @click="$emit('open-modal', { role: item, action: 0 })" size="small">
              </v-btn>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title>Gestión de Roles</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn v-if="canRead" icon="download" @click="handleDownloadExcel" :loading="downloadingExcel">
            </v-btn>
            <v-btn icon="tune" @click="drawerModel = !drawerModel"></v-btn>
            <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
              <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo"
                hide-details single-line v-model="search" @click:append-inner="handleSearch()"
                @keyup.enter="handleSearch()">
              </v-text-field>
            </v-col>
            <v-card-actions>
              <v-btn v-if="canCreate" @click="$emit('open-form')" color="indigo" size="large">
                Nuevo
              </v-btn>
            </v-card-actions>
          </v-toolbar>
        </template>
        <template v-slot:no-data>
          <v-btn color="primary" @click="$emit('fetch-roles')"> Reset </v-btn>
        </template>
      </v-data-table-server>
    </v-card>
    <RoleFilters v-model="drawerModel" v-model:selected-filter="selectedFilterModel" v-model:state="stateModel"
      v-model:start-date="startDateModel" v-model:end-date="endDateModel" @apply-filters="handleSearch" />
  </div>
</template>

<script lang="ts">
import { defineComponent, PropType } from 'vue';
import { Role } from '@/interfaces/roleInterface';
import RoleFilters from './RoleFilters.vue';

export default defineComponent({
  name: 'RoleList',
  components: {
    RoleFilters
  },
  props: {
    roles: {
      type: Array as PropType<Role[]>,
      required: true
    },
    loading: {
      type: Boolean,
      required: true
    },
    totalRoles: {
      type: Number,
      required: true
    },
    canCreate: {
      type: Boolean,
      required: true
    },
    canRead: {
      type: Boolean,
      required: true
    },
    canEdit: {
      type: Boolean,
      required: true
    },
    canDelete: {
      type: Boolean,
      required: true
    },
    drawer: {
      type: Boolean,
      default: false
    },
    selectedFilter: {
      type: String,
      default: 'Rol'
    },
    state: {
      type: String,
      default: 'Activos'
    },
    startDate: {
      type: [Date, null] as any,
      default: null
    },
    endDate: {
      type: [Date, null] as any,
      default: null
    },
    downloadingExcel: {
      type: Boolean,
      default: false
    }
  },
  emits: [
    'open-form',
    'open-modal',
    'edit-role',
    'fetch-roles',
    'search-roles',
    'update-items-per-page',
    'change-page',
    'download-excel',
    'update:drawer',
    'update:selectedFilter',
    'update:state',
    'update:startDate',
    'update:endDate'
  ],
  data() {
    return {
      itemsPerPage: 10,
      pages: "Roles por Página",
      search: null as string | null
    };
  },
  computed: {
    headers() {
      return [
        { title: 'Rol', key: 'roleName', sortable: false },
        { title: 'Fecha de registro', key: 'auditCreateDate', sortable: false },
        { title: 'Estado', key: 'statusRole', sortable: false },
        { title: 'Acciones', key: 'actions', sortable: false },
      ];
    },
    drawerModel: {
      get() {
        return this.drawer;
      },
      set(value: boolean) {
        this.$emit('update:drawer', value);
      }
    },
    selectedFilterModel: {
      get() {
        return this.selectedFilter;
      },
      set(value: string) {
        this.$emit('update:selectedFilter', value);
      }
    },
    stateModel: {
      get() {
        return this.state;
      },
      set(value: string) {
        this.$emit('update:state', value);
      }
    },
    startDateModel: {
      get() {
        return this.startDate;
      },
      set(value: Date | null) {
        this.$emit('update:startDate', value);
      }
    },
    endDateModel: {
      get() {
        return this.endDate;
      },
      set(value: Date | null) {
        this.$emit('update:endDate', value);
      }
    }
  },
  methods: {
    handleSearch() {
      this.$emit('search-roles', {
        search: this.search,
        selectedFilter: this.selectedFilterModel,
        state: this.stateModel,
        startDate: this.startDateModel,
        endDate: this.endDateModel
      });
    },

    handleDownloadExcel() {
      this.$emit('download-excel', {
        search: this.search,
        selectedFilter: this.selectedFilterModel,
        stateFilter: this.stateModel,
        startDate: this.startDateModel,
        endDate: this.endDateModel
      });
    }
  }
});
</script>
