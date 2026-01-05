<template>
  <v-dialog v-model="isOpen" max-width="1200px" persistent>
    <v-card elevation="2">
      <v-card-title class="bg-surface-light pt-4">
        <span>Seleccione el Producto</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-row justify="center" align="end">
          <v-col cols="4" md="2" lg="2" xl="2" class="mb-2">
            <v-select color="indigo" variant="underlined" :items="filterOptions" label="Opciones" hide-details />
          </v-col>
          <v-col cols="8" md="6" lg="6" xl="6" class="mb-2">
            <v-text-field append-inner-icon="search" density="compact" label="Búsqueda" variant="underlined"
              hide-details single-line @click:append-inner=""></v-text-field>
          </v-col>
        </v-row>
        <v-data-table :headers="headers" :items-per-page-options="[5, 10, 20]" :items-per-page="5"
          loading-text="Cargando... Espere por favor" page-text="{0}-{1} de {2}">
          <template v-slot:item="{ item }">
            <tr>
              <td>{{ item.code }}</td>
              <td>{{ item.description }}</td>
              <td>{{ item.material }}</td>
              <td>{{ item.colour }}</td>
              <td>{{ item.categorY_NAME }}</td>
              <td>{{ item.branD_NAME }}</td>
              <td>
                <v-btn color="blue" icon="add" variant="text" @click="" size="small"></v-btn>
              </td>
            </tr>
          </template>
        </v-data-table>
      </v-card-text>
      <v-col>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="red" dark class="mb-2" elevation="4" @click="close">Cerrar</v-btn>
        </v-card-actions>
      </v-col>
    </v-card>
  </v-dialog>
</template>
<script lang="ts">
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'CommonProduct',
  props: {
    modelValue: {
      type: Boolean,
      required: true
    }
  },
  emits: ['update:modelValue', 'close'],
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
        { title: 'Marca', key: 'brandName', sortable: false },
        { title: 'Categoría', key: 'categoryName', sortable: false },
        { title: 'Agregar', key: 'actions', sortable: false, align: 'center' },
      ];
    },
    isOpen: {
      get() {
        return this.modelValue;
      },
      set(value: boolean) {
        this.$emit('update:modelValue', value);
      }
    }
  },
  methods: {
    close() {
      this.$emit('update:modelValue', false);
      this.$emit('close');
    }
  }
});
</script>