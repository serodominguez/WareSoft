<template>
  <v-card elevation="2">
    <v-data-table-server :headers="headers" :items="categories" :search="search || undefined"
      :items-per-page-text="pages" :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage"
      :items-length="totalCategories" :loading="loading" loading-text="Cargando... Espere por favor"
      @update:items-per-page="updateItemsPerPage" @update:page="changePage">
      <template v-slot:item="{ item }">
        <tr>
          <td>{{ (item as Category).categoryName }}</td>
          <td>{{ (item as Category).description }}</td>
          <td>{{ (item as Category).auditCreateDate }}</td>
          <td>{{ (item as Category).statusCategory }}</td>
          <td>
            <v-btn v-if="canEdit && (item as Category).statusCategory == 'Activo'" color="indigo" icon="edit" variant="text"
              @click="editCategory(item)" size="small"></v-btn>
            <template v-if="canEdit && (item as Category).statusCategory == 'Inactivo'">
              <v-btn color="indigo" icon="check" variant="text" @click="openModal(item, 1)" size="small"></v-btn>
            </template>
            <template v-if="canEdit && (item as Category).statusCategory == 'Activo'">
              <v-btn color="indigo" icon="block" variant="text" @click="openModal(item, 2)" size="small"></v-btn>
            </template>
            <v-btn v-if="canDelete" color="indigo" icon="delete" variant="text" @click="openModal(item, 0)" size="small"></v-btn>
          </td>
        </tr>
      </template>
      <template v-slot:top>
        <v-toolbar>
          <v-toolbar-title>Gestión de Categorías</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-btn v-if="canRead" icon="download" @click="downloadExcel" :loading="downloadingExcel"></v-btn>
          <v-btn icon="tune" @click="drawer = !drawer"></v-btn>
          <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
            <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo" hide-details
              single-line v-model="search" @click:append-inner="searchCategories()"
              @keyup.enter="searchCategories()"></v-text-field>
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
  <CategoryFilters v-model="drawer" v-model:selected-filter="selectedFilter" v-model:state="state" v-model:start-date="startDate" v-model:end-date="endDate" @apply-filters="searchCategories" />
  <CategoryForm v-model="form" :category="selectedCategory" @saved="fetchCategories" />
  <CategoryModal v-model="modal" :category="selectedCategory" :action="action" @update:modelValue="modal = $event" />
</template>
<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Category } from '@/interfaces/categoryInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import CategoryForm from './CategoryForm.vue';
import CategoryModal from './CategoryModal.vue';
import CategoryFilters from './CategoryFilters.vue';

export default defineComponent({
  components: {
    CategoryForm,
    CategoryModal,
    CategoryFilters
  },
  data() {
    return {
      items: [10, 20, 50],
      currentPage: 1,
      itemsPerPage: 10,
      pages: "Categorías por Página",
      search: null as string | null,
      form: false,
      modal: false,
      selectedCategory: null as Category | null,
      action: 0,
      selectedFilter: 'Categoría',
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
        { title: 'Categoría', key: 'categoryName', sortable: false },
        { title: 'Descripción', key: 'description', sortable: false },
        { title: 'Fecha de registro', key: 'auditCreateDate', sortable: false },
        { title: 'Estado', key: 'statusCategory', sortable: false },
        { title: 'Acciones', key: 'actions', sortable: false },
      ];
    },
    totalPages() {
      return Math.ceil(this.store.getters['category/totalCategories'] / this.itemsPerPage);
    },
    categories() {
      return this.store.getters['category/categories'];
    },
    loading() {
      return this.store.getters['category/loading'];
    },
    totalCategories() {
      return this.store.getters['category/totalCategories'];
    },
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    },
    canCreate(): boolean {
      return this.$store.getters.hasPermission('categorias', 'crear');
    },
    canRead(): boolean {
      return this.$store.getters.hasPermission('categorias', 'leer');
    },
    canEdit(): boolean {
      return this.$store.getters.hasPermission('categorias', 'editar');
    },
    canDelete(): boolean {
      return this.$store.getters.hasPermission('categorias', 'eliminar');
    }
  },
  methods: {
    initialize() {
      this.fetchCategories();
    },
    openModal(category: any, action: number) {
      this.selectedCategory = category;
      this.action = action;
      this.modal = true;
    },
    openForm() {
      this.selectedCategory = {
        idCategory: null,
        categoryName: '',
        description: '',
        auditCreateDate: '',
        statusCategory: ''
      };
      this.form = true;
    },
    async fetchCategories() {
      try {
        await this.store.dispatch('category/fetchCategories', {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          stateFilter: this.stateFilter
        });
      } catch (error) {
        handleSilentError(error);
      }
    },
    async searchCategories() {
      let numberFilterValue: number | null = null;
      const filterMap: { [key: string]: number } = {
        "Categoría": 1,
        "Descripción": 2,
      }

      numberFilterValue = filterMap[this.selectedFilter];
      const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
      const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
      const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

      try {
        await this.store.dispatch("category/fetchCategories", {
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
        handleApiError(error, 'Error al buscar categorías');
      }
    },
    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;

      if (this.search && this.search.trim() !== "") {
        this.searchCategories();
      } else {
        this.fetchCategories();
      }
    },
    changePage(page: number) {
      this.currentPage = page;

      if (this.search && this.search.trim() !== "") {
        this.searchCategories();
      } else {
        this.fetchCategories();
      }
    },
    editCategory(category: any) {
      this.selectedCategory = { ...category };
      this.form = true;
    },
    async downloadExcel() {
      this.downloadingExcel = true;
      try {
        let numberFilterValue: number | null = null;
        const filterMap: { [key: string]: number } = {
          "Categoría": 1,
          "Descripción": 2,
        }

        numberFilterValue = filterMap[this.selectedFilter];
        const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
        const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
        const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

        await this.store.dispatch("category/downloadCategoriesExcel", {
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
    this.fetchCategories();
  },
  setup() {
    const store = useStore();
    return { store };
  }
});
</script>