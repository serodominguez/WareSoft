<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localModule.idModule ? 'Editar Módulo' : 'Agregar Módulo' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localModule.moduleName"
                  :rules="[rules.required, rules.onlyLetters]" counter="25" :maxlength="25" label="Nombre del Módulo"
                  required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="indigo" dark class="mb-2" elevation="4" @click="saveModule" :disabled="!valid"
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
import { Module } from '@/interfaces/moduleInterface';
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
    module: {
      type: Object as PropType<Module | null>,
      default: () => ({
        idModule: null,
        moduleName: ''
      }),
    },
  },
  data() {
    return {
      isOpen: this.modelValue,
      valid: false,
      saving: false,
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
      if (!form.validate()) {
        this.toast.warning('Por favor completa todos los campos requeridos');
        return;
      }
      this.saving = true;
      try {
        const isEditing = !!this.localModule.idModule;
        let result;

        if (isEditing) {
          result = await this.$store.dispatch('module/editModule', {
            id: this.localModule.idModule,
            module: { ...this.localModule }
          });
        } else {
          result = await this.$store.dispatch('module/registerModule', { ...this.localModule });
        }

        if (result.isSuccess) {
          const successMsg = isEditing
            ? 'Módulo actualizado con éxito!'
            : 'Módulo registrado con éxito!';

          this.toast.success(successMsg);
          this.$emit('saved', { ...this.localModule });
          this.close();
        }

      } catch (error: any) {
        const isEditing = !!this.localModule.idModule;
        const customMessage = isEditing
          ? 'Error en actualizar el módulo'
          : 'Error en guardar el módulo';

        handleApiError(error, customMessage);
      } finally {
        this.saving = false;
      }
    },
  },
});
</script>