<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localBrand.pK_BRAND ? 'Editar Marca' : 'Agregar Marca' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localBrand.branD_NAME"
                  :rules="[rules.required]" counter="25" :maxlength="25" @keyup="uppercase"
                  label="Nombre de la Marca" required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="indigo" dark class="mb-2" elevation="4" @click="saveBrand" :disabled="!valid">Guardar</v-btn>
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
import { Brand } from '@/models/brandModel';

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
    brand: {
      type: Object as PropType<Brand | null>,
      default: () => ({
        pK_BRAND: null,
        branD_NAME: ''
      }),
    },
  },
  data() {
    return {
      isOpen: this.modelValue,
      valid: false,
      localBrand: { ...this.brand } as Brand,
      toast: useToast(),
      rules: {
        required: (value: string) => !!value || 'Este campo es requerido.',
      },
    };
  },
  watch: {
    modelValue(newValue: boolean) {
      this.isOpen = newValue;
    },
    isOpen(newValue: boolean) {
      this.$emit('update:modelValue', newValue);
    },
    brand: {
      handler(newBrand: Brand) {
        this.localBrand = { ...newBrand };
      },
      deep: true,
    },
  },
  methods: {
    uppercase() {
      this.localBrand.branD_NAME = this.localBrand.branD_NAME.toUpperCase();
    },
    close() {
      this.isOpen = false;
    },
    async saveBrand() {
      const form = this.$refs.form as FormRef;
      if (form.validate()) {
        try {
          if (this.localBrand.pK_BRAND) {
            await this.$store.dispatch('brand/editBrand', {
              id: this.localBrand.pK_BRAND,
              brand: { ...this.localBrand }
            });
            this.toast.success('Marca actualizada con éxito!');
          } else {
            await this.$store.dispatch('brand/registerBrand', { ...this.localBrand });
            this.toast.success('Marca agregada con éxito!');
          }
          this.$emit('saved', { ...this.localBrand });
          this.close();
        } catch (error: any) {
          if (error.response) {
            this.toast.error('Error en generar la marca.');
          }
        }
      }
    },
  },
});
</script>