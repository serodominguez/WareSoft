<template>
  <v-card elevation="2">
    <v-data-table-server :headers="headers" :items="brands" :search="search || undefined" :items-per-page-text="pages"
      :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage" :items-length="totalBrands"
      :loading="loading" loading-text="Cargando... Espere por favor" @update:items-per-page="updateItemsPerPage"
      @update:page="changePage">
      <template v-slot:item="{ item }">
        <tr>
          <td>{{ (item as Brand).branD_NAME }}</td>
          <td>{{ (item as Brand).audiT_CREATE_DATE }}</td>
          <td>{{ (item as Brand).statE_BRAND }}</td>
          <td>
            <v-btn v-if="canEdit && (item as Brand).statE_BRAND == 'ACTIVO'" color="indigo" icon="edit" variant="text"
              @click="editBrand(item)" size="small"></v-btn>
            <template v-if="canEdit && (item as Brand).statE_BRAND == 'INACTIVO'">
              <v-btn  color="indigo" icon="check" variant="text" @click="openModal(item, 1)" size="small"></v-btn>
            </template>
            <template v-if="canEdit && (item as Brand).statE_BRAND == 'ACTIVO'">
              <v-btn color="indigo" icon="block" variant="text" @click="openModal(item, 2)" size="small"></v-btn>
            </template>
            <v-btn v-if="canDelete" color="indigo" icon="delete" variant="text" @click="openModal(item, 0)" size="small"></v-btn>
          </td>
        </tr>
      </template>
      <template v-slot:top>
        <v-toolbar>
          <v-toolbar-title>Gestión de Marcas</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-btn v-if="canRead" icon="download" @click="downloadExcel" :loading="downloadingExcel"></v-btn>
          <v-btn icon="tune" @click="drawer = !drawer"></v-btn>
          <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
            <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo" hide-details
              single-line v-model="search" @click:append-inner="searchBrands()"
              @keyup.enter="searchBrands()"></v-text-field>
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
  <BrandFilters v-model="drawer" v-model:selected-filter="selectedFilter" v-model:state="state" v-model:start-date="startDate" v-model:end-date="endDate" />
  <BrandForm v-model="form" :brand="selectedBrand" @saved="fetchBrands" />
  <BrandModal v-model="modal" :brand="selectedBrand" :action="action" @update:modelValue="modal = $event" />
</template>
<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { Brand } from '@/interfaces/brandInterface';
import BrandForm from './BrandForm.vue';
import BrandModal from './BrandModal.vue';
import BrandFilters from './BrandFilters.vue';

export default defineComponent({
  components: {
    BrandForm,
    BrandModal,
    BrandFilters
  },
  data() {
    return {
      items: [10, 20, 50],
      currentPage: 1,
      itemsPerPage: 10,
      pages: "Marcas por Página",
      search: null as string | null,
      form: false,
      modal: false,
      selectedBrand: null as Brand | null,
      action: 0,
      selectedFilter: 'Marca',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null,
      downloadingExcel: false,
    };
  },
  computed: {
    headers() {
      return [
        { title: 'Marca', key: 'branD_NAME', sortable: false },
        { title: 'Fecha de registro', key: 'audiT_CREATE_DATE', sortable: false },
        { title: 'Estado', key: 'statE_BRAND', sortable: false },
        { title: 'Acciones', key: 'actions', sortable: false },
      ];
    },
    totalPages() {
      return Math.ceil(this.store.getters['brand/totalBrands'] / this.itemsPerPage);
    },
    brands() {
      return this.store.getters['brand/brands'];
    },
    loading() {
      return this.store.getters['brand/loading'];
    },
    totalBrands() {
      return this.store.getters['brand/totalBrands'];
    },
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    },
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
    initialize() {
      this.fetchBrands();
    },
    openModal(brand: any, action: number) {
      this.selectedBrand = brand;
      this.action = action;
      this.modal = true;
    },
    openForm() {
      this.selectedBrand = {
        pK_BRAND: null,
        branD_NAME: '',
        audiT_CREATE_DATE: '',
        statE_BRAND: ''
      };
      this.form = true;
    },
    async fetchBrands() {
      await this.store.dispatch('brand/fetchBrands', {
        pageNumber: this.currentPage,
        pageSize: this.itemsPerPage,
        stateFilter: this.stateFilter
      });
    },
    async searchBrands() {
      let numberFilterValue: number | null = null;
      const filterMap: { [key: string]: number } = {
        "Marca": 1,
      }
        
      numberFilterValue = filterMap[this.selectedFilter];
      const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
      const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
      const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

      await this.store.dispatch("brand/fetchBrands", {
        pageNumber: 1,
        pageSize: this.itemsPerPage,
        textFilter: textFilterValue,
        numberFilter: numberFilterValue,
        stateFilter: this.stateFilter,
        startDate: startDateStr,
        endDate: endDateStr
      });
      this.currentPage = 1;
    },
    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;

      if (this.search && this.search.trim() !== "") {
        this.searchBrands();
      } else {
        this.fetchBrands();
      }
    },
    changePage(page: number) {
      this.currentPage = page;

      if (this.search && this.search.trim() !== "") {
        this.searchBrands();
      } else {
        this.fetchBrands();
      }
    },
    editBrand(brand: any) {
      this.selectedBrand = { ...brand };
      this.form = true;
    },
    async downloadExcel() {
      this.downloadingExcel = true;
      try {
      let numberFilterValue: number | null = null;
      const filterMap: { [key: string]: number } = {
        "Marca": 1,
      }
      
        numberFilterValue = filterMap[this.selectedFilter];
        const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
        const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
        const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

        await this.store.dispatch("brand/downloadBrandsExcel", {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          textFilter: textFilterValue,
          numberFilter: numberFilterValue,
          stateFilter: this.stateFilter,
          startDate: startDateStr,
          endDate: endDateStr
        });
      } catch (error) {
        console.error('Error al descargar el archivo:', error);
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
    this.fetchBrands();
  },
  setup() {
    const store = useStore();
    return { store };
  }
});
</script>
