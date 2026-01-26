<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localSupplier.idSupplier ? 'Editar Proveedor' : 'Agregar Proveedor' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localSupplier.companyName"
                  :rules="[rules.required, rules.onlyLetters]" counter="50" :maxlength="50"
                  label="Nombre de la empresa" required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localSupplier.contact"
                  :rules="[rules.required, rules.onlyLetters]" counter="50" :maxlength="50" label="Contacto" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localSupplier.phoneNumber" counter="8"
                  :rules="[rules.onlyNumbers]" :maxlength="8" label="Teléfono" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localSupplier.email" counter="50"
                  :rules="[rules.email]" :maxlength="50" label="Correo" />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="green" dark class="mb-2" elevation="4" @click="saveSupplier" :disabled="!valid"
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
import { Supplier } from '@/interfaces/supplierInterface';
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
    supplier: {
      type: Object as PropType<Supplier | null>,
      default: () => ({
        idSupplier: null,
        companyName: '',
        contact: '',
        phoneNumber: null,
        email: ''
      }),
    },
  },
  data() {
    return {
      isOpen: this.modelValue,
      valid: false,
      saving: false,
      localSupplier: { ...this.supplier } as Supplier,
      toast: useToast(),
      rules: {
        required: (value: string) => !!value || 'Este campo es requerido.',
        onlyLetters: (value: string) => !value || /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(value) || 'Solo se permiten letras.',
        onlyNumbers: (value: string) => !value || /^[0-9]+$/.test(value) || 'Solo se permiten números.',
        email: (value: string) => !value || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value) || 'Formato de correo inválido.',
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
    supplier: {
      handler(newSupplier: Supplier) {
        this.localSupplier = { ...newSupplier };
      },
      deep: true,
    },
  },
  methods: {
    close() {
      this.isOpen = false;
    },
    async saveSupplier() {
      const form = this.$refs.form as FormRef;
      if (!form.validate()) {
        this.toast.warning('Por favor completa todos los campos requeridos');
        return;
      }
      this.saving = true;
      try {
        const isEditing = !!this.localSupplier.idSupplier;
        let result;

        if (isEditing) {
          result = await this.$store.dispatch('supplier/editSupplier', {
            id: this.localSupplier.idSupplier,
            supplier: { ...this.localSupplier }
          });
        } else {
          result = await this.$store.dispatch('supplier/registerSupplier', { ...this.localSupplier });
        }

        if (result.isSuccess) {
          const successMsg = isEditing
            ? 'Proveedor actualizado con éxito!'
            : 'Proveedor registrado con éxito!';

          this.toast.success(successMsg);
          this.$emit('saved', { ...this.localSupplier });
          this.close();
        }
      } catch (error: any) {
        const isEditing = !!this.localSupplier.idSupplier;
        const customMessage = isEditing
          ? 'Error en actualizar al proveedor'
          : 'Error en guardar al proveedor';

        handleApiError(error, customMessage);
      } finally {
        this.saving = false;
      }
    },
  },
});
</script>