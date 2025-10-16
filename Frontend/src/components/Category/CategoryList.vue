<template>
  <v-card elevation="2">
    <v-data-table-server :headers="headers" :items="categories" :search="search || undefined"
      :items-per-page-text="pages" :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage"
      :items-length="totalCategories" :loading="loading" loading-text="Cargando... Espere por favor"
      @update:items-per-page="updateItemsPerPage" @update:page="changePage">
      <template v-slot:item="{ item }">
        <tr>
          <td>{{ (item as Category).categorY_NAME }}</td>
          <td>{{ (item as Category).description }}</td>
          <td>{{ (item as Category).audiT_CREATE_DATE }}</td>
          <td>{{ (item as Category).statE_CATEGORY }}</td>
          <td>
            <v-btn v-if="(item as Category).statE_CATEGORY == 'ACTIVO'" color="indigo" icon="edit" variant="text"
              @click="editCategory(item)" size="small"></v-btn>
            <template v-if="(item as Category).statE_CATEGORY == 'INACTIVO'">
              <v-btn color="indigo" icon="check" variant="text" @click="openModal(item, 1)" size="small"></v-btn>
            </template>
            <template v-if="(item as Category).statE_CATEGORY == 'ACTIVO'">
              <v-btn color="indigo" icon="block" variant="text" @click="openModal(item, 2)" size="small"></v-btn>
            </template>
            <v-btn color="indigo" icon="delete" variant="text" @click="openModal(item, 0)" size="small"></v-btn>
          </td>
        </tr>
      </template>
      <template v-slot:top>
        <v-toolbar>
          <v-toolbar-title>Gestión de Categorías</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-btn icon="tune" @click="drawer = true"></v-btn>
          <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
            <v-text-field append-inner-icon="search" density="compact" label="Búsqueda" variant="solo" hide-details
              single-line v-model="search" @click:append-inner="searchCategories()"
              @keyup.enter="searchCategories()"></v-text-field>
          </v-col>
          <v-card-actions>
            <v-btn @click="openForm" color="indigo" size="large"> Nuevo </v-btn>
          </v-card-actions>
        </v-toolbar>
      </template>
      <template v-slot:no-data>
        <v-btn color="primary" @click="initialize"> Reset </v-btn>
      </template>
    </v-data-table-server>
  </v-card>
  <CategoryFilters v-model="drawer" v-model:selected-filter="selectedFilter" v-model:state="state" v-model:start-date="startDate" v-model:end-date="endDate" />
  <CategoryForm v-model="form" :category="selectedCategory" @saved="fetchCategories" />
  <CategoryModal v-model="modal" :category="selectedCategory" :action="action" @update:modelValue="modal = $event" />
</template>
<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { Category } from '@/models/categoryModel';
import CategoryForm from './CategoryForm.vue';
import CategoryModal from './CategoryModal.vue';
import CategoryFilters from './CategoryFilters.vue';

export default defineComponent({
  components: {
    CategoryForm,
    CategoryModal,
    CategoryFilters
  },
  data() {
    return {
      items: [10, 20, 50],
      currentPage: 1,
      itemsPerPage: 10,
      pages: "Categorías por Página",
      search: null as string | null,
      form: false,
      modal: false,
      selectedCategory: null as Category | null,
      action: 0,
      selectedFilter: 'Categoría',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null
    };
  },
  computed: {
    headers() {
      return [
        { title: 'Categoría', key: 'categorY_NAME', sortable: false },
        { title: 'Descripción', key: 'description', sortable: false },
        { title: 'Fecha registro', key: 'audiT_CREATE_DATE', sortable: false },
        { title: 'Estado', key: 'statE_CATEGORY', sortable: false },
        { title: 'Acciones', key: 'actions', sortable: false },
      ];
    },
    totalPages() {
      return Math.ceil(this.store.getters['category/totalCategories'] / this.itemsPerPage);
    },
    categories() {
      return this.store.getters['category/categories'];
    },
    loading() {
      return this.store.getters['category/loading'];
    },
    totalCategories() {
      return this.store.getters['category/totalCategories'];
    },
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    }
  },
  methods: {
    initialize() {
      this.fetchCategories();
    },
    openModal(category: any, action: number) {
      this.selectedCategory = category;
      this.action = action;
      this.modal = true;
    },
    openForm() {
      this.selectedCategory = {
        pK_CATEGORY: null,
        categorY_NAME: '',
        description: '',
        audiT_CREATE_DATE: '',
        statE_CATEGORY: ''
      };
      this.form = true;
    },
    async fetchCategories() {
      await this.store.dispatch('category/fetchCategories', {
        pageNumber: this.currentPage,
        pageSize: this.itemsPerPage,
        stateFilter: this.stateFilter
      });
    },
    async searchCategories() {
      let numberFilterValue: number | null = null;
      if (this.selectedFilter === "Categoría") {
        numberFilterValue = 1;
      } else if (this.selectedFilter === "Descripción") {
        numberFilterValue = 2;
      }

      const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
      const startDateStr = this.startDate ? this.formatDate(this.startDate) : null;
      const endDateStr = this.endDate ? this.formatDate(this.endDate) : null;

      await this.store.dispatch("category/fetchCategories", {
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
        this.searchCategories();
      } else {
        this.fetchCategories();
      }
    },
    changePage(page: number) {
      this.currentPage = page;

      if (this.search && this.search.trim() !== "") {
        this.searchCategories();
      } else {
        this.fetchCategories();
      }
    },
    editCategory(category: any) {
      this.selectedCategory = { ...category };
      this.form = true;
    },
    formatDate(date: Date | null): string | null {
      if (!date) return null;

      // Formato ISO: YYYY-MM-DD
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const day = String(date.getDate()).padStart(2, '0');

      return `${year}-${month}-${day}`;
    }
  },
  mounted() {
    this.fetchCategories();
  },
  setup() {
    const store = useStore();
    return { store };
  }
});
</script>