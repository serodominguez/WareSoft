<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localBrand.idBrand ? 'Editar Marca' : 'Agregar Marca' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localBrand.brandName"
                  :rules="[rules.required]" counter="25" :maxlength="25"
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
import { Brand } from '@/interfaces/brandInterface';

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
        idBrand: null,
        brandName: ''
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
    close() {
      this.isOpen = false;
    },
    async saveBrand() {
      const form = this.$refs.form as FormRef;
      if (form.validate()) {
        try {
          const isEditing = !!this.localBrand.idBrand;
          let result;

          if (isEditing) {
            result = await this.$store.dispatch('brand/editBrand', {
              id: this.localBrand.idBrand,
              brand: { ...this.localBrand }
            });
          } else {
            result = await this.$store.dispatch('brand/registerBrand', { ...this.localBrand });
          }

          if (result.isSuccess) {
            const successMsg = isEditing 
              ? 'Marca actualizada con éxito!'
              : 'Marca registrada con éxito!';

            this.toast.success(successMsg);
            this.$emit('saved', { ...this.localBrand });
            this.close();
          }
          
        } catch (error: any) {
          const isEditing = !!this.localBrand.idBrand;
          let errorMsg = isEditing 
            ? 'Error en actualizar la marca'
            : 'Error en guardar la marca';

          if (error?.response?.status) {
            errorMsg += `: Error ${error.response.status}`;
          } else if (error?.response?.data?.message) {
            errorMsg += `: ${error.response.data.message}`;
          } else if (error?.message) {
            errorMsg += `: ${error.message}`;
          } else {
            errorMsg += '.';
          }

          this.toast.error(errorMsg);
        }
      }
    },
  },
});
</script>