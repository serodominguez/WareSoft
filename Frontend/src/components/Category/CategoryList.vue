<template>
  <v-data-table
    :headers="headers"
    :items="categories"
    :search="search || undefined"
    :items-per-page-text="pages"
    :items-per-page-options="[5, 10, 20]"
    :items-per-page="5"
    :loading="loading"
    loading-text="Cargando... Espere por favor"
  >
    <template v-slot:item="{ item }">
      <tr>
        <td>{{ (item as Category).categorY_NAME }}</td>
        <td>{{ (item as Category).state }}</td>
        <td>
          <v-icon class="me-2" size="small" @click="editCategory(item as Category)">mdi-pencil</v-icon>
          <template v-if="(item as Category).state == 'INACTIVO'">
            <v-icon size="small" @click="openStatusModal(item as Category, 1)">mdi-check</v-icon>
          </template>
          <template v-if="(item as Category).state == 'ACTIVO'">  
            <v-icon size="small" @click="openStatusModal(item as Category, 2)">mdi-cancel</v-icon>
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
// import Modal from '../Commons/Modal.vue';
// import CategoryForm from './CategoryForm.vue';

interface Category {
  pK_CATEGORY: number | null;
  categorY_NAME: string;
  state: string | null;
}

interface Header {
  title: string;
  key: string;
  sortable?: boolean;
}

interface ComponentData {
  items: number[];
  pages: string;
  search: string | null;
  modal: boolean;
  statusModal: boolean;
  selectedCategory: Category | null;
  action: number;
}

export default defineComponent({
  name: 'CategoryComponent',
  
  // components: {
  //   CategoryForm,
  //   Modal,
  // },
  
  data(): ComponentData {
    return {
      items: [5, 10, 25],
      pages: "Categorías por Página",
      search: null,
      modal: false,
      statusModal: false,
      selectedCategory: null,
      action: 0, // 1 for activate, 2 for deactivate
    };
  },
  
  computed: {
    ...mapGetters('category', ['categories', 'loading']),
    
    headers(): Header[] {
      return [
        { title: 'Categoría', key: 'categorY_NAME' },
        { title: 'Estado', key: 'state' },
        { title: 'Acciones', key: 'actions', sortable: false },
      ];
    },
  },
  
  methods: {
    initialize(): void {
      // Reiniciar búsqueda o recargar datos
      this.search = null;
      this.fetchCategories();
    },
    
    ...mapActions('category', ['fetchCategories']),
    
    openModal(): void {
      this.selectedCategory = { pK_CATEGORY: null, categorY_NAME: '', state: null };
      this.modal = true;
    },
    
    editCategory(category: Category): void {
      this.selectedCategory = { ...category };
      this.modal = true;
    },
    
    openStatusModal(category: Category, action: number): void {
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