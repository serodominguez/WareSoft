<template>
  <div>
    <CategoryList :categories="categories" :loading="loading" :totalCategories="totalCategories"
      :downloadingExcel="downloadingExcel" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-category="openForm"
      @fetch-categories="fetchCategories" @search-categories="searchCategories"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel" />

    <CategoryForm v-model="form" :category="selectedCategory" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedCategory?.idCategory || 0"
      :item="selectedCategory?.categoryName || ''" :action="action" moduleName="category" entityName="Category"
      name="Categoría" gender="female" @action-completed="handleActionCompleted" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Category } from '@/interfaces/categoryInterface';
import { formatDate } from '@/utils/date';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import CategoryList from '@/components/Category/CategoryList.vue';
import CategoryForm from '@/components/Category/CategoryForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

export default defineComponent({
  name: 'CategoryView',
  components: {
    CategoryList,
    CategoryForm,
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
      selectedCategory: null as Category | null,
      action: 0,
      selectedFilter: 'Categoría',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null,
      downloadingExcel: false
    };
  },
  computed: {
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
    openModal(payload: { category: Category, action: number }) {
      this.selectedCategory = payload.category;
      this.action = payload.action;
      this.modal = true;
    },

    openForm(category?: Category) {
      this.selectedCategory = category ? { ...category } : {
        idCategory: null,
        categoryName: '',
        description: '',
        auditCreateDate: '',
        statusCategory: ''
      };
      this.form = true;
    },

    async fetchCategories(params?: any) {
      try {
        await this.store.dispatch('category/fetchCategories', params || {
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
        "Categoría": 1,
        "Descripción": 2
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

    async searchCategories(params: any) {
      this.search = params.search;
      this.selectedFilter = params.selectedFilter;
      this.state = params.state;
      this.startDate = params.startDate;
      this.endDate = params.endDate;

      try {
        await this.store.dispatch("category/fetchCategories", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          ...this.getFilterParams(params)
        });
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar categorías');
      }
    },

    refreshCategories() {
      if (this.search?.trim()) {
        this.searchCategories({
          search: this.search,
          selectedFilter: this.selectedFilter,
          state: this.state,
          startDate: this.startDate,
          endDate: this.endDate
        });
      } else {
        this.fetchCategories();
      }
    },

    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;
      this.refreshCategories();
    },

    changePage(page: number) {
      this.currentPage = page;
      this.refreshCategories();
    },

    async downloadExcel(params: any) {
      this.downloadingExcel = true;
      try {
        await this.store.dispatch("category/downloadCategoriesExcel", {
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
      this.fetchCategories();
    },

    handleActionCompleted() {
      this.fetchCategories();
    }
  },
  mounted() {
    this.fetchCategories();
  }
});
</script>