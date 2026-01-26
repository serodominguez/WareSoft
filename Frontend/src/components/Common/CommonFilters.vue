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
                <v-switch v-model="stateModel" :label="`Estado: ${stateModel}`" false-value="Inactivos"
                    true-value="Activos" color="indigo" hide-details></v-switch>
            </v-list-item>
            <v-list-item>
                <v-date-input v-model="startDateModel" label="Desde:" prepend-icon="" variant="underlined"
                    persistent-placeholder></v-date-input>
            </v-list-item>
            <v-list-item>
                <v-date-input v-model="endDateModel" label="Hasta:" prepend-icon="" variant="underlined"
                    persistent-placeholder></v-date-input>
                <v-btn color="indigo" block @click="$emit('apply-filters')"> Aplicar </v-btn>
            </v-list-item>
            <v-list-item>
                <v-btn color="indigo" block @click="clearFilters"> Limpiar </v-btn>
            </v-list-item>
        </v-list>
    </v-navigation-drawer>
</template>

<script lang="ts">
import { defineComponent, computed, PropType } from 'vue';

export default defineComponent({
    name: 'CommonFilters',
    // Props recibidas del componente padre
    props: {
        // Controla la visibilidad del drawer (patrón v-model)
        modelValue: {
            type: Boolean,
            required: true
        },
        // Array de opciones de filtros disponibles (ej: ['Marca', 'Producto', 'Categoría'])
        filters: {
            type: Array as PropType<string[]>,
            required: true,
            validator: (value: string[]) => value.length > 0 // Valida que haya al menos un filtro
        },
        // Filtro actualmente seleccionado
        selectedFilter: {
            type: String,
            required: true
        },
        // Estado del filtro (Activos/Inactivos)
        state: {
            type: String,
            default: 'Activos'
        },
        // Fecha de inicio del rango de filtrado
        startDate: {
            type: [Date, null] as any,
            default: null
        },
        // Fecha de fin del rango de filtrado
        endDate: {
            type: [Date, null] as any,
            default: null
        }
    },
    // Todos los update:* son para implementar v-model en múltiples propiedades
    emits: [
        'update:modelValue',             // Actualiza visibilidad del drawer
        'update:selectedFilter',        // Actualiza filtro seleccionado
        'update:state',                // Actualiza estado (Activos/Inactivos)
        'update:startDate',           // Actualiza fecha de inicio
        'update:endDate',            // Actualiza fecha de fin
        'apply-filters'             // Se dispara al hacer clic en "Aplicar"
    ],
    //  Define la lógica reactiva del componente
    setup(props, { emit }) {
        // Implementa el patrón v-model
        const drawerModel = computed({
            get: () => props.modelValue,
            set: (value) => emit('update:modelValue', value)
        });
        // Permite cambiar dinámicamente el tipo de filtro (Marca, Producto, etc.)
        const selectedFilterModel = computed({
            get: () => props.selectedFilter,
            set: (value) => emit('update:selectedFilter', value)
        });
        // Alterna entre "Activos" e "Inactivos"
        const stateModel = computed({
            get: () => props.state,
            set: (value) => emit('update:state', value)
        });
        // Permite seleccionar desde qué fecha filtrar los registros
        const startDateModel = computed({
            get: () => props.startDate,
            set: (value) => emit('update:startDate', value)
        });

        const endDateModel = computed({
            get: () => props.endDate,
            set: (value) => emit('update:endDate', value)
        });
        // Limpia/resetea todos los filtros a sus valores por defecto
        const clearFilters = () => {
            selectedFilterModel.value = props.filters[0];
            stateModel.value = 'Activos';
            startDateModel.value = null;
            endDateModel.value = null;
        };
        // Expone las propiedades y métodos al template
        return {
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