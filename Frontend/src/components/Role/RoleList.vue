<template>
  <v-card elevation="2">
    <v-data-table-server :headers="headers" :items="roles" :search="search || undefined" :items-per-page-text="pages"
      :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage" :items-length="totalRoles"
      :loading="loading" loading-text="Cargando... Espere por favor" @update:items-per-page="updateItemsPerPage"
      @update:page="changePage">
      <template v-slot:item="{ item }">
        <tr>
          <td>{{ (item as Role).roleName }}</td>
          <td>{{ (item as Role).auditCreateDate }}</td>
          <td>{{ (item as Role).statusRole }}</td>
          <td>
            <v-btn v-if="canEdit && (item as Role).statusRole == 'Activo'" color="indigo" icon="edit" variant="text"
              @click="editRole(item)" size="small"></v-btn>
            <template v-if="canEdit && (item as Role).statusRole == 'Inactivo'">
              <v-btn color="indigo" icon="check" variant="text" @click="openModal(item, 1)" size="small"></v-btn>
            </template>
            <template v-if="canEdit && (item as Role).statusRole == 'Activo'">
              <v-btn color="indigo" icon="block" variant="text" @click="openModal(item, 2)" size="small"></v-btn>
            </template>
            <v-btn v-if="canDelete" color="indigo" icon="delete" variant="text" @click="openModal(item, 0)" size="small"></v-btn>
          </td>
        </tr>
      </template>
      <template v-slot:top>
        <v-toolbar>
          <v-toolbar-title>Gestión de Roles</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-btn v-if="canRead" icon="download" @click="downloadExcel" :loading="downloadingExcel"></v-btn>
          <v-btn icon="tune" @click="drawer = !drawer"></v-btn>
          <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
            <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo" hide-details
              single-line v-model="search" @click:append-inner="searchRoles()"
              @keyup.enter="searchRoles()"></v-text-field>
          </v-col>
          <v-card-actions>
            <v-btn v-if="canCreate" @click="openForm" color="indigo" size="large"> Nuevo </v-btn>
          </v-card-actions>
        </v-toolbar>
      </template>
      <template v-slot:no-data>
        <v-btn color="primary" @click="initialize"> Reset </v-btn>
      </template>
    </v-data-table-server>
  </v-card>
  <RoleFilters v-model="drawer" v-model:selected-filter="selectedFilter" v-model:state="state" v-model:start-date="startDate" v-model:end-date="endDate" @apply-filters="searchRoles" />
  <RoleForm v-model="form" :role="selectedRole" @saved="fetchRoles" />
  <RoleModal v-model="modal" :role="selectedRole" :action="action" @update:modelValue="modal = $event" />
</template>
<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Role } from '@/interfaces/roleInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import RoleForm from './RoleForm.vue';
import RoleModal from './RoleModal.vue';
import RoleFilters from './RoleFilters.vue';

export default defineComponent({
  components: {
    RoleForm,
    RoleModal,
    RoleFilters
  },
  data() {
    return {
      items: [10, 20, 50],
      currentPage: 1,
      itemsPerPage: 10,
      pages: "Roles por Página",
      search: null as string | null,
      form: false,
      modal: false,
      selectedRole: null as Role | null,
      action: 0,
      selectedFilter: 'Rol',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null,
      downloadingExcel: false,
      toast: useToast()
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
    totalPages() {
      return Math.ceil(this.store.getters['role/totalRoles'] / this.itemsPerPage);
    },
    roles() {
      return this.store.getters['role/roles'];
    },
    loading() {
      return this.store.getters['role/loading'];
    },
    totalRoles() {
      return this.store.getters['role/totalRoles'];
    },
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    },
    canCreate(): boolean {
      return this.$store.getters.hasPermission('roles', 'crear');
    },
    canRead(): boolean {
      return this.$store.getters.hasPermission('roles', 'leer');
    },
    canEdit(): boolean {
      return this.$store.getters.hasPermission('roles', 'editar');
    },
    canDelete(): boolean {
      return this.$store.getters.hasPermission('roles', 'eliminar');
    }
  },
  methods: {
    initialize() {
      this.fetchRoles();
    },
    openModal(role: any, action: number) {
      this.selectedRole = role;
      this.action = action;
      this.modal = true;
    },
    openForm() {
      this.selectedRole = {
        idRole: null,
        roleName: '',
        auditCreateDate: '',
        statusRole: ''
      };
      this.form = true;
    },
    async fetchRoles() {
      try {
        await this.store.dispatch('role/fetchRoles', {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          stateFilter: this.stateFilter
        });
      } catch (error) {
        handleSilentError(error);
      }
    },
    async searchRoles() {
      let numberFilterValue: number | null = null;
      const filterMap: { [key: string]: number } = {
        "Rol": 1,
      }
        
      numberFilterValue = filterMap[this.selectedFilter];
      const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
      const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
      const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

      try {
        await this.store.dispatch("role/fetchRoles", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          textFilter: textFilterValue,
          numberFilter: numberFilterValue,
          stateFilter: this.stateFilter,
          startDate: startDateStr,
          endDate: endDateStr
        });
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar roles');
      }
    },
    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;

      if (this.search && this.search.trim() !== "") {
        this.searchRoles();
      } else {
        this.fetchRoles();
      }
    },
    changePage(page: number) {
      this.currentPage = page;

      if (this.search && this.search.trim() !== "") {
        this.searchRoles();
      } else {
        this.fetchRoles();
      }
    },
    editRole(role: any) {
      this.selectedRole = { ...role };
      this.form = true;
    },    
    async downloadExcel() {
      this.downloadingExcel = true;
      try {
      let numberFilterValue: number | null = null;
      const filterMap: { [key: string]: number } = {
        "Rol": 1,
      }
        
        numberFilterValue = filterMap[this.selectedFilter];
        const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
        const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
        const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

        await this.store.dispatch("role/downloadRolesExcel", {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          textFilter: textFilterValue,
          numberFilter: numberFilterValue,
          stateFilter: this.stateFilter,
          startDate: startDateStr,
          endDate: endDateStr
        });
        this.toast.success('Archivo descargado correctamente');
      } catch (error) {
        handleApiError(error, 'Error al descargar el archivo Excel');
      } finally {
        this.downloadingExcel = false;
      }
    },
    formatDate(date: Date | null): string | null {
      if (!date) return null;

      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const day = String(date.getDate()).padStart(2, '0');

      return `${year}-${month}-${day}`;
    }
  },
  mounted() {
    this.fetchRoles();
  },
  setup() {
    const store = useStore();
    return { store };
  }
});
</script>
