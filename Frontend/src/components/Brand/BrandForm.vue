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
                <v-text-field color="indigo" variant="underlined" v-model="localBrand.brandName"
                  :rules="[rules.required]" counter="25" :maxlength="25" label="Nombre de la Marca" required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="green" dark class="mb-2" elevation="4" @click="saveBrand" :disabled="!valid"
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
import { Brand } from '@/interfaces/brandInterface';
import { handleApiError } from '@/helpers/errorHandler';

// Interfaz para el tipo de referencia del formulario
interface FormRef {
  validate: () => boolean;
}

// Extensión de las propiedades personalizadas del componente para TypeScript
declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $store: VuexStore<any>;
  }
}

export default defineComponent({
    /**
   * Props del componente:
   * - modelValue: Controla si el diálogo está abierto o cerrado (patrón v-model)
   * - brand: Objeto con los datos de la entidad a editar, o valores por defecto para nueva entidad
   */
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

  // Estado reactivo del componente
  data() {
    return {
      isOpen: this.modelValue,              // Estado local de visibilidad del diálogo
      valid: false,                        // Estado de validación del formulario
      saving: false,                      // Indica si se está guardando (para mostrar loading)
      localBrand: { ...this.brand } as Brand,  // Copia local de la marca para editar
      toast: useToast(),                      // Instancia de notificaciones toast
      rules: {
        // Reglas de validación
        required: (value: string) => !!value || 'Este campo es requerido.',
      },
    };
  },

  // Watchers para sincronizar props con estado local
  watch: {
    // Sincroniza cambios del prop modelValue con el estado local isOpen
    modelValue(newValue: boolean) {
      this.isOpen = newValue;
    },
    // Emite cambios del estado local isOpen al componente padre (patrón v-model)
    isOpen(newValue: boolean) {
      this.$emit('update:modelValue', newValue);
    },
    // Sincroniza cambios en el prop brand con la copia local
    brand: {
      handler(newBrand: Brand) {
        this.localBrand = { ...newBrand };
      },
      deep: true,
    },
  },
  methods: {
    // Cierra el diálogo
    close() {
      this.isOpen = false;
    },
    // Guarda o actualiza la entidad
    async saveBrand() {
      // Obtiene la referencia del formulario y valida
      const form = this.$refs.form as FormRef;
      if (!form.validate()) {
        this.toast.warning('Por favor completa todos los campos requeridos');
        return;
      }
      this.saving = true; // Activa estado de carga
      try {
        // Determina si es edición o creación según si existe id de la entidad
        const isEditing = !!this.localBrand.idBrand;
        let result;

        if (isEditing) {
          // Llama al action de Vuex para editar entidad existente
          result = await this.$store.dispatch('brand/editBrand', {
            id: this.localBrand.idBrand,
            brand: { ...this.localBrand }
          });
        } else {
          // Llama al action de Vuex para registrar nuevo
          result = await this.$store.dispatch('brand/registerBrand', { ...this.localBrand });
        }

        // Si la operación fue exitosa
        if (result.isSuccess) {
          const successMsg = isEditing
            ? 'Marca actualizada con éxito!'
            : 'Marca registrada con éxito!';

          this.toast.success(successMsg);                 // Muestra notificación de éxito
          this.$emit('saved', { ...this.localBrand });   // Emite evento al componente padre
          this.close();                                 // Cierra el diálogo
        }

      } catch (error: any) {
        // Manejo de errores
        const isEditing = !!this.localBrand.idBrand;
        const customMessage = isEditing
          ? 'Error al actualizar la marca'
          : 'Error al guardar la marca';

        handleApiError(error, customMessage); // Usa helper para manejar el error
      } finally {
        this.saving = false; // Desactiva estado de carga
      }
    },
  },
});
</script>