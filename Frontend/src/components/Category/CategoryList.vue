<template>
  <v-data-table-server :headers="headers" :items="categories" :search="search || undefined" :items-per-page-text="pages"
    :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage" :items-length="totalCategories"
    :loading="loading" loading-text="Cargando... Espere por favor" @update:items-per-page="updateItemsPerPage"
    @update:page="changePage">
    <template v-slot:item="{ item }">
      <tr>
        <td>{{ (item as Category).categorY_NAME }}</td>
        <td>{{ (item as Category).description }}</td>
        <td>{{ (item as Category).audiT_CREATE_DATE }}</td>
        <td>{{ (item as Category).statE_CATEGORY }}</td>
        <td>
          <v-btn v-if="(item as Category).statE_CATEGORY == 'ACTIVO'" color="blue" icon="edit" variant="text"
            @click="editCategory(item)" size="small"></v-btn>
          <template v-if="(item as Category).statE_CATEGORY == 'INACTIVO'">
            <v-btn color="red" icon="block" variant="text" @click="openModal(item, 2)" size="small"></v-btn>
          </template>
          <template v-if="(item as Category).statE_CATEGORY == 'ACTIVO'">
            <v-btn color="red" icon="block" variant="text" @click="openModal(item, 2)" size="small"></v-btn>
          </template>
        </td>
      </tr>
    </template>
    <template v-slot:top>
      <v-toolbar>
        <v-toolbar-title>Gestión de Categorías</v-toolbar-title>
        <v-spacer></v-spacer>
        <v-col cols="4" md="1" lg="1" xl="1" class="pa-1">
        <v-select v-model="selectedFilter" :items="filters" variant="solo" density="compact" single-line hide-details></v-select>
        </v-col>
        <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
        <v-text-field append-inner-icon="search" density="compact" label="Búsqueda" variant="solo" hide-details
          single-line v-model="search" @keyup.enter="searchCategories()"></v-text-field>
        </v-col>
        <v-card-actions>
          <v-btn @click="openForm" color="primary" size="large"> Nuevo </v-btn>
        </v-card-actions>
      </v-toolbar>
    </template>
    <template v-slot:no-data>
      <v-btn color="primary" @click="initialize"> Reset </v-btn>
    </template>
  </v-data-table-server>

</template>
<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { Category } from '@/models/categoryModel';

export default defineComponent({
  data() {
    return {
      items: [10, 20, 50],
      currentPage: 1,
      itemsPerPage: 10,
      pages: "Categorías por Página",
      search: null as string | null,
      modal: false,
      statusModal: false,
      selectedCategory: null as Category | null,
      action: 0,
      selectedFilter: 'Nombre',
      filters: ['Nombre', 'Descripción']
    };
  },
  computed: {
    headers() {
      return [
        { title: 'Categoría', key: 'categorY_NAME' },
        { title: 'Descripción', key: 'description' },
        { title: 'Fecha registro', key: 'audiT_CREATE_DATE' },
        { title: 'Estado', key: 'statE_CATEGORY' },
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
    }
  },
  methods: {
    initialize() {

    },
    openModal(category: any, action: number) {
      this.selectedCategory = category;
      this.action = action;
      this.statusModal = true;
    },
    openForm() {
      this.selectedCategory = {
        pK_CATEGORY: null,
        categorY_NAME: '',
        description: '',
        audiT_CREATE_DATE: '',
        statE_CATEGORY: ''
      };
      this.modal = true;
    },
    async fetchCategories() {
      await this.store.dispatch('category/fetchCategories', {
        pageNumber: this.currentPage,
        pageSize: this.itemsPerPage
      });
    },
    async searchCategories() {
      let numberFilterValue: number | null = null;
      if (this.selectedFilter === "Nombre") {
        numberFilterValue = 1;
      } else if (this.selectedFilter === "Descripción") {
        numberFilterValue = 2;
      }

      const textFilterValue = this.search && this.search.trim() !== "" ? this.search.trim() : null;
      await this.store.dispatch("category/fetchCategories", {
        pageNumber: 1,
        pageSize: this.itemsPerPage,
        textFilter: textFilterValue,
        numberFilter: numberFilterValue,
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
      this.modal = true;
    },
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
