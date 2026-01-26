<template>
  <div>
    <CustomerList :customers="customers" :loading="loading" :totalCustomers="totalCustomers"
      :downloadingExcel="downloadingExcel" :canCreate="canCreate" :canRead="canRead" :canEdit="canEdit"
      :canDelete="canDelete" :items-per-page="itemsPerPage" v-model:drawer="drawer"
      v-model:selectedFilter="selectedFilter" v-model:state="state" v-model:startDate="startDate"
      v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal" @edit-customer="openForm"
      @fetch-customers="fetchCustomers" @search-customers="searchCustomers" @update-items-per-page="updateItemsPerPage"
      @change-page="changePage" @download-excel="downloadExcel" />

    <CustomerForm v-model="form" :customer="selectedCustomer" @saved="handleSaved" />

    <CommonModal v-model="modal" :itemId="selectedCustomer?.idCustomer || 0" :item="selectedCustomer?.names || ''"
      :action="action" moduleName="customer" entityName="Customer" name="Cliente" gender="male"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Customer } from '@/interfaces/customerInterface';
import { formatDate } from '@/utils/date';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import CustomerList from '@/components/Customer/CustomerList.vue';
import CustomerForm from '@/components/Customer/CustomerForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

export default defineComponent({
  name: 'CustomerView',
  components: {
    CustomerList,
    CustomerForm,
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
      selectedCustomer: null as Customer | null,
      action: 0,
      selectedFilter: 'Nombres',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null,
      downloadingExcel: false
    };
  },
  computed: {
    customers() {
      return this.store.getters['customer/customers'];
    },
    loading() {
      return this.store.getters['customer/loading'];
    },
    totalCustomers() {
      return this.store.getters['customer/totalCustomers'];
    },
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    },
    canCreate(): boolean {
      return this.$store.getters.hasPermission('clientes', 'crear');
    },
    canRead(): boolean {
      return this.$store.getters.hasPermission('clientes', 'leer');
    },
    canEdit(): boolean {
      return this.$store.getters.hasPermission('clientes', 'editar');
    },
    canDelete(): boolean {
      return this.$store.getters.hasPermission('clientes', 'eliminar');
    }
  },
  methods: {
    openModal(payload: { customer: Customer, action: number }) {
      this.selectedCustomer = payload.customer;
      this.action = payload.action;
      this.modal = true;
    },

    openForm(customer?: Customer) {
      this.selectedCustomer = customer ? { ...customer } : {
        idCustomer: null,
        names: '',
        lastNames: '',
        identificationNumber: '',
        phoneNumber: null,
        auditCreateDate: '',
        statusCustomer: ''
      };
      this.form = true;
    },

    async fetchCustomers(params?: any) {
      try {
        await this.store.dispatch('customer/fetchCustomers', params || {
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
        "Nombres": 1,
        "Apellidos": 2,
        "Carnet": 3
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

    async searchCustomers(params: any) {
      this.search = params.search;
      this.selectedFilter = params.selectedFilter;
      this.state = params.state;
      this.startDate = params.startDate;
      this.endDate = params.endDate;

      try {
        await this.store.dispatch("customer/fetchCustomers", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          ...this.getFilterParams(params)
        });
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar clientes');
      }
    },

    refreshCustomers() {
      if (this.search?.trim()) {
        this.searchCustomers({
          search: this.search,
          selectedFilter: this.selectedFilter,
          state: this.state,
          startDate: this.startDate,
          endDate: this.endDate
        });
      } else {
        this.fetchCustomers();
      }
    },

    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;
      this.refreshCustomers();
    },

    changePage(page: number) {
      this.currentPage = page;
      this.refreshCustomers();
    },

    async downloadExcel(params: any) {
      this.downloadingExcel = true;
      try {
        await this.store.dispatch("customer/downloadCustomersExcel", {
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
      this.fetchCustomers();
    },

    handleActionCompleted() {
      this.fetchCustomers();
    }
  },
  mounted() {
    this.fetchCustomers();
  }
});
</script>