<template>
  <v-card elevation="2">
    <v-data-table-server :headers="headers" :items="users" :search="search || undefined" :items-per-page-text="pages"
      :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage" :items-length="totalUsers"
      :loading="loading" loading-text="Cargando... Espere por favor" @update:items-per-page="updateItemsPerPage"
      @update:page="changePage">
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
          <td>
            <v-btn v-if="canEdit && (item as User).statusUser == 'Activo'" color="indigo" icon="edit" variant="text"
              @click="editUser(item)" size="small"></v-btn>
            <template v-if="canEdit && (item as User).statusUser == 'Inactivo'">
              <v-btn color="indigo" icon="check" variant="text" @click="openModal(item, 1)" size="small"></v-btn>
            </template>
            <template v-if="canEdit && (item as User).statusUser == 'Activo'">
              <v-btn color="indigo" icon="block" variant="text" @click="openModal(item, 2)" size="small"></v-btn>
            </template>
            <v-btn v-if="canDelete" color="indigo" icon="delete" variant="text" @click="openModal(item, 0)" size="small"></v-btn>
          </td>
        </tr>
      </template>
      <template v-slot:top>
        <v-toolbar>
          <v-toolbar-title>Gestión de Usuarios</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-btn v-if="canRead" icon="download" @click="downloadExcel" :loading="downloadingExcel"></v-btn>
          <v-btn icon="tune" @click="drawer = !drawer"></v-btn>
          <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
            <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo" hide-details
              single-line v-model="search" @click:append-inner="searchUsers()"
              @keyup.enter="searchUsers()"></v-text-field>
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
  <UserFilters v-model="drawer" v-model:selected-filter="selectedFilter" v-model:state="state" v-model:start-date="startDate" v-model:end-date="endDate" @apply-filters="searchUsers" />
  <UserForm v-model="form" :user="selectedUser" @saved="fetchUsers" />
  <UserModal v-model="modal" :user="selectedUser" :action="action" @update:modelValue="modal = $event" />
</template>
<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { User } from '@/interfaces/userInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import UserForm from './UserForm.vue';
import UserModal from './UserModal.vue';
import UserFilters from './UserFilters.vue';

export default defineComponent({
  components: {
    UserForm,
    UserModal, 
    UserFilters
  },
  data() {
    return {
      items: [10, 20, 50],
      currentPage: 1,
      itemsPerPage: 10,
      pages: "Usuarios por Página",
      search: null as string | null,
      form: false,
      modal: false,
      selectedUser: null as User | null,
      action: 0,
      selectedFilter: 'Usuario', 
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
        { title: 'Usuario', key: 'userName', sortable: false  },
        { title: 'Nombres', key: 'names', sortable: false  },
        { title: 'Apellidos', key: 'lastNames', sortable: false  },
        { title: 'Teléfono', key: 'phoneNumber', sortable: false  },
        { title: 'Rol', key: 'roleName', sortable: false  },
        { title: 'Sucursal', key: 'storeName', sortable: false  },
        { title: 'Fecha registro', key: 'auditCreateDate', sortable: false  },
        { title: 'Estado', key: 'statusUser', sortable: false },
        { title: 'Acciones', key: 'actions', sortable: false },
      ];
    },
    totalPages() {
      return Math.ceil(this.store.getters['user/totalUsers'] / this.itemsPerPage);
    },
    users() {
      return this.store.getters['user/users'];
    },
    loading() {
      return this.store.getters['user/loading'];
    },
    totalUsers() {
      return this.store.getters['user/totalUsers'];
    },
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    },
    canCreate(): boolean {
      return this.$store.getters.hasPermission('usuarios', 'crear');
    },
    canRead(): boolean {
      return this.$store.getters.hasPermission('usuarios', 'leer');
    },
    canEdit(): boolean {
      return this.$store.getters.hasPermission('usuarios', 'editar');
    },
    canDelete(): boolean {
      return this.$store.getters.hasPermission('usuarios', 'eliminar');
    }
  },
  methods: {
    initialize() {
      this.fetchUsers();
    },
    openModal(user: any, action: number) {
      this.selectedUser = user;
      this.action = action;
      this.modal = true;
    },
    openForm() {
      this.selectedUser = {
        idUser: null,
        userName: '',
        password: '',
        passwordHash: '',
        names: '',
        lastNames: '',
        identificationNumber: '',
        phoneNumber: null,
        idRole: null,
        roleName: '',
        idStore: null,
        storeName: '',
        auditCreateDate: '',
        statusUser: '',
        updatePassword: false
      };
      this.form = true;
    },
    async fetchUsers() {
      try {
        await this.store.dispatch('user/fetchUsers', {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          stateFilter: this.stateFilter
        });
      } catch (error) {
        handleSilentError(error);
      }
    },
    async searchUsers() {
      let numberFilterValue: number | null = null;
      const filterMap: { [key: string]: number } = {
        "Usuario": 1,
        "Nombres": 2,
        "Apellidos": 3,
        "Tienda": 4,
        "Rol": 5,
      };

      numberFilterValue = filterMap[this.selectedFilter];
      const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
      const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
      const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

      try {
        await this.store.dispatch("user/fetchUsers", {
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
        handleApiError(error, 'Error al buscar usuarios');
      }
    },
    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;

      if (this.search && this.search.trim() !== "") {
        this.searchUsers();
      } else {
        this.fetchUsers();
      }
    },
    changePage(page: number) {
      this.currentPage = page;

      if (this.search && this.search.trim() !== "") {
        this.searchUsers();
      } else {
        this.fetchUsers();
      }
    },
    editUser(user: any) {
      this.selectedUser = { ...user };
      this.form = true;
    },
    async downloadExcel() {
      this.downloadingExcel = true;
      try {
      let numberFilterValue: number | null = null;
      const filterMap: { [key: string]: number } = {
        "Usuario": 1,
        "Nombres": 2,
        "Apellidos": 3,
        "Tienda": 4,
        "Rol": 5,
      }

        numberFilterValue = filterMap[this.selectedFilter];
        const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
        const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
        const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

        await this.store.dispatch("user/downloadUsersExcel", {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          textFilter: textFilterValue,
          numberFilter: numberFilterValue,
          stateFilter: this.stateFilter,
          startDate: startDateStr,
          endDate: endDateStr
        });
      } catch (error) {
        this.toast.success('Archivo descargado correctamente');
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
    this.fetchUsers();
  },
  setup() {
    const store = useStore();
    return { store };
  }
});
</script>
