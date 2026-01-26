<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="goodsissue" :search="search || undefined"
        :items-per-page-text="pages" :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage"
        :items-length="totalGoodsIssue" :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as GoodsIssue).code }}</td>
            <td>{{ (item as GoodsIssue).type }}</td>
            <td>{{ (item as GoodsIssue).storeName }}</td>
            <td>{{ (item as GoodsIssue).userName }}</td>
            <td>{{ (item as GoodsIssue).auditCreateDate }}</td>
            <td>{{ (item as GoodsIssue).statusIssue }}</td>
            <td class="text-center">
              <template v-if="canRead">
              <v-btn color="indigo" icon="preview" variant="text" @click="$emit('view-goodsissue', item)" size="small"
                title="Ver">
              </v-btn>
              </template>
              <template v-if="canRead && (item as GoodsIssue).statusIssue == 'Activo'">
              <v-btn color="grey" icon="picture_as_pdf" variant="text" @click="$emit('print-pdf', item)" size="small"
                title="Imprimir">
              </v-btn>
              </template>
              <template v-if="canDelete && (item as GoodsIssue).statusIssue == 'Activo'">
                <v-btn color="red" icon="block" variant="text"
                  @click="$emit('open-modal', { goodsissue: item, action: 2 })" size="small" title="Desactivar">
                </v-btn>
              </template>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title>Gestión de Salidas</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn v-if="canRead" icon="mdi:mdi-microsoft-excel" @click="handleDownloadExcel" :loading="downloadingExcel"
              title="Descargar Excel"></v-btn>
            <v-btn v-if="canRead" icon="tune" @click="drawerModel = !drawerModel" title="Filtros"></v-btn>
            <v-btn v-if="canCreate" icon="add_box" @click="$emit('open-form')" title="Registrar"></v-btn>
            <v-col cols="4" md="3" lg="3" xl="3" class="pa-1">
              <v-text-field v-if="canRead" append-inner-icon="search" density="compact" label="Búsqueda" variant="solo"
                hide-details single-line v-model="search" @click:append-inner="handleSearch()"
                @keyup.enter="handleSearch()">
              </v-text-field>
            </v-col>
          </v-toolbar>
        </template>
        <template v-slot:no-data>
          <v-btn color="indigo" @click="$emit('fetch-goodsissue')"> Reset </v-btn>
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
import { GoodsIssue } from '@/interfaces/goodsIssueInterface';
import CommonFilters from '@/components/Common/CommonFilters.vue';

export default defineComponent({
  name: 'GoodsIssueList',
  components: {
    CommonFilters
  },
  props: {
    goodsissue: {
      type: Array as PropType<GoodsIssue[]>,
      required: true
    },
    loading: {
      type: Boolean,
      required: true
    },
    totalGoodsIssue: {
      type: Number,
      required: true
    },
    canCreate: {
      type: Boolean,
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
    canDelete: {
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
    itemsPerPage: {
      type: Number,
      default: 10
    }
  },
  emits: [
    'open-form',
    'open-modal',
    'view-goodsissue',
    'fetch-goodsissue',
    'search-goodsissue',
    'update-items-per-page',
    'change-page',
    'download-excel',
    'print-pdf',
    'update:drawer',
    'update:selectedFilter',
    'update:state',
    'update:startDate',
    'update:endDate'
  ],
  data() {
    return {
      pages: "Salidas por Página",
      search: null as string | null,
      filterOptions: ['Código', 'Tienda', 'Proveedor']
    };
  },
  computed: {
    headers(): Array<{ title: string; key: string; sortable: boolean; align?: 'start' | 'end' | 'center' }> {
      return [
        { title: 'Código', key: 'code', sortable: false },
        { title: 'Tipo', key: 'type', sortable: false },
        { title: 'Tienda', key: 'storeName', sortable: false },
        { title: 'Personal', key: 'userName', sortable: false },
        { title: 'Fecha de registro', key: 'auditCreateDate', sortable: false },
        { title: 'Estado', key: 'statusIssue', sortable: false },
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
      this.$emit('search-goodsissue', {
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
    }
  }
});
</script>