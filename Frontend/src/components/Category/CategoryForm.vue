<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localCategory.pK_CATEGORY ? 'Editar Categoría' : 'Agregar Categoría' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localCategory.categorY_NAME"
                  :rules="[rules.required, rules.onlyLetters]" counter="25" :maxlength="25" @keyup="uppercase"
                  label="Nombre de la Categoría" required />
              </v-col>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localCategory.description"
                  :rules="[rules.required, rules.onlyLetters]" counter="50" :maxlength="50" @keyup="uppercase"
                  label="Descripción" required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="indigo" dark class="mb-2" elevation="4" @click="saveCategory" :disabled="!valid">Guardar</v-btn>
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
import { Category } from '@/models/categoryModel';

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
    category: {
      type: Object as PropType<Category | null>,
      default: () => ({
        pK_CATEGORY: null,
        categorY_NAME: '',
        description: ''
      }),
    },
  },
  data() {
    return {
      isOpen: this.modelValue,
      valid: false,
      localCategory: { ...this.category } as Category,
      toast: useToast(),
      rules: {
        required: (value: string) => !!value || 'Este campo es requerido.',
        onlyLetters: (value: string) => !value || /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(value) || 'Solo se permiten letras.',
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
    category: {
      handler(newCategory: Category) {
        this.localCategory = { ...newCategory };
      },
      deep: true,
    },
  },
  methods: {
    uppercase() {
      this.localCategory.categorY_NAME = this.localCategory.categorY_NAME.toUpperCase();
      this.localCategory.description = this.localCategory.description.toUpperCase();
    },
    close() {
      this.isOpen = false;
    },
    async saveCategory() {
      const form = this.$refs.form as FormRef;
      if (form.validate()) {
        try {
          if (this.localCategory.pK_CATEGORY) {
            await this.$store.dispatch('category/editCategory', {
              id: this.localCategory.pK_CATEGORY,
              category: { ...this.localCategory }
            });
            this.toast.success('Categoría actualizada con éxito!');
          } else {
            await this.$store.dispatch('category/registerCategory', { ...this.localCategory });
            this.toast.success('Categoría agregada con éxito!');
          }
          this.$emit('saved', { ...this.localCategory });
          this.close();
        } catch (error: any) {
          if (error.response) {
            this.toast.error('Error en generar la categoría.');
          }
        }
      }
    },
  },
});
</script>