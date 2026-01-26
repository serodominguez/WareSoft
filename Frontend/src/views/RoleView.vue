<template>
  <div>
    <RoleList :roles="roles" :loading="loading" :totalRoles="totalRoles" :downloadingExcel="downloadingExcel"
      :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit" :canDelete="canDelete" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-role="openForm"
      @fetch-roles="fetchRoles" @search-roles="searchRoles" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" />

    <RoleForm v-model="form" :role="selectedRole" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedRole?.idRole || 0" :item="selectedRole?.roleName || ''"
      :action="action" moduleName="role" entityName="Role" name="Rol" gender="male" @action-completed="handleActionCompleted" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Role } from '@/interfaces/roleInterface';
import { formatDate } from '@/utils/date';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import RoleList from '@/components/Role/RoleList.vue';
import RoleForm from '@/components/Role/RoleForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

export default defineComponent({
  name: 'RoleView',
  components: {
    RoleList,
    RoleForm,
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
      selectedRole: null as Role | null,
      action: 0,
      selectedFilter: 'Rol',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null,
      downloadingExcel: false
    };
  },
  computed: {
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
    openModal(payload: { role: Role, action: number }) {
      this.selectedRole = payload.role;
      this.action = payload.action;
      this.modal = true;
    },

    openForm(role?: Role) {
      this.selectedRole = role ? { ...role } : {
        idRole: null,
        roleName: '',
        auditCreateDate: '',
        statusRole: ''
      };
      this.form = true;
    },

    async fetchRoles(params?: any) {
      try {
        await this.store.dispatch('role/fetchRoles', params || {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          stateFilter: this.stateFilter
        });
      } catch (error) {
        handleSilentError(error);
      }
    },

    getFilterParams(params: any) {
      const filterMap: { [key: string]: number } = { "Rol": 1 };
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

    async searchRoles(params: any) {
      this.search = params.search;
      this.selectedFilter = params.selectedFilter;
      this.state = params.state;
      this.startDate = params.startDate;
      this.endDate = params.endDate;

      try {
        await this.store.dispatch("role/fetchRoles", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          ...this.getFilterParams(params)
        });
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar roles');
      }
    },

    refreshRoles() {
      if (this.search?.trim()) {
        this.searchRoles({
          search: this.search,
          selectedFilter: this.selectedFilter,
          state: this.state,
          startDate: this.startDate,
          endDate: this.endDate
        });
      } else {
        this.fetchRoles();
      }
    },

    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;
      this.refreshRoles();
    },

    changePage(page: number) {
      this.currentPage = page;
      this.refreshRoles();
    },

    async downloadExcel(params: any) {
      this.downloadingExcel = true;
      try {
        await this.store.dispatch("role/downloadRolesExcel", {
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
      this.fetchRoles();
    },

    handleActionCompleted() {
      this.fetchRoles();
    }
  },
  mounted() {
    this.fetchRoles();
  }
});
</script>