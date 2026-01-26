<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localCustomer.idCustomer ? 'Editar Cliente' : 'Agregar Cliente' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localCustomer.names"
                  :rules="[rules.required, rules.onlyLetters]" counter="25" :maxlength="25"
                  label="Nombre del cliente" required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localCustomer.lastNames"
                  :rules="[rules.required, rules.onlyLetters]" counter="50" :maxlength="50" label="Apellidos" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localCustomer.identificationNumber" counter="8"
                  :maxlength="8" label="Carnet" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localCustomer.phoneNumber" counter="8"
                  :rules="[rules.onlyNumbers]" :maxlength="8" label="Teléfono" />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="green" dark class="mb-2" elevation="4" @click="saveCustomer" :disabled="!valid"
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
import { Customer } from '@/interfaces/customerInterface';
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
    customer: {
      type: Object as PropType<Customer | null>,
      default: () => ({
        idCustomer: null,
        names: '',
        lastNames: '',
        identificationNumber: '',
        phoneNumber: null
      }),
    },
  },
  data() {
    return {
      isOpen: this.modelValue,
      valid: false,
      saving: false,
      localCustomer: { ...this.customer } as Customer,
      toast: useToast(),
      rules: {
        required: (value: string) => !!value || 'Este campo es requerido.',
        onlyLetters: (value: string) => !value || /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(value) || 'Solo se permiten letras.',
        onlyNumbers: (value: string) => !value || /^[0-9]+$/.test(value) || 'Solo se permiten números.',
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
    customer: {
      handler(newCustomer: Customer) {
        this.localCustomer = { ...newCustomer };
      },
      deep: true,
    },
  },
  methods: {
    close() {
      this.isOpen = false;
    },
    async saveCustomer() {
      const form = this.$refs.form as FormRef;
      if (!form.validate()) {
        this.toast.warning('Por favor completa todos los campos requeridos');
        return;
      }
      this.saving = true;
      try {
        const isEditing = !!this.localCustomer.idCustomer;
        let result;

        if (isEditing) {
          result = await this.$store.dispatch('customer/editCustomer', {
            id: this.localCustomer.idCustomer,
            customer: { ...this.localCustomer }
          });
        } else {
          result = await this.$store.dispatch('customer/registerCustomer', { ...this.localCustomer });
        }

        if (result.isSuccess) {
          const successMsg = isEditing
            ? 'Cliente actualizado con éxito!'
            : 'Cliente registrado con éxito!';

          this.toast.success(successMsg);
          this.$emit('saved', { ...this.localCustomer });
          this.close();
        }
      } catch (error: any) {
        const isEditing = !!this.localCustomer.idCustomer;
        const customMessage = isEditing
          ? 'Error en actualizar al cliente'
          : 'Error en guardar al cliente';

        handleApiError(error, customMessage);
      } finally {
        this.saving = false;
      }
    },
  },
});
</script>