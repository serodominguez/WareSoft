<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{'Editar Precio'}}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localInventory.code" label="Código" readonly />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localInventory.description" label="Descripción" readonly />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localInventory.material" label="Material" readonly />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localInventory.unitMeasure" label="Unidad de Medida" readonly />
              </v-col>
             <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localInventory.brandName" label="Marca" readonly />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localInventory.categoryName" label="Categoría" readonly />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localInventory.stock" label="Cantidad" readonly />
              </v-col>
             <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model.number="localInventory.price" counter="5" type="number"
                  :rules="[rules.required]" :maxlength="5" label="Precio" ref="priceField" 
                  @focus="$event.target.select()" required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="green" dark class="mb-2" elevation="4" @click="savePrice" :disabled="!valid"
            :loading="saving">Guardar</v-btn>
          <v-btn color="red" dark class="mb-2" elevation="4" @click="close">Cancelar</v-btn>
        </v-card-actions>
      </v-col>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import { Store as VuexStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { defineComponent, PropType } from 'vue';
import { Inventory } from '@/interfaces/inventoryInterface';
import { handleApiError } from '@/helpers/errorHandler';

interface FormRef {
  validate: () => boolean;
}

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $store: VuexStore<any>;
  }
}

export default defineComponent({
  props: {
    modelValue: {
      type: Boolean,
      required: true,
    },
    inventory: {
      type: Object as PropType<Inventory | null>,
      default: () => ({
        idStore: null,
        idProduct: null,
        code: '',
        description: '',
        material: '',
        color: '',
        unitMeasure: '',
        stock: null,
        price: null,
        brandName: '',
        categoryName: ''
      }),
    },
  },
  data() {
    return {
      isOpen: this.modelValue,
      valid: false,
      saving: false,
      localInventory: { ...this.inventory } as Inventory,
      toast: useToast(),
      rules: {
        required: (value: string) => !!value || 'Este campo es requerido.',
        onlyNumbers: (value: string) => !value || /^[0-9]+$/.test(value) || 'Solo se permiten números.',
      },
    };
  },
  watch: {
    modelValue(newValue: boolean) {
      this.isOpen = newValue; // Añadir esta línea
    },
    isOpen(newValue: boolean) {
      if (newValue) {
        // Espera a que el modal se renderice completamente
        this.$nextTick(() => {
          const priceField = this.$refs.priceField as any; // Asigna un ref al campo
          if (priceField) {
            priceField.focus();
          }
        });
      }

      this.$emit('update:modelValue', newValue);
    },
    inventory: {
      handler(newInventory: Inventory) {
        this.localInventory = { ...newInventory };
      },
      deep: true,
    },
  },
  methods: {
    close() {
      this.isOpen = false;
    },
    async savePrice() {
      const form = this.$refs.form as FormRef;
      if (!form.validate()) {
        this.toast.warning('Por favor completa todos los campos requeridos');
        return;
      }

      try {
        this.saving = true;

        const result = await this.$store.dispatch('inventory/editInventoryPrice', { inventory: { ...this.localInventory }});

        if (result.isSuccess) {
          this.toast.success('Precio actualizado con éxito!');
          this.$emit('saved', { ...this.localInventory });
          this.close();
        }

      } catch (error: any) {
        handleApiError(error, 'Error al actualizar el precio');
      } finally {
        this.saving = false;
      }
    },
  },
});
</script>