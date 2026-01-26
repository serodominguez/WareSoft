<template>
  <div>
    <InventoryList :inventories="inventories" :loading="loading" :totalInventories="totalInventories"
      :downloadingExcel="downloadingExcel" :downloadingPdf="downloadingPdf" :canRead="canRead" :canEdit="canEdit"
      :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-inventory="openForm"
      @fetch-inventories="fetchInventories" @search-inventories="searchInventories" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf" />

    <PriceForm v-model="form" :inventory="selectedInventory" @saved="handleSaved" />

  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Inventory } from '@/interfaces/inventoryInterface';
import { formatDate } from '@/utils/date';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import InventoryList from '@/components/Inventory/InventoryList.vue';
import PriceForm from '@/components/Inventory/PriceForm.vue';

export default defineComponent({
  name: 'InventoryView',
  components: {
    InventoryList,
    PriceForm
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
      selectedInventory: null as Inventory | null,
      action: 0,
      selectedFilter: 'Código',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null,
      downloadingExcel: false,
      downloadingPdf: false
    };
  },
  computed: {
    inventories() {
      return this.store.getters['inventory/inventories'];
    },
    loading() {
      return this.store.getters['inventory/loading'];
    },
    totalInventories() {
      return this.store.getters['inventory/totalInventories'];
    },
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    },

    canRead(): boolean {
      return this.$store.getters.hasPermission('inventario', 'leer');
    },
    canEdit(): boolean {
      return this.$store.getters.hasPermission('inventario', 'editar');
    },
  },
  methods: {
    openModal(payload: { inventory: Inventory, action: number }) {
      this.selectedInventory = payload.inventory;
      this.action = payload.action;
      this.modal = true;
    },

    openForm(inventory?: Inventory) {
      this.selectedInventory = inventory ? { ...inventory } : {
        idStore: null,
        idProduct: null,
        code: '',
        description: '',
        material: '',
        color: '',
        unitMeasure: '',
        stock: null,
        price: null,
        brandName: '',
        categoryName: ''
      };
      this.form = true;
    },

    async fetchInventories(params?: any) {
      try {
        await this.store.dispatch('inventory/fetchInventories', params || {
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

    async searchInventories(params: any) {
      this.search = params.search;
      this.selectedFilter = params.selectedFilter;
      this.state = params.state;
      this.startDate = params.startDate;
      this.endDate = params.endDate;

      try {
        await this.store.dispatch("inventory/fetchInventories", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          ...this.getFilterParams(params)
        });
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar productos');
      }
    },

    refreshInventories() {
      if (this.search?.trim()) {
        this.searchInventories({
          search: this.search,
          selectedFilter: this.selectedFilter,
          state: this.state,
          startDate: this.startDate,
          endDate: this.endDate
        });
      } else {
        this.fetchInventories();
      }
    },

    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;
      this.refreshInventories();
    },

    changePage(page: number) {
      this.currentPage = page;
      this.refreshInventories();
    },

    async downloadExcel(params: any) {
      this.downloadingExcel = true;
      try {
        await this.store.dispatch("inventory/downloadInventoriesExcel", {
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

    async downloadPdf(params: any) {
      this.downloadingPdf = true;
      try {
        await this.store.dispatch("inventory/downloadInventoriesPdf", {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          ...this.getFilterParams(params)
        });
        this.toast.success('Archivo PDF descargado correctamente');
      } catch (error) {
        handleApiError(error, 'Error al descargar el archivo PDF');
      } finally {
        this.downloadingPdf = false;
      }
    },

    handleSaved() {
      this.fetchInventories();
    },

    handleActionCompleted() {
      this.fetchInventories();
    }
  },
  mounted() {
    this.fetchInventories();
  }
});
</script>