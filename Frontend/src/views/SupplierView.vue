<template>
  <div>
    <SupplierList :suppliers="suppliers" :loading="loading" :totalSuppliers="totalSuppliers"
      :downloadingExcel="downloadingExcel" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-supplier="openForm"
      @fetch-supplier="fetchSuppliers" @search-suppliers="searchSuppliers" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" />

    <SupplierForm v-model="form" :supplier="selectedSupplier" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedSupplier?.idSupplier || 0" :item="selectedSupplier?.companyName || ''"
      :action="action" moduleName="supplier" entityName="Supplier" name="Proveedor" gender="male"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Supplier } from '@/interfaces/supplierInterface';
import { formatDate } from '@/utils/date';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import SupplierList from '@/components/Supplier/SupplierList.vue';
import SupplierForm from '@/components/Supplier/SupplierForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

export default defineComponent({
  name: 'SupplierView',
  components: {
    SupplierList,
    SupplierForm,
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
      form: false,
      modal: false,
      selectedSupplier: null as Supplier | null,
      action: 0,
      selectedFilter: 'Empresa',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null,
      downloadingExcel: false
    };
  },
  computed: {
    suppliers() {
      return this.store.getters['supplier/suppliers'];
    },
    loading() {
      return this.store.getters['supplier/loading'];
    },
    totalSuppliers() {
      return this.store.getters['supplier/totalSuppliers'];
    },
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    },
    canCreate(): boolean {
      return this.$store.getters.hasPermission('proveedores', 'crear');
    },
    canRead(): boolean {
      return this.$store.getters.hasPermission('proveedores', 'leer');
    },
    canEdit(): boolean {
      return this.$store.getters.hasPermission('proveedores', 'editar');
    },
    canDelete(): boolean {
      return this.$store.getters.hasPermission('proveedores', 'eliminar');
    }
  },
  methods: {
    openModal(payload: { supplier: Supplier, action: number }) {
      this.selectedSupplier = payload.supplier;
      this.action = payload.action;
      this.modal = true;
    },

    openForm(supplier?: Supplier) {
      this.selectedSupplier = supplier ? { ...supplier } : {
        idSupplier: null,
        companyName: '',
        contact: '',
        email: '',
        phoneNumber: null,
        auditCreateDate: '',
        statusSupplier: ''
      };
      this.form = true;
    },

    async fetchSuppliers(params?: any) {
      try {
        await this.store.dispatch('supplier/fetchSuppliers', params || {
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
        "Empresa": 1,
        "Contacto": 2
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

    async searchSuppliers(params: any) {
      this.search = params.search;
      this.selectedFilter = params.selectedFilter;
      this.state = params.state;
      this.startDate = params.startDate;
      this.endDate = params.endDate;

      try {
        await this.store.dispatch("supplier/fetchSuppliers", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          ...this.getFilterParams(params)
        });
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar proveedores');
      }
    },

    refreshSuppliers() {
      if (this.search?.trim()) {
        this.searchSuppliers({
          search: this.search,
          selectedFilter: this.selectedFilter,
          state: this.state,
          startDate: this.startDate,
          endDate: this.endDate
        });
      } else {
        this.fetchSuppliers();
      }
    },

    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;
      this.refreshSuppliers();
    },

    changePage(page: number) {
      this.currentPage = page;
      this.refreshSuppliers();
    },

    async downloadExcel(params: any) {
      this.downloadingExcel = true;
      try {
        await this.store.dispatch("supplier/downloadSuppliersExcel", {
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

    handleSaved() {
      this.fetchSuppliers();
    },

    handleActionCompleted() {
      this.fetchSuppliers();
    }
  },
  mounted() {
    this.fetchSuppliers();
  }
});
</script>