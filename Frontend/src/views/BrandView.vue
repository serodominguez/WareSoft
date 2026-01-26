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

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Brand } from '@/interfaces/brandInterface';
import { formatDate } from '@/utils/date';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import BrandList from '@/components/Brand/BrandList.vue';
import BrandForm from '@/components/Brand/BrandForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

export default defineComponent({
  name: 'BrandView',
  components: {
    BrandList,
    BrandForm,
    CommonModal
  },
  setup() {
    // Inicialización del store de Vuex y el sistema de notificaciones toast
    const store = useStore();
    const toast = useToast();
    return { store, toast };
  },
  data() {
    return {
      // Control de paginación
      currentPage: 1,
      itemsPerPage: 10,
      // Control de búsqueda y filtros
      search: null as string | null,
      selectedFilter: 'Marca',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null,
      // Control de modales y formularios
      form: false,
      modal: false,
      // Marca seleccionada para edición o eliminación
      selectedBrand: null as Brand | null,
      // Tipo de acción a realizar en el modal (0: eliminar, etc.)
      action: 0, 

      // Estado de descarga de Excel
      downloadingExcel: false
    };
  },
  computed: {
    // Obtiene la lista de marcas desde el store de Vuex
    brands() {
      return this.store.getters['brand/brands'];
    },
    // Estado de carga desde el store
    loading() {
      return this.store.getters['brand/loading'];
    },
    // Total de marcas para la paginación
    totalBrands() {
      return this.store.getters['brand/totalBrands'];
    },
    // Convierte el filtro de estado textual a numérico (1: Activos, 0: Inactivos)
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    },
    // Permisos del usuario para diferentes acciones en el módulo de marcas
    canCreate(): boolean {
      return this.$store.getters.hasPermission('marcas', 'crear');
    },
    canRead(): boolean {
      return this.$store.getters.hasPermission('marcas', 'leer');
    },
    canEdit(): boolean {
      return this.$store.getters.hasPermission('marcas', 'editar');
    },
    canDelete(): boolean {
      return this.$store.getters.hasPermission('marcas', 'eliminar');
    }
  },
  methods: {
    // Abre el modal de confirmación para acciones sobre marcas, @param payload - Objeto con la marca y el tipo de acción a realizar
    openModal(payload: { brand: Brand, action: number }) {
      this.selectedBrand = payload.brand;
      this.action = payload.action;
      this.modal = true;
    },
    // Abre el formulario para crear o editar una marca, @param brand - Marca a editar (opcional). Si no se proporciona, crea una nueva
    openForm(brand?: Brand) {
      this.selectedBrand = brand ? { ...brand } : {
        idBrand: null,
        brandName: '',
        auditCreateDate: '',
        statusBrand: ''
      };
      this.form = true;
    },
    // Obtiene la lista de marcas desde el servidor, @param params - Parámetros opcionales de filtrado y paginación
    async fetchBrands(params?: any) {
      try {
        await this.store.dispatch('brand/fetchBrands', params || {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          stateFilter: this.stateFilter
        });
      } catch (error) {
        // Manejo silencioso del error (no muestra notificación al usuario)
        handleSilentError(error);
      }
    },
    // Construye los parámetros de filtrado para las peticiones al servidor
    getFilterParams(params: any) {
      // Mapeo de filtros textuales a valores numéricos
      const filterMap: { [key: string]: number } = { "Marca": 1 };
      const numberFilterValue = filterMap[params.selectedFilter || this.selectedFilter];
      const textFilterValue = params.search?.trim() || null;
      // Formateo de fechas si existen
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
    // Realiza una búsqueda de marcas con los parámetros especificados
    async searchBrands(params: any) {
      // Actualiza los valores locales de búsqueda y filtros
      this.search = params.search;
      this.selectedFilter = params.selectedFilter;
      this.state = params.state;
      this.startDate = params.startDate;
      this.endDate = params.endDate;

      try {
        await this.store.dispatch("brand/fetchBrands", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          ...this.getFilterParams(params)
        });
        // Resetea a la primera página después de una búsqueda
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar marcas');
      }
    },
    // Refresca la lista de marcas manteniendo los filtros actuales
    refreshBrands() {
      // // Si hay una búsqueda activa, la ejecuta; si no, obtiene todas las marcas
      if (this.search?.trim()) {
        this.searchBrands({
          search: this.search,
          selectedFilter: this.selectedFilter,
          state: this.state,
          startDate: this.startDate,
          endDate: this.endDate
        });
      } else {
        this.fetchBrands();
      }
    },
    // Actualiza el número de items por página y refresca la lista
    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1; // Resetea a la primera página
      this.refreshBrands();
    },
    // Cambia la página actual y refresca la lista
    changePage(page: number) {
      this.currentPage = page;
      this.refreshBrands();
    },
    // Descarga un archivo Excel con los datos de las marcas filtradas
    async downloadExcel(params: any) {
      this.downloadingExcel = true;
      try {
        await this.store.dispatch("brand/downloadBrandsExcel", {
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
    // Manejador del evento después de guardar una marca
    handleSaved() {
      this.fetchBrands();
    },
    // Manejador del evento después de completar una acción en el modal
    handleActionCompleted() {
      this.fetchBrands();
    }
  },
  // Hook que se ejecuta cuando el componente es montado en el DOM
  mounted() {
    // Carga inicial de las marcas
    this.fetchBrands();
  }
});
</script>