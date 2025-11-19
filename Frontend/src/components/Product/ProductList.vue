<template>
  <v-card elevation="2">
    <v-data-table-server :headers="headers" :items="products" :search="search || undefined" :items-per-page-text="pages"
      :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage" :items-length="totalProducts"
      :loading="loading" loading-text="Cargando... Espere por favor" @update:items-per-page="updateItemsPerPage"
      @update:page="changePage">
      <template v-slot:item="{ item }">
        <tr>
          <td>{{ (item as Product).code }}</td>
          <td>{{ (item as Product).description }}</td>
          <td>{{ (item as Product).material }}</td>
          <td>{{ (item as Product).color }}</td>
          <td>{{ (item as Product).brandName }}</td>
          <td>{{ (item as Product).categoryName }}</td>
          <td>{{ (item as Product).auditCreateDate }}</td>
          <td>{{ (item as Product).statusProduct }}</td>
          <td>
            <v-btn v-if="canEdit && (item as Product).statusProduct == 'Activo'" color="indigo" icon="edit" variant="text"
              @click="editProduct(item)" size="small"></v-btn>
            <template v-if="canEdit && (item as Product).statusProduct == 'Inactivo'">
              <v-btn color="indigo" icon="check" variant="text" @click="openModal(item, 1)" size="small"></v-btn>
            </template>
            <template v-if="canEdit && (item as Product).statusProduct == 'Activo'">
              <v-btn color="indigo" icon="block" variant="text" @click="openModal(item, 2)" size="small"></v-btn>
            </template>
            <v-btn v-if="canDelete" color="indigo" icon="delete" variant="text" @click="openModal(item, 0)" size="small"></v-btn>
          </td>
        </tr>
      </template>
      <template v-slot:top>
        <v-toolbar>
          <v-toolbar-title>Gestión de Productos</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-btn v-if="canRead" icon="download" @click="downloadExcel" :loading="downloadingExcel"></v-btn>
          <v-btn icon="tune" @click="drawer = !drawer"></v-btn>
          <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
            <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo" hide-details
              single-line v-model="search" @click:append-inner="searchProducts()"
              @keyup.enter="searchProducts()"></v-text-field>
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
  <ProductFilters v-model="drawer" v-model:selected-filter="selectedFilter" v-model:state="state" v-model:start-date="startDate" v-model:end-date="endDate" @apply-filters="searchProducts" />
  <ProductForm v-model="form" :product="selectedProduct" @saved="fetchProducts" />
  <ProductModal v-model="modal" :product="selectedProduct" :action="action" @update:modelValue="modal = $event" />
</template>
<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { Product } from '@/interfaces/productInterface';
import ProductForm from './ProductForm.vue';
import ProductModal from './ProductModal.vue';
import ProductFilters from './ProductFilters.vue';

export default defineComponent({
  components: {
    ProductForm,
    ProductModal, 
    ProductFilters
  },
  data() {
    return {
      items: [10, 20, 50],
      currentPage: 1,
      itemsPerPage: 10,
      pages: "Productos por Página",
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
      downloadingExcel: false,
    };
  },
  computed: {
    headers() {
      return [
        { title: 'Código', key: 'code', sortable: false  },
        { title: 'Descripción', key: 'description', sortable: false  },
        { title: 'Material', key: 'material', sortable: false  },
        { title: 'Color', key: 'color', sortable: false  },
        { title: 'Marca', key: 'brandName', sortable: false  },
        { title: 'Categoría', key: 'categoryName', sortable: false  },
        { title: 'Fecha registro', key: 'auditCreateDate', sortable: false  },
        { title: 'Estado', key: 'statusProduct', sortable: false },
        { title: 'Acciones', key: 'actions', sortable: false },
      ];
    },
    totalPages() {
      return Math.ceil(this.store.getters['product/totalProducts'] / this.itemsPerPage);
    },
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
    initialize() {
      this.fetchProducts();
    },
    openModal(product: any, action: number) {
      this.selectedProduct = product;
      this.action = action;
      this.modal = true;
    },
    openForm() {
      this.selectedProduct = {
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
    async fetchProducts() {
      await this.store.dispatch('product/fetchProducts', {
        pageNumber: this.currentPage,
        pageSize: this.itemsPerPage,
        stateFilter: this.stateFilter
      });
    },
    async searchProducts() {
      let numberFilterValue: number | null = null;
      const filterMap: { [key: string]: number } = {
        "Código": 1,
        "Descripción": 2,
        "Material": 3,
        "Color": 4,
        "Marca": 5,
        "Categoría": 6,
      };

      numberFilterValue = filterMap[this.selectedFilter];
      const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
      const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
      const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

      await this.store.dispatch("product/fetchProducts", {
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
        this.searchProducts();
      } else {
        this.fetchProducts();
      }
    },
    changePage(page: number) {
      this.currentPage = page;

      if (this.search && this.search.trim() !== "") {
        this.searchProducts();
      } else {
        this.fetchProducts();
      }
    },
    editProduct(product: any) {
      this.selectedProduct = { ...product };
      this.form = true;
    },
    async downloadExcel() {
      this.downloadingExcel = true;
      try {
      let numberFilterValue: number | null = null;
      const filterMap: { [key: string]: number } = {
        "Código": 1,
        "Descripción": 2,
        "Material": 3,
        "Color": 4,
        "Marca": 5,
        "Categoría": 6,
      }

        numberFilterValue = filterMap[this.selectedFilter];
        const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
        const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
        const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

        await this.store.dispatch("product/downloadProductsExcel", {
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
    this.fetchProducts();
  },
  setup() {
    const store = useStore();
    return { store };
  }
});
</script>
