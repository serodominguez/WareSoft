<template>
  <div>
    <StoreList :stores="stores" :loading="loading" :totalStores="totalStores" :downloadingExcel="downloadingExcel"
      :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit" :canDelete="canDelete" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-store="openForm"
      @fetch-stores="fetchStores" @search-stores="searchStores" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" />

    <StoreForm v-model="form" :store="selectedStore" @saved="handleSaved" />

    <StoreModal v-model="modal" :store="selectedStore" :action="action" @action-completed="handleActionCompleted" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Store } from '@/interfaces/storeInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import StoreList from '@/components/Store/StoreList.vue';
import StoreForm from '@/components/Store/StoreForm.vue';
import StoreModal from '@/components/Store/StoreModal.vue';

export default defineComponent({
  name: 'StoreView',
  components: {
    StoreList,
    StoreForm,
    StoreModal
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
      selectedStore: null as Store | null,
      action: 0,
      selectedFilter: 'Tienda',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null,
      downloadingExcel: false
    };
  },
  computed: {
    stores() {
      return this.store.getters['store/stores'];
    },
    loading() {
      return this.store.getters['store/loading'];
    },
    totalStores() {
      return this.store.getters['store/totalStores'];
    },
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    },
    canCreate(): boolean {
      return this.$store.getters.hasPermission('tiendas', 'crear');
    },
    canRead(): boolean {
      return this.$store.getters.hasPermission('tiendas', 'leer');
    },
    canEdit(): boolean {
      return this.$store.getters.hasPermission('tiendas', 'editar');
    },
    canDelete(): boolean {
      return this.$store.getters.hasPermission('tiendas', 'eliminar');
    }
  },
  methods: {
    openModal(payload: { store: Store, action: number }) {
      this.selectedStore = payload.store;
      this.action = payload.action;
      this.modal = true;
    },

    openForm(store?: Store) {
      this.selectedStore = store ? { ...store } : {
        idStore: null,
        storeName: '',
        manager: '',
        address: '',
        phoneNumber: null,
        city: '',
        email: '',
        type: '',
        auditCreateDate: '',
        statusStore: ''
      };
      this.form = true;
    },

    async fetchStores(params?: any) {
      try {
        await this.store.dispatch('store/fetchStores', params || {
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
        "Tienda": 1,
        "Encargado": 2,
        "Dirección": 3,
        "Ciudad": 4
      };
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

    async searchStores(params: any) {
      this.search = params.search;
      this.selectedFilter = params.selectedFilter;
      this.state = params.state;
      this.startDate = params.startDate;
      this.endDate = params.endDate;

      try {
        await this.store.dispatch("store/fetchStores", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          ...this.getFilterParams(params)
        });
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar tiendas');
      }
    },

    refreshStores() {
      if (this.search?.trim()) {
        this.searchStores({
          search: this.search,
          selectedFilter: this.selectedFilter,
          state: this.state,
          startDate: this.startDate,
          endDate: this.endDate
        });
      } else {
        this.fetchStores();
      }
    },

    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;
      this.refreshStores();
    },

    changePage(page: number) {
      this.currentPage = page;
      this.refreshStores();
    },

    async downloadExcel(params: any) {
      this.downloadingExcel = true;
      try {
        await this.store.dispatch("store/downloadStoresExcel", {
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
      this.fetchStores();
    },

    handleActionCompleted() {
      this.fetchStores();
    }
  },
  mounted() {
    this.fetchStores();
  }
});
</script>
