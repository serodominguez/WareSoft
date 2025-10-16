<template>
  <div>
    <v-toolbar>
      <v-toolbar-title>Gestión de Marcas</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn icon="tune" @click="drawer = !drawer"></v-btn>
      <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
        <v-text-field append-inner-icon="search" density="compact" label="Búsqueda" variant="solo" hide-details
          single-line v-model="search" @click:append-inner="searchBrands()"
          @keyup.enter="searchBrands()"></v-text-field>
      </v-col>
      <v-card-actions>
        <v-btn @click="openForm" color="indigo" size="large"> Nuevo </v-btn>
      </v-card-actions>
    </v-toolbar>
    <div style="display: flex; gap: 16px; margin-top: 16px;">
      <v-data-table-server :headers="headers" :items="brands" :search="search || undefined"
        :items-per-page-text="pages" :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage"
        :items-length="totalBrands" :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="updateItemsPerPage" @update:page="changePage">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as Brand).branD_NAME }}</td>
            <td>{{ (item as Brand).audiT_CREATE_DATE }}</td>
            <td>{{ (item as Brand).statE_BRAND }}</td>
            <td>
              <v-btn v-if="(item as Brand).statE_BRAND == 'ACTIVO'" color="indigo" icon="edit" variant="text"
                @click="editBrand(item)" size="small"></v-btn>
              <template v-if="(item as Brand).statE_BRAND == 'INACTIVO'">
                <v-btn color="indigo" icon="check" variant="text" @click="openModal(item, 1)" size="small"></v-btn>
              </template>
              <template v-if="(item as Brand).statE_BRAND == 'ACTIVO'">
                <v-btn color="indigo" icon="block" variant="text" @click="openModal(item, 2)" size="small"></v-btn>
              </template>
              <v-btn color="indigo" icon="delete" variant="text" @click="openModal(item, 0)" size="small"></v-btn>
            </td>
          </tr>
        </template>
        <template v-slot:no-data>
          <v-btn color="primary" @click="initialize"> Reset </v-btn>
        </template>
      </v-data-table-server>
    </div>
    <v-navigation-drawer v-model="drawer" temporary>
      <v-list>
        <v-list-item>
          <v-list-item-title class="text-h6">Filtros</v-list-item-title>
        </v-list-item>
        <v-list-item>
          <v-select v-model="selectedFilter" :items="filters" variant="outlined" density="compact"
            hide-details></v-select>
        </v-list-item>
        <v-list-item>
          <v-switch v-model="state" :label="`Estado: ${state}`" false-value="Inactivos" true-value="Activos"
            color="indigo" hide-details></v-switch>
        </v-list-item>
        <v-list-item>
          <v-date-input v-model="startDate" label="Desde:" prepend-icon="" variant="underlined" persistent-placeholder></v-date-input>
        </v-list-item>
        <v-list-item>
          <v-date-input v-model="endDate" label="Hasta:" prepend-icon="" variant="underlined" persistent-placeholder></v-date-input>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
  </div>
  <BrandForm v-model="form" :brand="selectedBrand" @saved="fetchBrands" />
  <BrandModal v-model="modal" :brand="selectedBrand" :action="action" @update:modelValue="modal = $event" />
</template>
<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { Brand } from '@/models/brandModel';
import BrandForm from './BrandForm.vue';
import BrandModal from './BrandModal.vue';

export default defineComponent({
  components: {
    BrandForm,
    BrandModal
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
      filters: ['Marca'],
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null
    };
  },
  computed: {
    headers() {
      return [
        { title: 'Marca', key: 'branD_NAME', sortable: false },
        { title: 'Fecha registro', key: 'audiT_CREATE_DATE', sortable: false },
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
      if (this.selectedFilter === "Marca") {
        numberFilterValue = 1;
      }

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
