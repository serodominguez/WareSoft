<template>
  <div>
    <InventoryList :inventories="inventories" :loading="loading" :totalInventories="totalInventories"
      :downloadingExcel="downloadingExcel" :downloadingPdf="downloadingPdf" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit" :canDelete="canDelete"
      :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-inventory="openForm"
      @fetch-inventories="fetchInventories" @search-inventories="searchInventories" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" @download-pdf="downloadPdf" />

    <PriceForm v-model="form" :inventory="selectedInventory" @saved="handleSaved" />

  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import { useInventoryStore } from '@/stores/inventoryStore';
import { useAuthStore } from '@/stores/auth';
import { Inventory } from '@/interfaces/inventoryInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import InventoryList from '@/components/Inventory/InventoryList.vue';
import PriceForm from '@/components/Inventory/PriceForm.vue';

// Inicialización
const inventoryStore = useInventoryStore();
const authStore = useAuthStore();
const toast = useToast();

// Composable de filtros
const filterMap: Record<string, number> = {
  "Código": 1,
  "Descripción": 2,
  "Material": 3,
  "Color": 4,
  "Marca": 5,
  "Categoría": 6
};
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Código', filterMap);

// Control de paginación
const currentPage = ref(1);
const itemsPerPage = ref(10);

// Control de búsqueda
const search = ref<string | null>(null);
const drawer = ref(false);

// Control de modales y formularios
const form = ref(false);
const modal = ref(false);

// Inventario seleccionado
const selectedInventory = ref<Inventory | null>(null);

// Tipo de acción
const action = ref<0 | 1 | 2>(0);

// Estado de descargas
const downloadingExcel = ref(false);
const downloadingPdf = ref(false);

// Computed properties
const inventories = computed(() => inventoryStore.inventories);
const loading = computed(() => inventoryStore.loading);
const totalInventories = computed(() => inventoryStore.totalInventories);

const stateFilter = computed<number>(() => state.value === 'Activos' ? 1 : 0);

// Permisos
const canCreate = computed(() => authStore.hasPermission('inventario', 'crear'));
const canRead = computed((): boolean => authStore.hasPermission('inventario', 'leer'));
const canEdit = computed((): boolean => authStore.hasPermission('inventario', 'editar'));
const canDelete = computed(() => authStore.hasPermission('inventario', 'eliminar'));

// Métodos
const openModal = (payload: { inventory: Inventory; action: 0 | 1 | 2 }) => {
  selectedInventory.value = payload.inventory;
  action.value = payload.action;
  modal.value = true;
};

const openForm = (inventory?: Inventory) => {
  selectedInventory.value = inventory ? { ...inventory } : {
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
  form.value = true;
};

const fetchInventories = async (params?: any) => {
  try {
    await inventoryStore.fetchInventories(params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      stateFilter: stateFilter.value
    });
  } catch (error) {
    handleSilentError(error);
  }
};

const searchInventories = async (params: any) => {
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await inventoryStore.fetchInventories({
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar productos');
  }
};

const refreshInventories = () => {
  if (search.value?.trim()) {
    searchInventories({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchInventories();
  }
};

const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1;
  refreshInventories();
};

const changePage = (page: number) => {
  currentPage.value = page;
  refreshInventories();
};

const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await inventoryStore.downloadInventoriesExcel({
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    toast.success('Archivo descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el archivo Excel');
  } finally {
    downloadingExcel.value = false;
  }
};

const downloadPdf = async (params: any) => {
  downloadingPdf.value = true;
  try {
    await inventoryStore.downloadInventoriesPdf({
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    toast.success('Archivo PDF descargado correctamente');
  } catch (error) {
    handleApiError(error, 'Error al descargar el archivo PDF');
  } finally {
    downloadingPdf.value = false;
  }
};

const handleSaved = () => {
  fetchInventories();
};

const handleActionCompleted = () => {
  fetchInventories();
};

// Lifecycle
onMounted(() => {
  fetchInventories();
});
</script>