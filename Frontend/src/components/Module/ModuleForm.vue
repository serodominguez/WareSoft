<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localModule.pK_MODULE ? 'Editar Módulo' : 'Agregar Módulo' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localModule.modulE_NAME"
                  :rules="[rules.required, rules.onlyLetters]" counter="25" :maxlength="25"
                  label="Nombre del Módulo" required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="indigo" dark class="mb-2" elevation="4" @click="saveModule" :disabled="!valid">Guardar</v-btn>
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
import { Module } from '@/interfaces/moduleInterface';

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
    module: {
      type: Object as PropType<Module | null>,
      default: () => ({
        pK_MODULE: null,
        modulE_NAME: ''
      }),
    },
  },
  data() {
    return {
      isOpen: this.modelValue,
      valid: false,
      localModule: { ...this.module } as Module,
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
    module: {
      handler(newModule: Module) {
        this.localModule = { ...newModule };
      },
      deep: true,
    },
  },
  methods: {
    close() {
      this.isOpen = false;
    },
    async saveModule() {
      const form = this.$refs.form as FormRef;
      if (form.validate()) {
        try {
          if (this.localModule.pK_MODULE) {
            await this.$store.dispatch('module/editModule', {
              id: this.localModule.pK_MODULE,
              module: { ...this.localModule }
            });
            this.toast.success('Módulo actualizado con éxito!');
          } else {
            await this.$store.dispatch('module/registerModule', { ...this.localModule });
            this.toast.success('Módulo agregado con éxito!');
          }
          this.$emit('saved', { ...this.localModule });
          this.close();
        } catch (error: any) {
          if (error.response) {
            this.toast.error('Error en generar el módulo.');
          }
        }
      }
    },
  },
});
</script>