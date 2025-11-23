<template>
  <div>
    <BrandList 
      :brands="brands"
      :loading="loading"
      :totalBrands="totalBrands"
      :downloadingExcel="downloadingExcel"
      :canCreate="canCreate"
      :canRead="canRead"
      :canEdit="canEdit"
      :canDelete="canDelete"
      v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter"
      v-model:state="state"
      v-model:startDate="startDate"
      v-model:endDate="endDate"
      @open-form="openForm"
      @open-modal="openModal"
      @edit-brand="openForm"
      @fetch-brands="fetchBrands"
      @search-brands="searchBrands"
      @update-items-per-page="updateItemsPerPage"
      @change-page="changePage"
      @download-excel="downloadExcel"
    />
    
    <BrandForm 
      v-model="form" 
      :brand="selectedBrand" 
      @saved="handleSaved" 
    />
    
    <BrandModal 
      v-model="modal" 
      :brand="selectedBrand" 
      :action="action" 
      @action-completed="handleActionCompleted"
    />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Brand } from '@/interfaces/brandInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import BrandList from '@/components/Brand/BrandList.vue';
import BrandForm from '@/components/Brand/BrandForm.vue';
import BrandModal from '@/components/Brand/BrandModal.vue';

export default defineComponent({
  name: 'BrandView',
  components: {
    BrandList,
    BrandForm,
    BrandModal
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
      selectedBrand: null as Brand | null,
      action: 0,
      selectedFilter: 'Marca',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null,
      downloadingExcel: false
    };
  },
  computed: {
    brands() {
      return this.store.getters['brand/brands'];
    },
    loading() {
      return this.store.getters['brand/loading'];
    },
    totalBrands() {
      return this.store.getters['brand/totalBrands'];
    },
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    },
    canCreate(): boolean {
      return this.$store.getters.hasPermission('marcas', 'crear');
    },
    canRead(): boolean {
      return this.$store.getters.hasPermission('marcas', 'leer');
    },
    canEdit(): boolean {
      return this.$store.getters.hasPermission('marcas', 'editar');
    },
    canDelete(): boolean {
      return this.$store.getters.hasPermission('marcas', 'eliminar');
    }
  },
  methods: {
    openModal(payload: { brand: Brand, action: number }) {
      this.selectedBrand = payload.brand;
      this.action = payload.action;
      this.modal = true;
    },
    
    openForm(brand?: Brand) {
      this.selectedBrand = brand ? { ...brand } : {
        idBrand: null,
        brandName: '',
        auditCreateDate: '',
        statusBrand: ''
      };
      this.form = true;
    },
    
    async fetchBrands(params?: any) {
      try {
        await this.store.dispatch('brand/fetchBrands', params || {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          stateFilter: this.stateFilter
        });
      } catch (error) {
        handleSilentError(error);
      }
    },
    
    getFilterParams(params: any) {
      const filterMap: { [key: string]: number } = { "Marca": 1 };
      const numberFilterValue = filterMap[params.selectedFilter || this.selectedFilter];
      const textFilterValue = params.search?.trim() || null;
      const startDateStr = params.startDate ? this.formatDate(params.startDate) : null;
      const endDateStr = params.endDate ? this.formatDate(params.endDate) : null;
      const stateFilter = typeof params.stateFilter === 'string' 
        ? (params.stateFilter === 'Activos' ? 1 : 0)
        : (params.state === 'Activos' ? 1 : 0);

      return {
        textFilter: textFilterValue,
        numberFilter: numberFilterValue,
        stateFilter,
        startDate: startDateStr,
        endDate: endDateStr
      };
    },
    
    async searchBrands(params: any) {
      this.search = params.search;
      this.selectedFilter = params.selectedFilter;
      this.state = params.state;
      this.startDate = params.startDate;
      this.endDate = params.endDate;

      try {
        await this.store.dispatch("brand/fetchBrands", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          ...this.getFilterParams(params)
        });
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar marcas');
      }
    },
    
    refreshBrands() {
      if (this.search?.trim()) {
        this.searchBrands({
          search: this.search,
          selectedFilter: this.selectedFilter,
          state: this.state,
          startDate: this.startDate,
          endDate: this.endDate
        });
      } else {
        this.fetchBrands();
      }
    },
    
    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;
      this.refreshBrands();
    },
    
    changePage(page: number) {
      this.currentPage = page;
      this.refreshBrands();
    },
    
    async downloadExcel(params: any) {
      this.downloadingExcel = true;
      try {
        await this.store.dispatch("brand/downloadBrandsExcel", {
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
    
    formatDate(date: Date | null): string | null {
      if (!date) return null;
      
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const day = String(date.getDate()).padStart(2, '0');

      return `${year}-${month}-${day}`;
    },
    
    handleSaved() {
      this.fetchBrands();
    },
    
    handleActionCompleted() {
      this.fetchBrands();
    }
  },
  mounted() {
    this.fetchBrands();
  }
});
</script>