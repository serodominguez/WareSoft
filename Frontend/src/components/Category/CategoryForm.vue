<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localCategory.idCategory ? 'Editar Categoría' : 'Agregar Categoría' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localCategory.categoryName"
                  :rules="[rules.required, rules.onlyLetters]" counter="25" :maxlength="25"
                  label="Nombre de la Categoría" required />
              </v-col>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localCategory.description"
                  :rules="[rules.onlyLetters]" counter="50" :maxlength="50" label="Descripción" />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="green" dark class="mb-2" elevation="4" @click="saveCategory" :disabled="!valid"
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
import { Category } from '@/interfaces/categoryInterface';
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
    category: {
      type: Object as PropType<Category | null>,
      default: () => ({
        idCategory: null,
        categoryName: '',
        description: ''
      }),
    },
  },
  data() {
    return {
      isOpen: this.modelValue,
      valid: false,
      saving: false,
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
    close() {
      this.isOpen = false;
    },
    async saveCategory() {
      const form = this.$refs.form as FormRef;
      if (!form.validate()) {
        this.toast.warning('Por favor completa todos los campos requeridos');
        return;
      }
      this.saving = true;
      try {
        const isEditing = !!this.localCategory.idCategory;
        let result;

        if (isEditing) {
          result = await this.$store.dispatch('category/editCategory', {
            id: this.localCategory.idCategory,
            category: { ...this.localCategory }
          });
        } else {
          result = await this.$store.dispatch('category/registerCategory', { ...this.localCategory });
        }

        if (result.isSuccess) {
          const successMsg = isEditing
            ? 'Categoría actualizada con éxito!'
            : 'Categoría registrada con éxito!';

          this.toast.success(successMsg);
          this.$emit('saved', { ...this.localCategory });
          this.close();
        }
      } catch (error: any) {
        const isEditing = !!this.localCategory.idCategory;
        const customMessage = isEditing
          ? 'Error en actualizar la categoría'
          : 'Error en guardar la categoría';

        handleApiError(error, customMessage);
      } finally {
        this.saving = false;
      }
    },
  },
});
</script>