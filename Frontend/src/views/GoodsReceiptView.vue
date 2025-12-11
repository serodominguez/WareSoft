<template>
  <div>
    <GoodsReceiptList :goodsreceipt="goodsreceipt" :loading="loading" :totalGoodsReceipt="totalGoodsReceipt" :downloadingExcel="downloadingExcel"
      :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit" :canDelete="canDelete" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal"
      @view-goodsreceipt="openForm" @fetch-goodsreceipt="fetchGoodsReceipt" @search-goodsreceipt="searchGoodsReceipt"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel" />


    <CommonModal v-model="modal" :itemId="selectedGoodsReceipt?.idReceipt || 0" :item="selectedGoodsReceipt?.code || ''"
      :action="action" moduleName="goodsreceipt" entityName="GoodsReceipt" name="Ingreso" gender="male"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { GoodsReceipt } from '@/interfaces/goodsReceiptInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import GoodsReceiptList from '@/components/GoodsReceipt/GoodsReceiptList.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

export default defineComponent({
  name: 'GoodsReceiptView',
  components: {
    GoodsReceiptList,
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
      selectedFilter: 'Código',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null,
      form: false,
      modal: false,
      selectedGoodsReceipt: null as GoodsReceipt | null,
      action: 0, 
      downloadingExcel: false
    };
  },
  computed: {
    goodsreceipt() {
      return this.store.getters['goodsreceipt/goodsreceipt'];
    },
    loading() {
      return this.store.getters['goodsreceipt/loading'];
    },
    totalGoodsReceipt() {
      return this.store.getters['goodsreceipt/totalGoodsReceipt'];
    },
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    },
    canCreate(): boolean {
      return this.$store.getters.hasPermission('ingreso de productos', 'crear');
    },
    canRead(): boolean {
      return this.$store.getters.hasPermission('ingreso de productos', 'leer');
    },
    canEdit(): boolean {
      return this.$store.getters.hasPermission('ingreso de productos', 'editar');
    },
    canDelete(): boolean {
      return this.$store.getters.hasPermission('ingreso de productos', 'eliminar');
    }
  },
  methods: {
    openModal(payload: { goodsreceipt: GoodsReceipt, action: number }) {
      this.selectedGoodsReceipt = payload.goodsreceipt;
      this.action = payload.action;
      this.modal = true;
    },
    openForm(goodsreceipt?: GoodsReceipt) {
      this.selectedGoodsReceipt = goodsreceipt ? { ...goodsreceipt } : {
        idReceipt: null,
        code: '',
        type: '',
        storeName: '',
        companyName: '',
        documentDate: '',
        documentType: '',
        auditCreateDate: '',
        statusReceipt: ''
      };
      this.form = true;
    },
    async fetchGoodsReceipt(params?: any) {
      try {
        await this.store.dispatch('goodsreceipt/fetchGoodsReceipt', params || {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          stateFilter: this.stateFilter,
          sort: 'IdReceipt',
          order: 'asc' 
        });
      } catch (error) {
        handleSilentError(error);
      }
    },
    getFilterParams(params: any) {
      const filterMap: { [key: string]: number } = { 
        "Código": 1,
        "Tienda": 2,
        "Proveedor": 3 
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
    async searchGoodsReceipt(params: any) {
      this.search = params.search;
      this.selectedFilter = params.selectedFilter;
      this.state = params.state;
      this.startDate = params.startDate;
      this.endDate = params.endDate;

      try {
        await this.store.dispatch("goodsreceipt/fetchGoodsReceipt", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          sort: 'IdReceipt',
          order: 'asc', 
          ...this.getFilterParams(params)
        });
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar ingresos');
      }
    },
    refreshGoodsReceipt() {
      if (this.search?.trim()) {
        this.searchGoodsReceipt({
          search: this.search,
          selectedFilter: this.selectedFilter,
          state: this.state,
          startDate: this.startDate,
          endDate: this.endDate
        });
      } else {
        this.fetchGoodsReceipt();
      }
    },
    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;
      this.refreshGoodsReceipt();
    },
    changePage(page: number) {
      this.currentPage = page;
      this.refreshGoodsReceipt();
    },
    async downloadExcel(params: any) {
      this.downloadingExcel = true;
      try {
        await this.store.dispatch("goodsreceipt/downloadGoodsReceiptExcel", {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          sort: 'IdReceipt',
          order: 'asc', 
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
      this.fetchGoodsReceipt();
    },
    handleActionCompleted() {
      this.fetchGoodsReceipt();
    }
  },
  mounted() {
    this.fetchGoodsReceipt();
  }
});
</script>