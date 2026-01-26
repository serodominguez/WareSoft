<template>
  <div>
    <UserList :users="users" :loading="loading" :totalUsers="totalUsers" :downloadingExcel="downloadingExcel"
      :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit" :canDelete="canDelete" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal"
      @edit-user="openForm" @fetch-users="fetchUsers" @search-users="searchUsers"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel" />

    <UserForm v-model="form" :user="selectedUser" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedUser?.idUser || 0" :item="selectedUser?.userName || ''"
      :action="action" moduleName="user" entityName="User" name="Usuario" gender="male"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { User } from '@/interfaces/userInterface';
import { formatDate } from '@/utils/date';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import UserList from '@/components/User/UserList.vue';
import UserForm from '@/components/User/UserForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

export default defineComponent({
  name: 'UserView',
  components: {
    UserList,
    UserForm,
    CommonModal
  },
  setup() {
    const store = useStore();
    const toast = useToast();
    return { store, toast };
  },
  data() {
    return {
      currentPage: 1,
      itemsPerPage: 10,
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
      downloadingExcel: false
    };
  },
  computed: {
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
    openModal(payload: { user: User, action: number }) {
      this.selectedUser = payload.user;
      this.action = payload.action;
      this.modal = true;
    },

    openForm(user?: User) {
      this.selectedUser = user ? { ...user } : {
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

    async fetchUsers(params?: any) {
      try {
        await this.store.dispatch('user/fetchUsers', params || {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          stateFilter: this.stateFilter
        });
      } catch (error) {
        handleSilentError(error);
      }
    },

    getFilterParams(params: any) {
      const filterMap: { [key: string]: number } = {
        "Usuario": 1,
        "Nombres": 2,
        "Apellidos": 3,
        "Tienda": 4,
        "Rol": 5
      };
      const numberFilterValue = filterMap[params.selectedFilter || this.selectedFilter];
      const textFilterValue = params.search?.trim() || null;
      const startDateStr = params.startDate ? formatDate(params.startDate) : null;
      const endDateStr = params.endDate ? formatDate(params.endDate) : null;

      return {
        textFilter: textFilterValue,
        numberFilter: numberFilterValue,
        stateFilter: this.stateFilter,
        startDate: startDateStr,
        endDate: endDateStr
      };
    },

    async searchUsers(params: any) {
      this.search = params.search;
      this.selectedFilter = params.selectedFilter;
      this.state = params.state;
      this.startDate = params.startDate;
      this.endDate = params.endDate;

      try {
        await this.store.dispatch("user/fetchUsers", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          ...this.getFilterParams(params)
        });
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar usuarios');
      }
    },

    refreshUsers() {
      if (this.search?.trim()) {
        this.searchUsers({
          search: this.search,
          selectedFilter: this.selectedFilter,
          state: this.state,
          startDate: this.startDate,
          endDate: this.endDate
        });
      } else {
        this.fetchUsers();
      }
    },

    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;
      this.refreshUsers();
    },

    changePage(page: number) {
      this.currentPage = page;
      this.refreshUsers();
    },

    async downloadExcel(params: any) {
      this.downloadingExcel = true;
      try {
        await this.store.dispatch("user/downloadUsersExcel", {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          ...this.getFilterParams(params)
        });
        this.toast.success('Archivo descargado correctamente');
      } catch (error) {
        handleApiError(error, 'Error al descargar el archivo Excel');
      } finally {
        this.downloadingExcel = false;
      }
    },
    
    handleSaved() {
      this.fetchUsers();
    },

    handleActionCompleted() {
      this.fetchUsers();
    }
  },
  mounted() {
    this.fetchUsers();
  }
});
</script>