<template>
  <div>
    <v-card elevation="2">
      <v-data-table-server :headers="headers" :items="brands" :search="search || undefined" :items-per-page-text="pages"
        :items-per-page-options="[10, 20, 50]" :items-per-page="itemsPerPage" :items-length="totalBrands"
        :loading="loading" loading-text="Cargando... Espere por favor"
        @update:items-per-page="$emit('update-items-per-page', $event)" @update:page="$emit('change-page', $event)">
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ (item as Brand).brandName }}</td>
            <td>{{ (item as Brand).auditCreateDate }}</td>
            <td>{{ (item as Brand).statusBrand }}</td>
            <td class="text-center">
              <v-btn v-if="canEdit && (item as Brand).statusBrand == 'Activo'" color="indigo" icon="edit" variant="text"
                @click="$emit('edit-brand', item)" size="small" title="Editar">
              </v-btn>
              <template v-if="canEdit && (item as Brand).statusBrand == 'Inactivo'">
                <v-btn color="green" icon="check" variant="text"
                  @click="$emit('open-modal', { brand: item, action: 1 })" size="small" title="Activar">
                </v-btn>
              </template>
              <template v-if="canEdit && (item as Brand).statusBrand == 'Activo'">
                <v-btn color="red" icon="block" variant="text" @click="$emit('open-modal', { brand: item, action: 2 })"
                  size="small" title="Desactivar">
                </v-btn>
              </template>
              <v-btn v-if="canDelete" color="grey" icon="delete" variant="text"
                @click="$emit('open-modal', { brand: item, action: 0 })" size="small" title="Eliminar">
              </v-btn>
            </td>
          </tr>
        </template>
        <template v-slot:top>
          <v-toolbar>
            <v-toolbar-title>Gestión de Marcas</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn v-if="canRead" icon="mdi:mdi-microsoft-excel" @click="handleDownloadExcel" :loading="downloadingExcel" title="Descargar Excel"></v-btn>
            <v-btn icon="tune" @click="drawerModel = !drawerModel" title="Filtros"></v-btn>
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
          <v-btn color="indigo" @click="$emit('fetch-brands')"> Reset </v-btn>
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
import { Brand } from '@/interfaces/brandInterface';
import CommonFilters from '@/components/Common/CommonFilters.vue';

export default defineComponent({
  name: 'BrandList',
  components: {
    CommonFilters // Componente de filtros reutilizable
  },
  // Props recibidas del componente padre incluye datos, permisos y configuración de filtros
  props: {
    // Array de la entidad a mostrar en la tabla
    brands: {
      type: Array as PropType<Brand[]>,
      required: true
    },
    // Indica si los datos están cargando
    loading: {
      type: Boolean,
      required: true
    },
    // Total de la entidad (para paginación del servidor)
    totalBrands: {
      type: Number,
      required: true
    },
    // Permisos CRUD del usuario actual
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
    // Estado del drawer de filtros
    drawer: {
      type: Boolean,
      default: false
    },
    // Filtro seleccionado actualmente
    selectedFilter: {
      type: String,
      default: 'Marca'
    },
    // Estado de filtro (Activos/Inactivos/Todos)
    state: {
      type: String,
      default: 'Activos'
    },
    // Rango de fechas para filtrar
    startDate: {
      type: [Date, null] as any,
      default: null
    },
    endDate: {
      type: [Date, null] as any,
      default: null
    },
    // Estado de descarga de Excel
    downloadingExcel: {
      type: Boolean,
      default: false
    },
    itemsPerPage: {
    type: Number,
    default: 10
  }
  },
  /**
   * Eventos que este componente puede emitir al padre
   * Sigue el patrón de comunicación padre-hijo de Vue
   */
  emits: [
    'open-form',                        // Abrir formulario para crear
    'open-modal',                      // Abrir modal de confirmación (activar/desactivar/eliminar)
    'edit-brand',                     // Editar marca existente
    'fetch-brands',                  // Recargar/obtener marca
    'search-brands',                // Recargar/obtener marca
    'update-items-per-page',       // Cambiar cantidad de items por página
    'change-page',                // Cambiar de página
    'download-excel',            // Descargar reporte Excel
    'update:drawer',            // Actualizar estado del drawer (patrón v-model)
    'update:selectedFilter',   // Actualizar filtro seleccionado
    'update:state',           // Actualizar estado de filtro
    'update:startDate',      // Actualizar fecha inicio
    'update:endDate'        // Actualizar fecha fin
  ],
  data() {
    return {
      pages: "Marcas por Página",      // Texto para selector de items por página
      search: null as string | null,  // Término de búsqueda actual
      filterOptions: ['Marca']       // Opciones disponibles para filtrar
    };
  },
  // Propiedades computadas
  computed: {
    // Define las columnas/encabezados de la tabla
    headers(): Array<{ title: string; key: string; sortable: boolean; align?: 'start' | 'end' | 'center' }> {
      return [
        { title: 'Marca', key: 'brandName', sortable: false },
        { title: 'Fecha de registro', key: 'auditCreateDate', sortable: false },
        { title: 'Estado', key: 'statusBrand', sortable: false },
        { title: 'Acciones', key: 'actions', sortable: false, align: 'center' },
      ];
    },
    // Computed property bidireccional para el drawer de filtros
    drawerModel: {
      get() {
        return this.drawer;
      },
      set(value: boolean) {
        this.$emit('update:drawer', value);
      }
    },
    // Computed property bidireccional para el filtro seleccionado
    selectedFilterModel: {
      get() {
        return this.selectedFilter;
      },
      set(value: string) {
        this.$emit('update:selectedFilter', value);
      }
    },
    // Computed property bidireccional para el estado del filtro
    stateModel: {
      get() {
        return this.state;
      },
      set(value: string) {
        this.$emit('update:state', value);
      }
    },
    // Computed property bidireccional para la fecha de inicio
    startDateModel: {
      get() {
        return this.startDate;
      },
      set(value: Date | null) {
        this.$emit('update:startDate', value);
      }
    },
    // Computed property bidireccional para la fecha de fin
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
    // Maneja la búsqueda de marcas con todos los filtros activos
    handleSearch() {
      this.$emit('search-brands', {
        search: this.search,
        selectedFilter: this.selectedFilterModel,
        state: this.stateModel,
        startDate: this.startDateModel,
        endDate: this.endDateModel
      });
    },
    // Maneja la descarga del reporte Excel
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