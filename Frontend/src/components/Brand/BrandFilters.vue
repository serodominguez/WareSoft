<template>
  <v-navigation-drawer v-model="drawerModel" temporary app>
    <v-list>
      <v-list-item>
      <div class="d-flex justify-space-between align-center w-100">
          <v-list-item-title class="text-h6">Filtros</v-list-item-title>
          <v-btn icon="close" variant="text" size="small" @click="drawerModel = false"></v-btn>
        </div>
      </v-list-item>
      <v-list-item>
        <v-select v-model="selectedFilterModel" :items="filters" variant="outlined" density="compact"
          hide-details></v-select>
      </v-list-item>
      <v-list-item>
        <v-switch v-model="stateModel" :label="`Estado: ${stateModel}`" false-value="Inactivos" true-value="Activos"
          color="indigo" hide-details></v-switch>
      </v-list-item>
      <v-list-item>
        <v-date-input v-model="startDateModel" label="Desde:" prepend-icon="" variant="underlined"
          persistent-placeholder></v-date-input>
      </v-list-item>
      <v-list-item>
        <v-date-input v-model="endDateModel" label="Hasta:" prepend-icon="" variant="underlined"
          persistent-placeholder></v-date-input>
        <v-btn color="primary" block @click="clearFilters"> Limpiar Filtros </v-btn>
      </v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<script lang="ts">
import { defineComponent, computed } from 'vue';

export default defineComponent({
  name: 'BrandFilters',
  props: {
    modelValue: {
      type: Boolean,
      required: true
    },
    selectedFilter: {
      type: String,
      default: 'Marca'
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
    }
  },
  emits: ['update:modelValue', 'update:selectedFilter', 'update:state', 'update:startDate', 'update:endDate'],
  setup(props, { emit }) {
    const filters = ['Marca'];

    const drawerModel = computed({
      get: () => props.modelValue,
      set: (value) => emit('update:modelValue', value)
    });

    const selectedFilterModel = computed({
      get: () => props.selectedFilter,
      set: (value) => emit('update:selectedFilter', value)
    });

    const stateModel = computed({
      get: () => props.state,
      set: (value) => emit('update:state', value)
    });

    const startDateModel = computed({
      get: () => props.startDate,
      set: (value) => emit('update:startDate', value)
    });

    const endDateModel = computed({
      get: () => props.endDate,
      set: (value) => emit('update:endDate', value)
    });

    const clearFilters = () => {
      selectedFilterModel.value = 'Marca';
      stateModel.value = 'Activos';
      startDateModel.value = null;
      endDateModel.value = null;
    };

    return {
      filters,
      drawerModel,
      selectedFilterModel,
      stateModel,
      startDateModel,
      endDateModel,
      clearFilters
    };
  }
});
</script>