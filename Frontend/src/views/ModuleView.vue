<template>
  <div>
    <ModuleList :modules="modules" :loading="loading" :totalModules="totalModules" :downloadingExcel="downloadingExcel"
      :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit" :canDelete="canDelete" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal"
      @edit-module="openForm" @fetch-modules="fetchModules" @search-modules="searchModules"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel" />

    <ModuleForm v-model="form" :module="selectedModule" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedModule?.idModule || 0" :item="selectedModule?.moduleName || ''"
      :action="action" moduleName="module" entityName="Module" name="Módulo" gender="male"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Module } from '@/interfaces/moduleInterface';
import { formatDate } from '@/utils/date';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import ModuleList from '@/components/Module/ModuleList.vue';
import ModuleForm from '@/components/Module/ModuleForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

export default defineComponent({
  name: 'ModuleView',
  components: {
    ModuleList,
    ModuleForm,
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
      selectedModule: null as Module | null,
      action: 0,
      selectedFilter: 'Módulo',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null,
      downloadingExcel: false
    };
  },
  computed: {
    modules() {
      return this.store.getters['module/modules'];
    },
    loading() {
      return this.store.getters['module/loading'];
    },
    totalModules() {
      return this.store.getters['module/totalModules'];
    },
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    },
    canCreate(): boolean {
      return this.$store.getters.hasPermission('modulos', 'crear');
    },
    canRead(): boolean {
      return this.$store.getters.hasPermission('modulos', 'leer');
    },
    canEdit(): boolean {
      return this.$store.getters.hasPermission('modulos', 'editar');
    },
    canDelete(): boolean {
      return this.$store.getters.hasPermission('modulos', 'eliminar');
    }
  },
  methods: {
    openModal(payload: { module: Module, action: number }) {
      this.selectedModule = payload.module;
      this.action = payload.action;
      this.modal = true;
    },

    openForm(module?: Module) {
      this.selectedModule = module ? { ...module } : {
        idModule: null,
        moduleName: '',
        auditCreateDate: '',
        statusModule: ''
      };
      this.form = true;
    },

    async fetchModules(params?: any) {
      try {
        await this.store.dispatch('module/fetchModules', params || {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          stateFilter: this.stateFilter
        });
      } catch (error) {
        handleSilentError(error);
      }
    },

    getFilterParams(params: any) {
      const filterMap: { [key: string]: number } = { "Módulo": 1 };
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

    async searchModules(params: any) {
      this.search = params.search;
      this.selectedFilter = params.selectedFilter;
      this.state = params.state;
      this.startDate = params.startDate;
      this.endDate = params.endDate;

      try {
        await this.store.dispatch("module/fetchModules", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          ...this.getFilterParams(params)
        });
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar módulos');
      }
    },

    refreshModules() {
      if (this.search?.trim()) {
        this.searchModules({
          search: this.search,
          selectedFilter: this.selectedFilter,
          state: this.state,
          startDate: this.startDate,
          endDate: this.endDate
        });
      } else {
        this.fetchModules();
      }
    },

    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;
      this.refreshModules();
    },

    changePage(page: number) {
      this.currentPage = page;
      this.refreshModules();
    },

    async downloadExcel(params: any) {
      this.downloadingExcel = true;
      try {
        await this.store.dispatch("module/downloadModulesExcel", {
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
      this.fetchModules();
    },

    handleActionCompleted() {
      this.fetchModules();
    }
  },
  mounted() {
    this.fetchModules();
  }
});
</script>