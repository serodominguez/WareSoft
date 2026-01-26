<template>
  <div>
    <ProductList :products="products" :loading="loading" :totalProducts="totalProducts"
      :downloadingExcel="downloadingExcel" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-product="openForm"
      @fetch-products="fetchProducts" @search-products="searchProducts" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" />

    <ProductForm v-model="form" :product="selectedProduct" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedProduct?.idProduct || 0" :item="selectedProduct?.description || ''"
      :action="action" moduleName="product" entityName="Product" name="Producto" gender="male"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Product } from '@/interfaces/productInterface';
import { formatDate } from '@/utils/date';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import ProductList from '@/components/Product/ProductList.vue';
import ProductForm from '@/components/Product/ProductForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

export default defineComponent({
  name: 'ProductView',
  components: {
    ProductList,
    ProductForm,
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
      selectedProduct: null as Product | null,
      action: 0,
      selectedFilter: 'Código',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null,
      downloadingExcel: false
    };
  },
  computed: {
    products() {
      return this.store.getters['product/products'];
    },
    loading() {
      return this.store.getters['product/loading'];
    },
    totalProducts() {
      return this.store.getters['product/totalProducts'];
    },
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    },
    canCreate(): boolean {
      return this.$store.getters.hasPermission('productos', 'crear');
    },
    canRead(): boolean {
      return this.$store.getters.hasPermission('productos', 'leer');
    },
    canEdit(): boolean {
      return this.$store.getters.hasPermission('productos', 'editar');
    },
    canDelete(): boolean {
      return this.$store.getters.hasPermission('productos', 'eliminar');
    }
  },
  methods: {
    openModal(payload: { product: Product, action: number }) {
      this.selectedProduct = payload.product;
      this.action = payload.action;
      this.modal = true;
    },

    openForm(product?: Product) {
      this.selectedProduct = product ? { ...product } : {
        idProduct: null,
        code: '',
        description: '',
        material: '',
        color: '',
        unitMeasure: '',
        idBrand: null,
        brandName: '',
        idCategory: null,
        categoryName: '',
        auditCreateDate: '',
        statusProduct: ''
      };
      this.form = true;
    },

    async fetchProducts(params?: any) {
      try {
        await this.store.dispatch('product/fetchProducts', params || {
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
        "Código": 1,
        "Descripción": 2,
        "Material": 3,
        "Color": 4,
        "Marca": 5,
        "Categoría": 6
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

    async searchProducts(params: any) {
      this.search = params.search;
      this.selectedFilter = params.selectedFilter;
      this.state = params.state;
      this.startDate = params.startDate;
      this.endDate = params.endDate;

      try {
        await this.store.dispatch("product/fetchProducts", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          ...this.getFilterParams(params)
        });
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar productos');
      }
    },

    refreshProducts() {
      if (this.search?.trim()) {
        this.searchProducts({
          search: this.search,
          selectedFilter: this.selectedFilter,
          state: this.state,
          startDate: this.startDate,
          endDate: this.endDate
        });
      } else {
        this.fetchProducts();
      }
    },

    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;
      this.refreshProducts();
    },

    changePage(page: number) {
      this.currentPage = page;
      this.refreshProducts();
    },

    async downloadExcel(params: any) {
      this.downloadingExcel = true;
      try {
        await this.store.dispatch("product/downloadProductsExcel", {
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
      this.fetchProducts();
    },

    handleActionCompleted() {
      this.fetchProducts();
    }
  },
  mounted() {
    this.fetchProducts();
  }
});
</script>