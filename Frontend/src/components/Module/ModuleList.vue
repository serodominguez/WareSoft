<template>
  <v-card elevation="2">
    <v-data-table-server :headers="headers" :items="modules" :search="search || undefined" :items-per-page-text="pages"
      :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage" :items-length="totalModules"
      :loading="loading" loading-text="Cargando... Espere por favor" @update:items-per-page="updateItemsPerPage"
      @update:page="changePage">
      <template v-slot:item="{ item }">
        <tr>
          <td>{{ (item as Module).moduleName }}</td>
          <td>{{ (item as Module).auditCreateDate }}</td>
          <td>{{ (item as Module).statusModule }}</td>
          <td>
            <v-btn v-if="canEdit && (item as Module).statusModule == 'Activo'" color="indigo" icon="edit" variant="text"
              @click="editModule(item)" size="small"></v-btn>
            <template v-if="canEdit && (item as Module).statusModule == 'Inactivo'">
              <v-btn color="indigo" icon="check" variant="text" @click="openModal(item, 1)" size="small"></v-btn>
            </template>
            <template v-if="canEdit && (item as Module).statusModule == 'Activo'">
              <v-btn color="indigo" icon="block" variant="text" @click="openModal(item, 2)" size="small"></v-btn>
            </template>
            <v-btn v-if="canDelete" color="indigo" icon="delete" variant="text" @click="openModal(item, 0)" size="small"></v-btn>
          </td>
        </tr>
      </template>
      <template v-slot:top>
        <v-toolbar>
          <v-toolbar-title>Gestión de Módulos</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-btn v-if="canRead" icon="download" @click="downloadExcel" :loading="downloadingExcel"></v-btn>
          <v-btn icon="tune" @click="drawer = !drawer"></v-btn>
          <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
            <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo" hide-details
              single-line v-model="search" @click:append-inner="searchModules()"
              @keyup.enter="searchModules()"></v-text-field>
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
  <ModuleFilters v-model="drawer" v-model:selected-filter="selectedFilter" v-model:state="state" v-model:start-date="startDate" v-model:end-date="endDate" @apply-filters="searchModules" />
  <ModuleForm v-model="form" :module="selectedModule" @saved="fetchModules" />
  <ModuleModal v-model="modal" :module="selectedModule" :action="action" @update:modelValue="modal = $event" />
</template>
<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { Module } from '@/interfaces/moduleInterface';
import ModuleForm from './ModuleForm.vue';
import ModuleModal from './ModuleModal.vue';
import ModuleFilters from './ModuleFilters.vue';

export default defineComponent({
  components: {
    ModuleForm,
    ModuleModal,
    ModuleFilters
  },
  data() {
    return {
      items: [10, 20, 50],
      currentPage: 1,
      itemsPerPage: 10,
      pages: "Módulos por Página",
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
      downloadingExcel: false,
    };
  },
  computed: {
    headers() {
      return [
        { title: 'Módulo', key: 'moduleName', sortable: false },
        { title: 'Fecha de registro', key: 'auditCreateDate', sortable: false },
        { title: 'Estado', key: 'statusModule', sortable: false },
        { title: 'Acciones', key: 'actions', sortable: false },
      ];
    },
    totalPages() {
      return Math.ceil(this.store.getters['module/totalModules'] / this.itemsPerPage);
    },
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
    initialize() {
      this.fetchModules();
    },
    openModal(module: any, action: number) {
      this.selectedModule = module;
      this.action = action;
      this.modal = true;
    },
    openForm() {
      this.selectedModule = {
        idModule: null,
        moduleName: '',
        auditCreateDate: '',
        statusModule: ''
      };
      this.form = true;
    },
    async fetchModules() {
      await this.store.dispatch('module/fetchModules', {
        pageNumber: this.currentPage,
        pageSize: this.itemsPerPage,
        stateFilter: this.stateFilter
      });
    },
    async searchModules() {
      let numberFilterValue: number | null = null;
      const filterMap: { [key: string]: number } = {
        "Módulo": 1,
      }
        
      numberFilterValue = filterMap[this.selectedFilter];
      const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
      const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
      const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

      await this.store.dispatch("module/fetchModule", {
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
        this.searchModules();
      } else {
        this.fetchModules();
      }
    },
    changePage(page: number) {
      this.currentPage = page;

      if (this.search && this.search.trim() !== "") {
        this.searchModules();
      } else {
        this.fetchModules();
      }
    },
    editModule(module: any) {
      this.selectedModule = { ...module };
      this.form = true;
    },    
    async downloadExcel() {
      this.downloadingExcel = true;
      try {
      let numberFilterValue: number | null = null;
      const filterMap: { [key: string]: number } = {
        "Módulo": 1,
      }
        
        numberFilterValue = filterMap[this.selectedFilter];
        const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
        const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
        const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

        await this.store.dispatch("module/downloadModulesExcel", {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          textFilter: textFilterValue,
          numberFilter: numberFilterValue,
          stateFilter: this.stateFilter,
          startDate: startDateStr,
          endDate: endDateStr
        });
      } catch (error) {
        console.error('Error al descargar el archivo:', error);
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
    this.fetchModules();
  },
  setup() {
    const store = useStore();
    return { store };
  }
});
</script>
