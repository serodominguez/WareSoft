<template>
  <v-data-table
    :headers="headers"
    :items="categories"
    :search="search || undefined"
    :items-per-page="itemsPerPage"
    :page.sync="currentPage"
    :items-per-page-options="[5, 10, 20, 30]"
    :total-items="totalRecords"
    :loading="loading"
    show-current-page
    :items-per-page-text="pages"
    loading-text="Cargando... Espere por favor"
    @update:options="handlePaginationChange"
    server
  >
    <template v-slot:item="{ item }">
      <tr>
        <td>{{ (item as Category).categorY_NAME }}</td>
        <td>{{ (item as Category).statE_CATEGORY || ((item as Category).state ? 'ACTIVO' : 'INACTIVO') }}</td>
        <td>
          <v-btn v-if="(item as Category).statE_CATEGORY =='ACTIVO'" color="blue" icon="edit" variant="text" @click="editCategory(item as Category)" size="small"></v-btn>
          <template v-if="(item as Category).statE_CATEGORY == 'INACTIVO' || !(item as Category).state">
            <v-btn icon="check" variant="text" @click="openStatusModal(item as Category, 1)"> size="small"></v-btn>
          </template>
            <template v-if="(item as Category).statE_CATEGORY == 'ACTIVO' || (item as Category).state"> 
              <v-btn color="red" icon="block" variant="text" @click="openStatusModal(item as Category, 2)"size="small"></v-btn>
            </template>
        </td>
      </tr>
    </template>
    <template v-slot:top>
      <v-toolbar>
        <v-toolbar-title>Gestión de Categorías</v-toolbar-title>
        <v-spacer></v-spacer>
        <v-text-field
          append-inner-icon="search"
          density="compact"
          label="Búsqueda"
          variant="solo"
          hide-details
          single-line
          v-model="search"
          @update:model-value="handleSearch"
        ></v-text-field>
        <v-card-actions>
          <v-btn @click="openModal" color="primary" size="large"> Agregar </v-btn>
        </v-card-actions>
      </v-toolbar>
    </template>
    <template v-slot:no-data>
      <v-btn color="primary" @click="initialize"> Reset </v-btn>
    </template>
  </v-data-table>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { mapGetters, mapActions } from 'vuex';

interface Category {
  pK_CATEGORY: number | null;
  categorY_NAME: string;
  description?: string;
  audiT_CREATE_DATE?: string;
  state: boolean;
  statE_CATEGORY?: string;
}

interface Header {
  title: string;
  key: string;
  sortable?: boolean;
}

interface ComponentData {
  pages: string;
  search: string | null;
  modal: boolean;
  statusModal: boolean;
  selectedCategory: Category | null;
  action: number;
}

export default defineComponent({
  name: 'CategoryComponent',
  
  data(): ComponentData {
    return {
      pages: "Categorías por Página",
      search: null,
      modal: false,
      statusModal: false,
      selectedCategory: null,
      action: 0,
    };
  },
  
  computed: {
    ...mapGetters({
      categories: 'category/categories',
      loading: 'category/loading',
      totalRecords: 'category/totalRecords',
      currentPage: 'category/currentPage',
      itemsPerPage: 'category/itemsPerPage',
    }),
    
    headers(): Header[] {
      return [
        { title: 'Categoría', key: 'categorY_NAME', sortable: true },
        { title: 'Estado', key: 'statE_CATEGORY', sortable: true },
        { title: 'Acciones', key: 'actions', sortable: false },
      ];
    },
  },
  
  methods: {
    async handlePaginationChange(options: any) {
      const { page, itemsPerPage, sortBy, sortDesc } = options;
      const sort = sortBy?.[0]?.key || 'PK_CATEGORY';
      const order = sortDesc?.[0] ? 'desc' : 'asc';
      await this.updatePagination({ page, itemsPerPage, sort, order });
    },

    async handleSearch(searchValue: string | null) {
      this.search = searchValue;
      await this.setCurrentPage(1);
      await this.fetchCategories({ page: 1 });
    },

    async initialize() {
      this.search = null;
      await this.resetPagination();
    },
    
    ...mapActions({
      fetchCategories: 'category/fetchCategories',
      updatePagination: 'category/updatePagination',
      setCurrentPage: 'category/setCurrentPage',
      resetPagination: 'category/resetPagination',
      createCategory: 'category/createCategory',
      updateCategory: 'category/updateCategory',
      enabledCategorie: 'category/enabledCategorie',
      disabledCategorie: 'category/disabledCategorie',
    }),
    
    openModal(): void {
      this.selectedCategory = { 
        pK_CATEGORY: null, 
        categorY_NAME: '', 
        description: '', 
        audiT_CREATE_DATE: '', 
        state: true, 
        statE_CATEGORY: 'ACTIVO' 
      };
      this.modal = true;
    },
    
    editCategory(category: Category): void {
      this.selectedCategory = { ...category };
      this.modal = true;
    },
    
    openStatusModal(category: Category, action: number): void {
      if (category.pK_CATEGORY === null) {
        console.error('No se puede cambiar el estado de una categoría sin ID');
        return;
      }
      
      this.selectedCategory = category;
      this.action = action;
      this.statusModal = true;
    },
  },
  
  mounted(): void {
    this.fetchCategories();
  },
});
</script>