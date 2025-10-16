<template>
  <div>
    <v-toolbar>
      <v-toolbar-title>Gestión de Usuarios</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn icon="tune" @click="drawer = !drawer"></v-btn>
      <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
        <v-text-field append-inner-icon="search" density="compact" label="Búsqueda" variant="solo" hide-details
          single-line v-model="search" @click:append-inner="searchUsers()"
          @keyup.enter="searchUsers()"></v-text-field>
      </v-col>
      <v-card-actions>
        <v-btn @click="openForm" color="indigo" size="large"> Nuevo </v-btn>
      </v-card-actions>
    </v-toolbar>
    <div style="display: flex; gap: 16px; margin-top: 16px;">
      <v-data-table-server :headers="headers" :items="users" :search="search || undefined"
        :items-per-page-text="pages" :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage"
        :items-length="totalUsers" :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="updateItemsPerPage" @update:page="changePage">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as User).useR_NAME }}</td>
            <td>{{ (item as User).names }}</td>
            <td>{{ (item as User).lasT_NAMES }}</td>
            <td>{{ (item as User).phonE_NUMBER }}</td>
            <td>{{ (item as User).rolE_NAME }}</td>
            <td>{{ (item as User).storE_NAME }}</td>
            <td>{{ (item as User).audiT_CREATE_DATE }}</td>
            <td>{{ (item as User).statE_USER }}</td>
            <td>
              <v-btn v-if="(item as User).statE_USER == 'ACTIVO'" color="indigo" icon="edit" variant="text"
                @click="editUser(item)" size="small"></v-btn>
              <template v-if="(item as User).statE_USER == 'INACTIVO'">
                <v-btn color="indigo" icon="check" variant="text" @click="openModal(item, 1)" size="small"></v-btn>
              </template>
              <template v-if="(item as User).statE_USER == 'ACTIVO'">
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
  <UserForm v-model="form" :user="selectedUser" @saved="fetchUsers" />
  <UserModal v-model="modal" :user="selectedUser" :action="action" @update:modelValue="modal = $event" />
</template>
<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { User } from '@/models/userModel';
import UserForm from './UserForm.vue';
import UserModal from './UserModal.vue';

export default defineComponent({
  components: {
    UserForm,
    UserModal
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
      filters: ['Usuario', 'Nombres', 'Apellidos', 'Rol'],
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null
    };
  },
  computed: {
    headers() {
      return [
        { title: 'Usuario', key: 'useR_NAME', sortable: false  },
        { title: 'Nombres', key: 'names', sortable: false  },
        { title: 'Apellidos', key: 'lasT_NAME', sortable: false  },
        { title: 'Teléfono', key: 'phonE_NUMBER', sortable: false  },
        { title: 'Rol', key: 'rolE_NAME', sortable: false  },
        { title: 'Sucursal', key: 'storE_NAME', sortable: false  },
        { title: 'Fecha registro', key: 'audiT_CREATE_DATE', sortable: false  },
        { title: 'Estado', key: 'statE_USER', sortable: false },
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
        pK_USER: null,
        useR_NAME: '',
        password: '',
        passworD_HASH: '',
        names: '',
        lasT_NAMES: '',
        identificatioN_NUMBER: '',
        phonE_NUMBER: null,
        pK_ROLE: null,
        rolE_NAME: '',
        pK_STORE: null,
        storE_NAME: '',
        audiT_CREATE_DATE: '',
        statE_USER: '',
        updatE_PASSWORD: false
      };
      this.form = true;
    },
    async fetchUsers() {
      await this.store.dispatch('user/fetchUsers', {
        pageNumber: this.currentPage,
        pageSize: this.itemsPerPage,
        stateFilter: this.stateFilter
      });
    },
    async searchUsers() {
      let numberFilterValue: number | null = null;
      const filterMap: { [key: string]: number } = {
        "Usuario": 1,
        "Nombres": 2,
        "Apellidos": 3,
        "Rol": 4,
      };

      numberFilterValue = filterMap[this.selectedFilter];
      const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
      const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
      const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

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
