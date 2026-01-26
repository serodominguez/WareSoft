<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="inventories" :search="search || undefined"
        :items-per-page-text="pages" :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage"
        :items-length="totalInventories" :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as Inventory).code }}</td>
            <td>{{ (item as Inventory).description }}</td>
            <td>{{ (item as Inventory).material }}</td>
            <td>{{ (item as Inventory).color }}</td>
            <td>{{ (item as Inventory).categoryName }}</td>
            <td>{{ (item as Inventory).brandName }}</td>
           <td class="text-center" :class="{ 'text-red': ((item as Inventory).stock ?? 0) <= 0 }">
              {{ (item as Inventory).stock }}
            </td>
            <td class="text-center" :class="{ 'text-red': ((item as Inventory).price ?? 0) <= 0 }">
              {{ (item as Inventory).price }}
            </td>
            <td class="text-center">
              <v-btn v-if="canEdit" color="indigo" icon="currency_exchange"
                variant="text" @click="$emit('edit-inventory', item)" size="small" title="Precio">
              </v-btn>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title>Gestión de Inventario</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn v-if="canRead" icon="mdi:mdi-file-pdf-box" @click="handleDownloadPdf" :loading="downloadingPdf" title="Descargar Planilla"></v-btn>
            <v-btn v-if="canRead" icon="mdi:mdi-microsoft-excel" @click="handleDownloadExcel" :loading="downloadingExcel" title="Descargar Excel"></v-btn>
            <v-btn icon="tune" @click="drawerModel = !drawerModel" title="Filtros"></v-btn>
            <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
              <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo"
                hide-details single-line v-model="search" @click:append-inner="handleSearch()"
                @keyup.enter="handleSearch()">
              </v-text-field>
            </v-col>
          </v-toolbar>
        </template>
        <template v-slot:no-data>
          <v-btn color="indigo" @click="$emit('fetch-inventories')"> Reset </v-btn>
        </template>
      </v-data-table-server>
    </v-card>
    <CommonFilters v-model="drawerModel" :filters="filterOptions" v-model:selected-filter="selectedFilterModel"
      v-model:state="stateModel" v-model:start-date="startDateModel" v-model:end-date="endDateModel"
      @apply-filters="handleSearch" />
  </div>
</template>

<script lang="ts">
import { defineComponent, PropType } from 'vue';
import { Inventory } from '@/interfaces/inventoryInterface';
import CommonFilters from '@/components/Common/CommonFilters.vue';

export default defineComponent({
  name: 'InventoryList',
  components: {
    CommonFilters
  },
  props: {
    inventories: {
      type: Array as PropType<Inventory[]>,
      required: true
    },
    loading: {
      type: Boolean,
      required: true
    },
    totalInventories: {
      type: Number,
      required: true
    },
    canRead: {
      type: Boolean,
      required: true
    },
    canEdit: {
      type: Boolean,
      required: true
    },
    drawer: {
      type: Boolean,
      default: false
    },
    selectedFilter: {
      type: String,
      default: 'Código'
    },
    state: {
      type: String,
      default: 'Activos'
    },
    startDate: {
      type: [Date, null] as any,
      default: null
    },
    endDate: {
      type: [Date, null] as any,
      default: null
    },
    downloadingExcel: {
      type: Boolean,
      default: false
    },
    downloadingPdf: {
      type: Boolean,
      default: false
    },
    itemsPerPage: {
      type: Number,
      default: 10
    }
  },
  emits: [
    'open-form',
    'open-modal',
    'edit-inventory',
    'fetch-inventories',
    'search-inventories',
    'update-items-per-page',
    'change-page',
    'download-excel',
    'download-pdf',
    'update:drawer',
    'update:selectedFilter',
    'update:state',
    'update:startDate',
    'update:endDate'
  ],
  data() {
    return {
      pages: "Productos por Página",
      search: null as string | null,
      filterOptions: ['Código', 'Descripción', 'Material', 'Color', 'Categoría', 'Marca']
    };
  },
  computed: {
    headers(): Array<{ title: string; key: string; sortable: boolean; align?: 'start' | 'end' | 'center' }> {
      return [
        { title: 'Código', key: 'code', sortable: false },
        { title: 'Descripción', key: 'description', sortable: false },
        { title: 'Material', key: 'material', sortable: false },
        { title: 'Color', key: 'color', sortable: false },
        { title: 'Categoría', key: 'categoryName', sortable: false },
        { title: 'Marca', key: 'brandName', sortable: false },
        { title: 'Cantidad', key: 'stock', sortable: false, align: 'center' },
        { title: 'Precio', key: 'price', sortable: false, align: 'center' },
        { title: 'Acciones', key: 'actions', sortable: false, align: 'center' },
      ];
    },
    drawerModel: {
      get() {
        return this.drawer;
      },
      set(value: boolean) {
        this.$emit('update:drawer', value);
      }
    },
    selectedFilterModel: {
      get() {
        return this.selectedFilter;
      },
      set(value: string) {
        this.$emit('update:selectedFilter', value);
      }
    },
    stateModel: {
      get() {
        return this.state;
      },
      set(value: string) {
        this.$emit('update:state', value);
      }
    },
    startDateModel: {
      get() {
        return this.startDate;
      },
      set(value: Date | null) {
        this.$emit('update:startDate', value);
      }
    },
    endDateModel: {
      get() {
        return this.endDate;
      },
      set(value: Date | null) {
        this.$emit('update:endDate', value);
      }
    }
  },
  methods: {
    handleSearch() {
      this.$emit('search-inventories', {
        search: this.search,
        selectedFilter: this.selectedFilterModel,
        state: this.stateModel,
        startDate: this.startDateModel,
        endDate: this.endDateModel
      });
    },

    handleDownloadExcel() {
      this.$emit('download-excel', {
        search: this.search,
        selectedFilter: this.selectedFilterModel,
        stateFilter: this.stateModel,
        startDate: this.startDateModel,
        endDate: this.endDateModel
      });
    },

    handleDownloadPdf() {
      this.$emit('download-pdf', {
        search: this.search,
        selectedFilter: this.selectedFilterModel,
        stateFilter: this.stateModel,
        startDate: this.startDateModel,
        endDate: this.endDateModel
      });
    }
  }
});
</script>