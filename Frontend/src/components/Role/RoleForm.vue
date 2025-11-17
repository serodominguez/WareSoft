<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localRole.idRole ? 'Editar Rol' : 'Agregar Rol' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localRole.roleName"
                  :rules="[rules.required, rules.onlyLetters]" counter="20" :maxlength="20"
                  label="Nombre del Rol" required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="indigo" dark class="mb-2" elevation="4" @click="saveRole" :disabled="!valid">Guardar</v-btn>
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
import { Role } from '@/interfaces/roleInterface';

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
    role: {
      type: Object as PropType<Role | null>,
      default: () => ({
        idRole: null,
        roleName: ''
      }),
    },
  },
  data() {
    return {
      isOpen: this.modelValue,
      valid: false,
      localRole: { ...this.role } as Role,
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
    role: {
      handler(newRole: Role) {
        this.localRole = { ...newRole };
      },
      deep: true,
    },
  },
  methods: {
    close() {
      this.isOpen = false;
    },
    async saveRole() {
      const form = this.$refs.form as FormRef;
      if (form.validate()) {
        try {
          const isEditing = !!this.localRole.idRole;
          let result;

          if (isEditing) {
            result = await this.$store.dispatch('role/editRole', {
              id: this.localRole.idRole,
              role: { ...this.localRole }
            });
          } else {
            result = await this.$store.dispatch('role/registerRole', { ...this.localRole });
          }

          if (result.isSuccess) {
            const successMsg = isEditing
              ? 'Rol actualizado con éxito!'
              : 'Rol registrado con éxito!';

            this.toast.success(successMsg);
            this.$emit('saved', { ...this.localRole });
            this.close();
          }

        } catch (error: any) {
          const isEditing = !!this.localRole.idRole;
          let errorMsg = isEditing
            ? 'Error en actualizar el rol'
            : 'Error en guardar el rol';

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