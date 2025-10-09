<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localStore.pK_STORE ? 'Editar Tienda' : 'Agregar Tienda' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localStore.storE_NAME"
                  :rules="[rules.required]" counter="50" :maxlength="50" @keyup="uppercase" label="Nombre de la Tienda"
                  required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localStore.manager"
                  :rules="[rules.required]" counter="30" :maxlength="30" @keyup="uppercase" label="Encargado"
                  required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localStore.address"
                  :rules="[rules.required]" counter="50" :maxlength="60" @keyup="uppercase" label="Dirección"
                  required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localStore.phonE_NUMBER" counter="8"
                  :maxlength="8" @keyup="uppercase" label="Teléfono" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localStore.city" counter="15"
                  :maxlength="15" @keyup="uppercase" label="Ciudad" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localStore.email" counter="50"
                  :maxlength="50" label="Correo" />
              </v-col>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-select color="primary" variant="underlined" v-model="localStore.type" :items="types" label="Tipo" />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="indigo" dark class="mb-2" elevation="4" @click="saveStore" :disabled="!valid">Guardar</v-btn>
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
import { Store } from '@/models/storeModel';

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
        pK_STORE: null,
        storE_NAME: '',
        manager: '',
        address: '',
        phonE_NUMBER: null,
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
      localStore: { ...this.store } as Store,
      toast: useToast(),
      types: ['SUCURSAL', 'ALMACÉN'],
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
    store: {
      handler(newStore: Store) {
        this.localStore = { ...newStore };
      },
      deep: true,
    },
  },
  methods: {
    uppercase() {
      this.localStore.storE_NAME = this.localStore.storE_NAME.toUpperCase();
      this.localStore.manager = this.localStore.manager.toUpperCase();
      this.localStore.address = this.localStore.address.toUpperCase();
      this.localStore.manager = this.localStore.manager.toUpperCase();
      this.localStore.city = this.localStore.city.toUpperCase();
    },
    close() {
      this.isOpen = false;
    },
    async saveStore() {
      const form = this.$refs.form as FormRef;
      if (form.validate()) {
        try {
          if (this.localStore.pK_STORE) {
            await this.$store.dispatch('store/editStore', {
              id: this.localStore.pK_STORE,
              store: { ...this.localStore }
            });
            this.toast.success('Tienda actualizada con éxito!');
          } else {
            await this.$store.dispatch('store/registerStore', { ...this.localStore });
            this.toast.success('Tienda agregada con éxito!');
          }
          this.$emit('saved', { ...this.localStore });
          this.close();
        } catch (error: any) {
          if (error.response) {
            this.toast.error('Error en generar la Tienda.');
          }
        }
      }
    },
  },
});
</script>