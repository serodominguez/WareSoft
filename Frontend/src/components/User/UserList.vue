<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="users" :search="search || undefined" :items-per-page-text="pages"
        :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage" :items-length="totalUsers"
        :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as User).userName }}</td>
            <td>{{ (item as User).names }}</td>
            <td>{{ (item as User).lastNames }}</td>
            <td>{{ (item as User).phoneNumber }}</td>
            <td>{{ (item as User).roleName }}</td>
            <td>{{ (item as User).storeName }}</td>
            <td>{{ (item as User).auditCreateDate }}</td>
            <td>{{ (item as User).statusUser }}</td>
            <td class="text-center">
              <v-btn v-if="canEdit && (item as User).statusUser == 'Activo'" color="indigo" icon="edit" variant="text"
                @click="$emit('edit-user', item)" size="small" title="Editar">
              </v-btn>
              <template v-if="canEdit && (item as User).statusUser == 'Inactivo'">
                <v-btn color="green" icon="check" variant="text"
                  @click="$emit('open-modal', { user: item, action: 1 })" size="small" title="Activar">
                </v-btn>
              </template>
              <template v-if="canEdit && (item as User).statusUser == 'Activo'">
                <v-btn color="red" icon="block" variant="text"
                  @click="$emit('open-modal', { user: item, action: 2 })" size="small" title="Desactivar">
                </v-btn>
              </template>
              <v-btn v-if="canDelete" color="grey" icon="delete" variant="text"
                @click="$emit('open-modal', { user: item, action: 0 })" size="small" title="Eliminar">
              </v-btn>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title>Gestión de Usuarios</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn v-if="canRead" icon="mdi:mdi-microsoft-excel" @click="handleDownloadExcel" :loading="downloadingExcel" title="Descargar Excel"></v-btn>
            <v-btn icon="tune" @click="drawerModel = !drawerModel" title="Filtros"></v-btn>
            <v-btn v-if="canCreate" icon="add_box" @click="$emit('open-form')" title="Registrar"></v-btn>
            <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
              <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo"
                hide-details single-line v-model="search" @click:append-inner="handleSearch()"
                @keyup.enter="handleSearch()">
              </v-text-field>
            </v-col>
          </v-toolbar>
        </template>
        <template v-slot:no-data>
          <v-btn color="indigo" @click="$emit('fetch-users')"> Reset </v-btn>
        </template>
      </v-data-table-server>
    </v-card>
    <CommonFilters v-model="drawerModel" :filters="filterOptions" v-model:selected-filter="selectedFilterModel"
      v-model:state="stateModel" v-model:start-date="startDateModel" v-model:end-date="endDateModel"
      @apply-filters="handleSearch" />
  </div>
</template>

<script lang="ts">
import { defineComponent, PropType } from 'vue';
import { User } from '@/interfaces/userInterface';
import CommonFilters from '@/components/Common/CommonFilters.vue';

export default defineComponent({
  name: 'UserList',
  components: {
    CommonFilters
  },
  props: {
    users: {
      type: Array as PropType<User[]>,
      required: true
    },
    loading: {
      type: Boolean,
      required: true
    },
    totalUsers: {
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
      default: 'Usuario'
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
    },
    itemsPerPage: {
      type: Number,
      default: 10
    }
  },
  emits: [
    'open-form',
    'open-modal',
    'edit-user',
    'fetch-users',
    'search-users',
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
      pages: "Usuarios por Página",
      search: null as string | null,
      filterOptions: ['Usuario', 'Nombres', 'Apellidos', 'Tienda', 'Rol']
    };
  },
  computed: {
    headers(): Array<{ title: string; key: string; sortable: boolean; align?: 'start' | 'end' | 'center' }> {
      return [
        { title: 'Usuario', key: 'userName', sortable: false },
        { title: 'Nombres', key: 'names', sortable: false },
        { title: 'Apellidos', key: 'lastNames', sortable: false },
        { title: 'Teléfono', key: 'phoneNumber', sortable: false },
        { title: 'Rol', key: 'roleName', sortable: false },
        { title: 'Sucursal', key: 'storeName', sortable: false },
        { title: 'Fecha registro', key: 'auditCreateDate', sortable: false },
        { title: 'Estado', key: 'statusUser', sortable: false },
        { title: 'Acciones', key: 'actions', sortable: false, align: 'center' },
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
      this.$emit('search-users', {
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
