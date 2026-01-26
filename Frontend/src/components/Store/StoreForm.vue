<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localStore.idStore ? 'Editar Tienda' : 'Agregar Tienda' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localStore.storeName"
                  :rules="[rules.required, rules.onlyLetters]" counter="50" :maxlength="50" label="Tienda" required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localStore.manager"
                  :rules="[rules.required, rules.onlyLetters]" counter="30" :maxlength="30" label="Encargado"
                  required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localStore.address"
                  :rules="[rules.required]" counter="60" :maxlength="60" label="Dirección" required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localStore.phoneNumber" counter="8"
                  :rules="[rules.onlyNumbers]" :maxlength="8" label="Teléfono" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localStore.city" counter="15"
                  :rules="[rules.required, rules.onlyLetters]" :maxlength="15" label="Ciudad" required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localStore.email" counter="50"
                  :rules="[rules.email]" :maxlength="50" label="Correo" />
              </v-col>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-select color="indigo" variant="underlined" :rules="[rules.required]" v-model="localStore.type"
                  :items="types" label="Tipo" required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="green" dark class="mb-2" elevation="4" @click="saveStore" :disabled="!valid"
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
import { Store } from '@/interfaces/storeInterface';
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
    store: {
      type: Object as PropType<Store | null>,
      default: () => ({
        idStore: null,
        storeName: '',
        manager: '',
        address: '',
        phoneNumber: null,
        city: '',
        email: '',
        type: ''
      }),
    },
  },
  data() {
    return {
      isOpen: this.modelValue,
      valid: false,
      saving: false,
      localStore: { ...this.store } as Store,
      toast: useToast(),
      types: ['Casa Matriz', 'Sucursal', 'Almacén'],
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
    store: {
      handler(newStore: Store) {
        this.localStore = { ...newStore };
      },
      deep: true,
    },
  },
  methods: {
    close() {
      this.isOpen = false;
    },
    async saveStore() {
      const form = this.$refs.form as FormRef;
      if (!form.validate()) {
        this.toast.warning('Por favor completa todos los campos requeridos');
        return;
      }
      this.saving = true;
      try {
        const isEditing = !!this.localStore.idStore;
        let result;

        if (isEditing) {
          result = await this.$store.dispatch('store/editStore', {
            id: this.localStore.idStore,
            store: { ...this.localStore }
          });
        } else {
          result = await this.$store.dispatch('store/registerStore', { ...this.localStore });
        }

        if (result.isSuccess) {
          const successMsg = isEditing
            ? 'Tienda actualizada con éxito!'
            : 'Tienda registrada con éxito!';

          this.toast.success(successMsg);
          this.$emit('saved', { ...this.localStore });
          this.close();
        }

      } catch (error: any) {
        const isEditing = !!this.localStore.idStore;
        const customMessage = isEditing
          ? 'Error en actualizar la tienda'
          : 'Error en guardar la tienda';

        handleApiError(error, customMessage);
      } finally {
        this.saving = false;
      }
    },
  },
});
</script>