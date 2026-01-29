<template>
  <div>
    <GoodsIssueList v-if="!form" :goodsissue="goodsissue" :loading="loading"
      :totalGoodsIssue="totalGoodsIssue" :downloadingExcel="downloadingExcel" :canCreate="canCreate"
      :canRead="canRead" :canEdit="canEdit" :canDelete="canDelete" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal"
      @view-goodsissue="openForm" @fetch-goodsissue="fetchGoodsIssue" @search-goodsissue="searchGoodsIssue"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel"
      @print-pdf="printPdf" />

    <GoodsIssueForm v-if="form" v-model="form" :issue="selectedGoodsIssue"
      :issueDetails="selectedIssueDetails" @saved="handleSaved" @close="closeForm" />

    <CommonModal v-model="modal" :itemId="selectedGoodsIssue?.idIssue || 0" :item="selectedGoodsIssue?.code || ''"
      :action="action" moduleName="goodsissue" entityName="GoodsIssue" name="Salida" gender="female"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { GoodsIssue } from '@/interfaces/goodsIssueInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import GoodsIssueList from '@/components/GoodsIssue/GoodsIssueList.vue';
import GoodsIssueForm from '@/components/GoodsIssue/GoodsIssueForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

const store = useStore();
const toast = useToast();

const filterMap = {
  "Código": 1,
  "Tienda": 2,
  "Personal": 3
};

const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Código', filterMap);

const currentPage = ref(1);
const itemsPerPage = ref(10);
const search = ref<string | null>(null);
const drawer = ref(false);
const form = ref(false);
const modal = ref(false);
const selectedGoodsIssue = ref<GoodsIssue | null>(null);
const action = ref<0 | 1 | 2>(0);
const downloadingExcel = ref(false);

const goodsissue = computed(() => store.getters['goodsissue/goodsissue']);
const selectedIssueDetails = computed(() => store.getters['goodsissue/selectedIssueDetails']);
const loading = computed(() => store.getters['goodsissue/loading']);
const totalGoodsIssue = computed(() => store.getters['goodsissue/totalGoodsIssue']);
const stateFilter = computed(() => state.value === 'Activos' ? 1 : 0);
const canCreate = computed(() => store.getters.hasPermission('salida de productos', 'crear'));
const canRead = computed(() => store.getters.hasPermission('salida de productos', 'leer'));
const canEdit = computed(() => store.getters.hasPermission('salida de productos', 'editar'));
const canDelete = computed(() => store.getters.hasPermission('salida de productos', 'eliminar'));

const openModal = (payload: { goodsissue: GoodsIssue, action: 0 | 1 | 2 }) => {
  selectedGoodsIssue.value = payload.goodsissue;
  action.value = payload.action;
  modal.value = true;
};

const openForm = async (goodsissue?: GoodsIssue) => {
  if (goodsissue?.idIssue) {
    try {
      await store.dispatch('goodsissue/fetchGoodsIssueById', goodsissue.idIssue);
      selectedGoodsIssue.value = store.getters['goodsissue/selectedGoodsIssue'];
    } catch (error) {
      handleApiError(error, 'Error al cargar los detalles de la salida');
      return;
    }
  } else {
    selectedGoodsIssue.value = {
      idIssue: null,
      code: '',
      type: '',
      storeName: '',
      idUser: null,
      userName: '',
      totalAmount: 0,
      annotations: '',
      auditCreateDate: '',
      statusIssue: ''
    };
  }

  form.value = true;
};

const closeForm = () => {
  store.commit('goodsissue/SET_SELECTED_ISSUE_DETAILS', []);
  store.commit('goodsissue/SET_SELECTED_ITEM', null);
  selectedGoodsIssue.value = null;
  form.value = false;
};

const fetchGoodsIssue = async (params?: any) => {
  try {
    await store.dispatch('goodsissue/fetchGoodsIssue', params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      stateFilter: stateFilter.value,
      sort: 'IdIssue',
      order: 'desc'
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchGoodsIssue = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await store.dispatch("goodsissue/fetchGoodsIssue", {
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      sort: 'IdIssue',
      order: 'desc',
      ...getFilterParams(search.value)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar las salidas');
  }
};

const refreshGoodsIssue = () => {
  if (search.value?.trim()) {
    searchGoodsIssue({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchGoodsIssue();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshGoodsIssue();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshGoodsIssue();
};

const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await store.dispatch("goodsissue/downloadGoodsIssueExcel", {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      sort: 'IdIssue',
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

const printPdf = async (goodsissue: GoodsIssue) => {
  if (!goodsissue.idIssue) return;

  try {
    const result = await store.dispatch('goodsissue/openGoodsIssuePdf', goodsissue.idIssue);

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
  fetchGoodsIssue();
};

const handleActionCompleted = () => {
  fetchGoodsIssue();
};

onMounted(() => {
  fetchGoodsIssue();
});
</script>