<template>
  <v-dialog v-model="isOpen" max-width="400px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4" v-if="action === 0">Eliminar Item?</v-card-title>
      <v-card-title class="bg-surface-light pt-4" v-if="action === 1">Activar Item?</v-card-title>
      <v-card-title class="bg-surface-light pt-4" v-if="action === 2">Desactivar Item?</v-card-title>
      <v-divider></v-divider>

      <v-card-text>
        Estás a punto de
        <span v-if="action === 0">eliminar</span>
        <span v-if="action === 1">activar</span>
        <span v-if="action === 2">desactivar</span>
        el ítem: {{ localProduct.description }}.
      </v-card-text>

      <v-card-actions class="d-flex justify-space-between">
        <div class="d-flex">
          <v-btn v-if="action === 0" color="indigo" dark class="mr-2" elevation="4" @click="remove"
            :loading="processing">
            Eliminar
          </v-btn>

          <v-btn v-if="action === 1" color="indigo" dark class="mr-2" elevation="4" @click="enabled"
            :loading="processing">
            Activar
          </v-btn>

          <v-btn v-if="action === 2" color="indigo" dark class="mr-2" elevation="4" @click="disabled"
            :loading="processing">
            Desactivar
          </v-btn>

          <v-btn color="red" elevation="4" @click="close" :disabled="processing">
            Cancelar
          </v-btn>
        </div>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import { useToast } from 'vue-toastification';
import { defineComponent, PropType } from 'vue';
import { Product } from '@/interfaces/productInterface';
import { handleApiError } from '@/helpers/errorHandler';

export default defineComponent({
  name: 'ProductModal',
  props: {
    modelValue: {
      type: Boolean,
      required: true,
    },
    product: {
      type: Object as PropType<Product | null>,
      default: () => ({
        idProduct: null,
        description: ''
      }),
    },
    action: {
      type: Number,
      required: true,
    },
  },
  emits: ['update:modelValue', 'action-completed'],
  data() {
    return {
      isOpen: this.modelValue,
      processing: false,
      toast: useToast(),
      localProduct: { ...this.product } as Product,
    };
  },
  watch: {
    modelValue(newValue: boolean) {
      this.isOpen = newValue;
    },
    product: {
      handler(newProduct: Product) {
        this.localProduct = { ...newProduct };
      },
      deep: true,
    },
  },
  methods: {
    close() {
      this.isOpen = false;
      this.$emit('update:modelValue', false);
    },
    
    async remove() {
      this.processing = true;
      try {
        const result = await this.$store.dispatch('product/removeProduct', this.localProduct.idProduct);
        if (result.isSuccess) {
          this.toast.success('Producto eliminado con éxito!');
          this.$emit('action-completed');
          this.close();
        }
      } catch (error: any) {
        handleApiError(error, 'Error al eliminar el producto');
      } finally {
        this.processing = false;
      }
    },
    
    async enabled() {
      this.processing = true;
      try {
        const result = await this.$store.dispatch('product/enableProduct', this.localProduct.idProduct);
        if (result.isSuccess) {
          this.toast.success('Producto habilitado con éxito!');
          this.$emit('action-completed');
          this.close();
        }
      } catch (error: any) {
        handleApiError(error, 'Error al habilitar el producto');
      } finally {
        this.processing = false;
      }
    },
    
    async disabled() {
      this.processing = true;
      try {
        const result = await this.$store.dispatch('product/disableProduct', this.localProduct.idProduct);
        if (result.isSuccess) {
          this.toast.success('Producto deshabilitado con éxito!');
          this.$emit('action-completed');
          this.close();
        }
      } catch (error: any) {
        handleApiError(error, 'Error al deshabilitar el producto');
      } finally {
        this.processing = false;
      }
    },
  },
});
</script>