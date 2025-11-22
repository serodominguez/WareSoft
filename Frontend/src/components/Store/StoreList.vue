<template>
  <v-card elevation="2">
    <v-data-table-server :headers="headers" :items="stores" :search="search || undefined" :items-per-page-text="pages"
      :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage" :items-length="totalStores"
      :loading="loading" loading-text="Cargando... Espere por favor" @update:items-per-page="updateItemsPerPage"
      @update:page="changePage">
      <template v-slot:item="{ item }">
        <tr>
          <td>{{ (item as Store).storeName }}</td>
          <td>{{ (item as Store).manager }}</td>
          <td>{{ (item as Store).address }}</td>
          <td>{{ (item as Store).city }}</td>
          <td>{{ (item as Store).auditCreateDate }}</td>
          <td>{{ (item as Store).statusStore }}</td>
          <td>
            <v-btn v-if="canEdit && (item as Store).statusStore == 'Activo'" color="indigo" icon="edit" variant="text"
              @click="editStore(item)" size="small"></v-btn>
            <template v-if="canEdit && (item as Store).statusStore == 'Inactivo'">
              <v-btn color="indigo" icon="check" variant="text" @click="openModal(item, 1)" size="small"></v-btn>
            </template>
            <template v-if="canEdit &&  (item as Store).statusStore == 'Activo'">
              <v-btn color="indigo" icon="block" variant="text" @click="openModal(item, 2)" size="small"></v-btn>
            </template>
            <v-btn v-if="canDelete" color="indigo" icon="delete" variant="text" @click="openModal(item, 0)" size="small"></v-btn>
          </td>
        </tr>
      </template>
      <template v-slot:top>
        <v-toolbar>
          <v-toolbar-title>Gestión de Tiendas</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-btn v-if="canRead" icon="download" @click="downloadExcel" :loading="downloadingExcel"></v-btn>
          <v-btn icon="tune" @click="drawer = !drawer"></v-btn>
          <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
            <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo" hide-details
              single-line v-model="search" @click:append-inner="searchStores()"
              @keyup.enter="searchStores()"></v-text-field>
          </v-col>
          <v-card-actions>
            <v-btn v-if="canCreate" @click="openForm" color="indigo" size="large"> Nuevo </v-btn>
          </v-card-actions>
        </v-toolbar>
      </template>
      <template v-slot:no-data>
        <v-btn color="primary" @click="initialize"> Reset </v-btn>
      </template>
    </v-data-table-server>
  </v-card>
  <StoreFilters v-model="drawer" v-model:selected-filter="selectedFilter" v-model:state="state" v-model:start-date="startDate" v-model:end-date="endDate" @apply-filters="searchStores" />
  <StoreForm v-model="form" :store="selectedStore" @saved="fetchStores" />
  <StoreModal v-model="modal" :store="selectedStore" :action="action" @update:modelValue="modal = $event" />
</template>
<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Store } from '@/interfaces/storeInterface';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import StoreForm from './StoreForm.vue';
import StoreModal from './StoreModal.vue';
import StoreFilters from './StoreFilters.vue';

export default defineComponent({
  components: {
    StoreForm,
    StoreModal,
    StoreFilters
  },
  data() {
    return {
      items: [10, 20, 50],
      currentPage: 1,
      itemsPerPage: 10,
      pages: "Tiendas por Página",
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
      downloadingExcel: false,
      toast: useToast()
    };
  },
  computed: {
    headers() {
      return [
        { title: 'Tienda', key: 'storeName', sortable: false },
        { title: 'Encargado', key: 'manager', sortable: false },
        { title: 'Dirección', key: 'address', sortable: false },
        { title: 'Ciudad', key: 'city', sortable: false },
        { title: 'Fecha de registro', key: 'auditCreateDate', sortable: false },
        { title: 'Estado', key: 'statusStore', sortable: false },
        { title: 'Acciones', key: 'actions', sortable: false },
      ];
    },
    totalPages() {
      return Math.ceil(this.store.getters['store/totalStores'] / this.itemsPerPage);
    },
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
    initialize() {
      this.fetchStores();
    },
    openModal(store: any, action: number) {
      this.selectedStore = store;
      this.action = action;
      this.modal = true;
    },
    openForm() {
      this.selectedStore = {
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
    async fetchStores() {
      try {
        await this.store.dispatch('store/fetchStores', {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          stateFilter: this.stateFilter
        });
      } catch (error) {
        handleSilentError(error);
      }
    },
    async searchStores() {
      let numberFilterValue: number | null = null;
      const filterMap: { [key: string]: number } = {
        "Tienda": 1,
        "Encargado": 2,
        "Dirección": 3,
        "Ciudad": 4,
      };

      numberFilterValue = filterMap[this.selectedFilter];
      const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
      const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
      const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

      try {
        await this.store.dispatch("store/fetchStores", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          textFilter: textFilterValue,
          numberFilter: numberFilterValue,
          stateFilter: this.stateFilter,
          startDate: startDateStr,
          endDate: endDateStr
        });
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar tiendas');
      }
    },
    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;

      if (this.search && this.search.trim() !== "") {
        this.searchStores();
      } else {
        this.fetchStores();
      }
    },
    changePage(page: number) {
      this.currentPage = page;

      if (this.search && this.search.trim() !== "") {
        this.searchStores();
      } else {
        this.fetchStores();
      }
    },
    editStore(store: any) {
      this.selectedStore = { ...store };
      this.form = true;
    },
    async downloadExcel() {
      this.downloadingExcel = true;
      try {
      let numberFilterValue: number | null = null;
      const filterMap: { [key: string]: number } = {
        "Tienda": 1,
        "Encargado": 2,
        "Dirección": 3,
        "Ciudad": 4,
      };

        numberFilterValue = filterMap[this.selectedFilter];
        const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
        const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
        const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

        await this.store.dispatch("store/downloadStoresExcel", {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          textFilter: textFilterValue,
          numberFilter: numberFilterValue,
          stateFilter: this.stateFilter,
          startDate: startDateStr,
          endDate: endDateStr
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
    }
  },
  mounted() {
    this.fetchStores();
  },
  setup() {
    const store = useStore();
    return { store };
  }
});
</script>
