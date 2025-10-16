<template>
  <div>
    <v-toolbar>
      <v-toolbar-title>Gestión de Roles</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn icon="tune" @click="drawer = !drawer"></v-btn>
      <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
        <v-text-field append-inner-icon="search" density="compact" label="Búsqueda" variant="solo" hide-details
          single-line v-model="search" @click:append-inner="searchRoles()"
          @keyup.enter="searchRoles()"></v-text-field>
      </v-col>
      <v-card-actions>
        <v-btn @click="openForm" color="indigo" size="large"> Nuevo </v-btn>
      </v-card-actions>
    </v-toolbar>
    <div style="display: flex; gap: 16px; margin-top: 16px;">
      <v-data-table-server :headers="headers" :items="roles" :search="search || undefined"
        :items-per-page-text="pages" :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage"
        :items-length="totalRoles" :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="updateItemsPerPage" @update:page="changePage">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as Role).rolE_NAME }}</td>
            <td>{{ (item as Role).audiT_CREATE_DATE }}</td>
            <td>{{ (item as Role).statE_ROLE }}</td>
            <td>
              <v-btn v-if="(item as Role).statE_ROLE == 'ACTIVO'" color="indigo" icon="edit" variant="text"
                @click="editRole(item)" size="small"></v-btn>
              <template v-if="(item as Role).statE_ROLE == 'INACTIVO'">
                <v-btn color="indigo" icon="check" variant="text" @click="openModal(item, 1)" size="small"></v-btn>
              </template>
              <template v-if="(item as Role).statE_ROLE == 'ACTIVO'">
                <v-btn color="indigo" icon="block" variant="text" @click="openModal(item, 2)" size="small"></v-btn>
              </template>
              <v-btn color="indigo" icon="delete" variant="text" @click="openModal(item, 0)" size="small"></v-btn>
            </td>
          </tr>
        </template>
        <template v-slot:no-data>
          <v-btn color="primary" @click="initialize"> Reset </v-btn>
        </template>
      </v-data-table-server>
    </div>
    <v-navigation-drawer v-model="drawer" temporary>
      <v-list>
        <v-list-item>
          <v-list-item-title class="text-h6">Filtros</v-list-item-title>
        </v-list-item>
        <v-list-item>
          <v-select v-model="selectedFilter" :items="filters" variant="outlined" density="compact"
            hide-details></v-select>
        </v-list-item>
        <v-list-item>
          <v-switch v-model="state" :label="`Estado: ${state}`" false-value="Inactivos" true-value="Activos"
            color="indigo" hide-details></v-switch>
        </v-list-item>
        <v-list-item>
          <v-date-input v-model="startDate" label="Desde:" prepend-icon="" variant="underlined" persistent-placeholder></v-date-input>
        </v-list-item>
        <v-list-item>
          <v-date-input v-model="endDate" label="Hasta:" prepend-icon="" variant="underlined" persistent-placeholder></v-date-input>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
  </div>
  <RoleForm v-model="form" :role="selectedRole" @saved="fetchRoles" />
  <RoleModal v-model="modal" :role="selectedRole" :action="action" @update:modelValue="modal = $event" />
</template>
<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { Role } from '@/models/roleModel';
import RoleForm from './RoleForm.vue';
import RoleModal from './RoleModal.vue';

export default defineComponent({
  components: {
    RoleForm,
    RoleModal
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
      filters: ['Rol'],
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null
    };
  },
  computed: {
    headers() {
      return [
        { title: 'Rol', key: 'rolE_NAME', sortable: false },
        { title: 'Fecha registro', key: 'audiT_CREATE_DATE', sortable: false },
        { title: 'Estado', key: 'statE_ROLE', sortable: false },
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
        pK_ROLE: null,
        rolE_NAME: '',
        audiT_CREATE_DATE: '',
        statE_ROLE: ''
      };
      this.form = true;
    },
    async fetchRoles() {
      await this.store.dispatch('role/fetchRoles', {
        pageNumber: this.currentPage,
        pageSize: this.itemsPerPage,
        stateFilter: this.stateFilter
      });
    },
    async searchRoles() {
      let numberFilterValue: number | null = null;
      if (this.selectedFilter === "Rol") {
        numberFilterValue = 1;
      }

      const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
      const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
      const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

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
