<template>
  <v-dialog v-model="isOpen" max-width="400px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4" v-if="action === 0">Eliminar Item?</v-card-title>
      <v-card-title class="bg-surface-light pt-4" v-if="action === 1">Activar Item?</v-card-title>
      <v-card-title class="bg-surface-light pt-4" v-if="action === 2">Desactivar Item?</v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        Estás a punto de
        <span v-if="action === 0">eliminar</span>
        <span v-if="action === 1">activar</span>
        <span v-if="action === 2">desactivar</span>
        el ítem: {{ localUser.userName }}.
      </v-card-text>
      <v-card-actions class="d-flex justify-space-between">
        <div class="d-flex">
          <v-btn v-if="action === 0" color="indigo" dark class="mr-2" elevation="4" @click="remove"
            :loading="processing">
            Eliminar
          </v-btn>
          <v-btn v-if="action === 1" color="indigo" dark class="mr-2" elevation="4" @click="enabled"
            :loading="processing">
            Activar
          </v-btn>
          <v-btn v-if="action === 2" color="indigo" dark class="mr-2" elevation="4" @click="disabled"
            :loading="processing">
            Desactivar
          </v-btn>
          <v-btn color="red" elevation="4" @click="close" :disabled="processing">
            Cancelar
          </v-btn>
        </div>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import { useToast } from 'vue-toastification';
import { defineComponent, PropType } from 'vue';
import { User } from '@/interfaces/userInterface';
import { handleApiError } from '@/helpers/errorHandler';

export default defineComponent({
  name: 'UserModal',
  props: {
    modelValue: {
      type: Boolean,
      required: true,
    },
    user: {
      type: Object as PropType<User | null>,
      default: () => ({
        idUser: null,
        userName: ''
      }),
    },
    action: {
      type: Number,
      required: true,
    },
  },
  emits: ['update:modelValue', 'action-completed'],
  data() {
    return {
      isOpen: this.modelValue,
      processing: false,
      toast: useToast(),
      localUser: { ...this.user } as User,
    };
  },
  watch: {
    modelValue(newValue: boolean) {
      this.isOpen = newValue;
    },
    user: {
      handler(newUser: User) {
        this.localUser = { ...newUser };
      },
      deep: true,
    },
  },
  methods: {
    close() {
      this.isOpen = false;
      this.$emit('update:modelValue', false);
    },

    async remove() {
      this.processing = true;
      try {
        const result = await this.$store.dispatch('user/removeUser', this.localUser.idUser);
        if (result.isSuccess) {
          this.toast.success('Usuario eliminado con éxito!');
          this.$emit('action-completed');
          this.close();
        }
      } catch (error: any) {
        handleApiError(error, 'Error al eliminar el usuario');
      } finally {
        this.processing = false;
      }
    },

    async enabled() {
      this.processing = true;
      try {
        const result = await this.$store.dispatch('user/enableUser', this.localUser.idUser);
        if (result.isSuccess) {
          this.toast.success('Usuario habilitado con éxito!');
          this.$emit('action-completed');
          this.close();
        }
      } catch (error: any) {
        handleApiError(error, 'Error al habilitar el usuario');
      } finally {
        this.processing = false;
      }
    },

    async disabled() {
      this.processing = true;
      try {
        const result = await this.$store.dispatch('user/disableUser', this.localUser.idUser);
        if (result.isSuccess) {
          this.toast.success('Usuario deshabilitado con éxito!');
          this.$emit('action-completed');
          this.close();
        }
      } catch (error: any) {
        handleApiError(error, 'Error al deshabilitar el usuario');
      } finally {
        this.processing = false;
      }
    },
  },
});
</script>
