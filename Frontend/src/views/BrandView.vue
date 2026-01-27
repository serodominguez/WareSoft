<template>
  <div>
    <BrandList :brands="brands" :loading="loading" :totalBrands="totalBrands" :downloadingExcel="downloadingExcel"
      :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit" :canDelete="canDelete" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal"
      @edit-brand="openForm" @fetch-brands="fetchBrands" @search-brands="searchBrands"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel" />

    <BrandForm v-model="form" :brand="selectedBrand" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedBrand?.idBrand || 0" :item="selectedBrand?.brandName || ''"
      :action="action" moduleName="brand" entityName="Brand" name="Marca" gender="female"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Brand } from '@/interfaces/brandInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import { useFilters } from '@/composables/useFilters';
import BrandList from '@/components/Brand/BrandList.vue';
import BrandForm from '@/components/Brand/BrandForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

// Inicialización del store de Vuex y el sistema de notificaciones toast
const store = useStore();
const toast = useToast();

// Composable de filtros
const filterMap: Record<string, number> = { "Marca": 1 };
const { selectedFilter, state, startDate, endDate, getFilterParams } = useFilters('Marca', filterMap);

// Control de paginación
const currentPage = ref(1);
const itemsPerPage = ref(10);

// Control de búsqueda
const search = ref<string | null>(null);
const drawer = ref(false);

// Control de modales y formularios
const form = ref(false);
const modal = ref(false);

// Marca seleccionada para edición o eliminación
const selectedBrand = ref<Brand | null>(null);

// Tipo de acción a realizar en el modal (0: eliminar, 1: activar, 2: desactivar)
const action = ref<0 | 1 | 2>(0);

// Estado de descarga de Excel
const downloadingExcel = ref(false);

// Computed properties
const brands = computed(() => store.getters['brand/brands']);
const loading = computed(() => store.getters['brand/loading']);
const totalBrands = computed(() => store.getters['brand/totalBrands']);

// Permisos del usuario para diferentes acciones en el módulo de marcas
const canCreate = computed((): boolean => store.getters.hasPermission('marcas', 'crear'));
const canRead = computed((): boolean => store.getters.hasPermission('marcas', 'leer'));
const canEdit = computed((): boolean => store.getters.hasPermission('marcas', 'editar'));
const canDelete = computed((): boolean => store.getters.hasPermission('marcas', 'eliminar'));

// Abre el modal de confirmación para acciones sobre marcas
const openModal = (payload: { brand: Brand, action: 0 | 1 | 2 }) => {
  selectedBrand.value = payload.brand;
  action.value = payload.action;
  modal.value = true;
};

// Abre el formulario para crear o editar una marca
const openForm = (brand?: Brand) => {
  selectedBrand.value = brand ? { ...brand } : {
    idBrand: null,
    brandName: '',
    auditCreateDate: '',
    statusBrand: ''
  };
  form.value = true;
};

// Obtiene la lista de marcas desde el servidor
const fetchBrands = async (params?: any) => {
  try {
    await store.dispatch('brand/fetchBrands', params || {
      pageNumber: currentPage.value,
      pageSize: itemsPerPage.value,
      stateFilter: state.value === 'Activos' ? 1 : 0
    });
  } catch (error) {
    // Manejo silencioso del error (no muestra notificación al usuario)
    handleSilentError(error);
  }
};

// Realiza una búsqueda de marcas con los parámetros especificados
const searchBrands = async (params: any) => {
  // Actualiza los valores locales de búsqueda y filtros
  search.value = params.search;
  selectedFilter.value = params.selectedFilter;
  state.value = params.state;
  startDate.value = params.startDate;
  endDate.value = params.endDate;

  try {
    await store.dispatch("brand/fetchBrands", {
      pageNumber: 1,
      pageSize: itemsPerPage.value,
      ...getFilterParams(params.search)
    });
    // Resetea a la primera página después de una búsqueda
    currentPage.value = 1;
  } catch (error) {
    handleApiError(error, 'Error al buscar marcas');
  }
};

// Refresca la lista de marcas manteniendo los filtros actuales
const refreshBrands = () => {
  // Si hay una búsqueda activa, la ejecuta; si no, obtiene todas las marcas
  if (search.value?.trim()) {
    searchBrands({
      search: search.value,
      selectedFilter: selectedFilter.value,
      state: state.value,
      startDate: startDate.value,
      endDate: endDate.value
    });
  } else {
    fetchBrands();
  }
};

// Actualiza el número de items por página y refresca la lista
const updateItemsPerPage = (newItemsPerPage: number) => {
  itemsPerPage.value = newItemsPerPage;
  currentPage.value = 1; // Resetea a la primera página
  refreshBrands();
};

// Cambia la página actual y refresca la lista
const changePage = (page: number) => {
  currentPage.value = page;
  refreshBrands();
};

// Descarga un archivo Excel con los datos de las marcas filtradas
const downloadExcel = async (params: any) => {
  downloadingExcel.value = true;
  try {
    await store.dispatch("brand/downloadBrandsExcel", {
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

// Manejador del evento después de guardar una marca
const handleSaved = () => {
  fetchBrands();
};

// Manejador del evento después de completar una acción en el modal
const handleActionCompleted = () => {
  fetchBrands();
};

// Hook que se ejecuta cuando el componente es montado en el DOM
onMounted(() => {
  // Carga inicial de las marcas
  fetchBrands();
});
</script>