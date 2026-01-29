<template>
  <div>
    <GoodsReceiptList v-if="!form" :goodsreceipt="goodsreceipt" :loading="loading"
      :totalGoodsReceipt="totalGoodsReceipt" :downloadingExcel="downloadingExcel" :canCreate="canCreate"
      :canRead="canRead" :canEdit="canEdit" :canDelete="canDelete" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal"
      @view-goodsreceipt="openForm" @fetch-goodsreceipt="fetchGoodsReceipt" @search-goodsreceipt="searchGoodsReceipt"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel"
      @print-pdf="printPdf" />

    <GoodsReceiptForm v-if="form" v-model="form" :receipt="selectedGoodsReceipt"
      :receiptDetails="selectedReceiptDetails" @saved="handleSaved" @close="closeForm" />

    <CommonModal v-model="modal" :itemId="selectedGoodsReceipt?.idReceipt || 0" :item="selectedGoodsReceipt?.code || ''"
      :action="action" moduleName="goodsreceipt" entityName="GoodsReceipt" name="Entrada" gender="female"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { GoodsReceipt } from '@/interfaces/goodsReceiptInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import GoodsReceiptList from '@/components/GoodsReceipt/GoodsReceiptList.vue';
import GoodsReceiptForm from '@/components/GoodsReceipt/GoodsReceiptForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const store = useStore();
const toast = useToast();

// Composable de filtros
const filterMap = {
  "Código": 1,
  "Tienda": 2,
  "Proveedor": 3
};

const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Código', filterMap);

// Data
const currentPage = ref(1);
const itemsPerPage = ref(10);
const search = ref<string | null>(null);
const drawer = ref(false);
const form = ref(false);
const modal = ref(false);
const selectedGoodsReceipt = ref<GoodsReceipt | null>(null);
const action = ref<0 | 1 | 2>(0);
const downloadingExcel = ref(false);

// Computed
const goodsreceipt = computed(() => store.getters['goodsreceipt/goodsreceipt']);
const selectedReceiptDetails = computed(() => store.getters['goodsreceipt/selectedReceiptDetails']);
const loading = computed(() => store.getters['goodsreceipt/loading']);
const totalGoodsReceipt = computed(() => store.getters['goodsreceipt/totalGoodsReceipt']);
const stateFilter = computed(() => state.value === 'Activos' ? 1 : 0);
const canCreate = computed(() => store.getters.hasPermission('entrada de productos', 'crear'));
const canRead = computed(() => store.getters.hasPermission('entrada de productos', 'leer'));
const canEdit = computed(() => store.getters.hasPermission('entrada de productos', 'editar'));
const canDelete = computed(() => store.getters.hasPermission('entrada de productos', 'eliminar'));

// Methods
const openModal = (payload: { goodsreceipt: GoodsReceipt, action: 0 | 1 | 2 }) => {
  selectedGoodsReceipt.value = payload.goodsreceipt;
  action.value = payload.action;
  modal.value = true;
};

const openForm = async (goodsreceipt?: GoodsReceipt) => {
  if (goodsreceipt?.idReceipt) {
    try {
      await store.dispatch('goodsreceipt/fetchGoodsReceiptById', goodsreceipt.idReceipt);
      selectedGoodsReceipt.value = store.getters['goodsreceipt/selectedGoodsReceipt'];
    } catch (error) {
      handleApiError(error, 'Error al cargar los detalles de la entrada');
      return;
    }
  } else {
    selectedGoodsReceipt.value = {
      idReceipt: null,
      code: '',
      type: '',
      storeName: '',
      documentType: '',
      documentNumber: '',
      documentDate: '',
      idSupplier: null,
      companyName: '',
      totalAmount: 0,
      annotations: '',
      auditCreateDate: '',
      statusReceipt: ''
    };
  }

  form.value = true;
};

const closeForm = () => {
  store.commit('goodsreceipt/SET_SELECTED_RECEIPT_DETAILS', []);
  store.commit('goodsreceipt/SET_SELECTED_ITEM', null);
  selectedGoodsReceipt.value = null;
  form.value = false;
};

const fetchGoodsReceipt = async (params?: any) => {
  try {
    await store.dispatch('goodsreceipt/fetchGoodsReceipt', params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      stateFilter: stateFilter.value,
      sort: 'IdReceipt',
      order: 'desc'
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchGoodsReceipt = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await store.dispatch("goodsreceipt/fetchGoodsReceipt", {
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      sort: 'IdReceipt',
      order: 'desc',
      ...getFilterParams(search.value)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar las entradas');
  }
};

const refreshGoodsReceipt = () => {
  if (search.value?.trim()) {
    searchGoodsReceipt({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchGoodsReceipt();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshGoodsReceipt();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshGoodsReceipt();
};

const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await store.dispatch("goodsreceipt/downloadGoodsReceiptExcel", {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      sort: 'IdReceipt',
      order: 'desc',
      ...getFilterParams(params.search)
    });
    toast.success('Archivo descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el archivo Excel');
  } finally {
    downloadingExcel.value = false;
  }
};

const printPdf = async (goodsreceipt: GoodsReceipt) => {
  if (!goodsreceipt.idReceipt) return;

  try {
    const result = await store.dispatch('goodsreceipt/openGoodsReceiptPdf', goodsreceipt.idReceipt);

    if (result.isSuccess) {
      toast.success('PDF abierto correctamente');
    } else {
      toast.error('Error al abrir el PDF');
    }
  } catch (error) {
    handleApiError(error, 'Error al abrir el PDF');
  }
};

const handleSaved = () => {
  fetchGoodsReceipt();
};

const handleActionCompleted = () => {
  fetchGoodsReceipt();
};

// Lifecycle
onMounted(() => {
  fetchGoodsReceipt();
});
</script>