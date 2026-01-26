<template>
  <v-dialog v-model="isOpen" max-width="1200px" persistent>
    <v-card elevation="2">
      <v-card-title class="bg-surface-light pt-4">
        <span>Seleccione el Producto</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-row justify="center" align="end">
          <v-col cols="4" md="2" lg="2" xl="2" class="mb-2">
            <v-select color="indigo" variant="underlined" v-model="selectedFilter" :items="filterOptions"
              label="Opciones" hide-details />
          </v-col>
          <v-col cols="8" md="6" lg="6" xl="6" class="mb-2">
            <v-text-field append-inner-icon="search" density="compact" label="Búsqueda" variant="underlined"
              hide-details single-line v-model="search" @click:append-inner="searchProducts"
              @keyup.enter="searchProducts" />
          </v-col>
        </v-row>
        <v-data-table-server :headers="headers" :items="products" :items-per-page-options="[5, 10]"
          :items-per-page-text="pages" :items-per-page="itemsPerPage" :items-length="totalProducts" :loading="loading"
          loading-text="Cargando... Espere por favor" @update:items-per-page="updateItemsPerPage"
          @update:page="changePage">
          <template v-slot:item="{ item }">
            <tr>
              <td>{{ item.code }}</td>
              <td>{{ item.description }}</td>
              <td>{{ item.material }}</td>
              <td>{{ item.color }}</td>
              <td>{{ item.categoryName }}</td>
              <td>{{ item.brandName }}</td>
              <td class="text-center">
                <v-btn color="blue" icon="add" variant="text" @click="addProduct(item)" size="small" title="Agregar" />
              </td>
            </tr>
          </template>
          <template v-slot:no-data>
            <div class="text-center py-4">
              <p v-if="!loading">{{ noDataMessage }}</p>
            </div>
          </template>
        </v-data-table-server>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="red" dark class="mb-2" elevation="4" @click="close">Cerrar</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { Product } from '@/interfaces/productInterface';
import { handleApiError } from '@/helpers/errorHandler';

export default defineComponent({
  name: 'CommonProductIn',
  props: {
    modelValue: {
      type: Boolean,
      required: true
    }
  },
  emits: ['update:modelValue', 'close', 'product-added'],
  setup() {
    const store = useStore();
    return { store };
  },
  data() {
    return {
      pages: "Productos por Página",
      currentPage: 1,
      itemsPerPage: 5,
      search: null as string | null,
      selectedFilter: '',
      filterOptions: ['Código', 'Descripción', 'Material', 'Color', 'Categoría', 'Marca'],
      hasSearched: false,
      products: [] as Product[],
      totalProducts: 0,
      loading: false
    };
  },
  computed: {
    headers(): Array<{ title: string; key: string; sortable: boolean; align?: 'start' | 'end' | 'center' }> {
      return [
        { title: 'Código', key: 'code', sortable: false },
        { title: 'Descripción', key: 'description', sortable: false },
        { title: 'Material', key: 'material', sortable: false },
        { title: 'Color', key: 'color', sortable: false },
        { title: 'Categoría', key: 'categoryName', sortable: false },
        { title: 'Marca', key: 'brandName', sortable: false },
        { title: 'Agregar', key: 'actions', sortable: false, align: 'center' },
      ];
    },
    isOpen: {
      get() {
        return this.modelValue;
      },
      set(value: boolean) {
        this.$emit('update:modelValue', value);
      }
    },
    noDataMessage(): string {
      return this.hasSearched ? 'No hay productos para mostrar' : 'Realice una búsqueda para ver los productos';
    }
  },
  methods: {
    close() {
      this.resetModal();
      this.$emit('update:modelValue', false);
      this.$emit('close');
    },
    // Limpia todos los campos y la tabla
    resetModal() {
      this.search = null;
      this.selectedFilter = '';
      this.currentPage = 1;
      this.itemsPerPage = 5;
      this.hasSearched = false;
      this.products = [];
      this.totalProducts = 0;
    },
    // Obtiene los parámetros de filtro
    getFilterParams() {
      const filterMap: { [key: string]: number } = {
        "Código": 1,
        "Descripción": 2,
        "Material": 3,
        "Color": 4,
        "Marca": 5,
        "Categoría": 6
      };

      return {
        textFilter: this.search?.trim() || null,
        numberFilter: filterMap[this.selectedFilter],
        stateFilter: 1
      };
    },
    // Busca productos con filtros
    async searchProducts() {
      this.currentPage = 1;
      await this.fetchProducts();
    },
    // Obtiene productos maneja estado local
    async fetchProducts() {
      try {
        this.loading = true;
        const response = await this.store.dispatch('product/fetchProducts', {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          ...this.getFilterParams()
        });

        // Obtiene los datos del store después de la acción
        this.products = this.store.getters['product/products'] || [];
        this.totalProducts = this.store.getters['product/totalProducts'] || 0;
        this.hasSearched = true;
      } catch (error) {
        handleApiError(error, 'Error al buscar productos');
        this.products = [];
        this.totalProducts = 0;
      } finally {
        this.loading = false;
      }
    },
    // Actualiza items por página
    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;
      this.fetchProducts();
    },
    // Refresca la lista manteniendo filtros
    changePage(page: number) {
      this.currentPage = page;
      this.fetchProducts();
    },
    // Agrega un producto seleccionado
    addProduct(product: Product) {
      this.$emit('product-added', product);
    }
  }
});
</script>