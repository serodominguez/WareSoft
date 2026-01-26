<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localUser.idUser ? 'Editar Usuario' : 'Agregar Usuario' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localUser.userName"
                  :rules="[rules.required, rules.onlyLetters]" counter="20" :maxlength="20" @keyup="uppercase"
                  label="Usuario" required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localUser.passwordHash" type="password"
                  :rules="[rules.required]" label="Contraseña" clearable required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localUser.names"
                  :rules="[rules.required, rules.onlyLetters]" counter="30" :maxlength="30" label="Nombres" required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localUser.lastNames"
                  :rules="[rules.required, rules.onlyLetters]" counter="50" :maxlength="50" label="Apellidos"
                  required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localUser.identificationNumber" counter="8"
                  :maxlength="8" label="Carnet" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localUser.phoneNumber" counter="8"
                  :rules="[rules.onlyNumbers]" :maxlength="8" label="Teléfono" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-autocomplete color="indigo" variant="underlined" :items="roles" v-model="localUser.idRole"
                  item-title="roleName" item-value="idRole" :rules="[rules.required]"
                  no-data-text="No hay datos disponibles" label="Rol" required :loading="loadingRoles" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-autocomplete color="indigo" variant="underlined" :items="stores" v-model="localUser.idStore"
                  item-title="storeName" item-value="idStore" :rules="[rules.required]"
                  no-data-text="No hay datos disponibles" label="Tienda" required :loading="loadingStores" />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="green" dark class="mb-2" elevation="4" @click="saveUser" :disabled="!valid"
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
import { User } from '@/interfaces/userInterface';
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
    user: {
      type: Object as PropType<User | null>,
      default: () => ({
        idUser: null,
        userName: '',
        password: '',
        passwordHash: '',
        names: '',
        lastNames: '',
        identificationNumber: '',
        phoneNumber: null,
        idRole: null,
        idStore: null,
        updatePassword: false
      }),
    },
  },
  data() {
    return {
      isOpen: this.modelValue,
      valid: false,
      saving: false,
      localUser: { ...this.user } as User,
      oldPassword: '',
      toast: useToast(),
      rules: {
        required: (value: string) => !!value || 'Este campo es requerido.',
        onlyLetters: (value: string) => !value || /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(value) || 'Solo se permiten letras.',
        onlyNumbers: (value: string) => !value || /^[0-9]+$/.test(value) || 'Solo se permiten números.',
      },
    };
  },
  computed: {
    roles() {
      const rolesFromStore = this.$store.getters['role/roles'];
      return Array.isArray(rolesFromStore) ? rolesFromStore : [];
    },
    loadingRoles(): boolean {
      return this.$store.getters['role/loading'];
    },
    stores() {
      const storesFromStore = this.$store.getters['store/stores'];
      return Array.isArray(storesFromStore) ? storesFromStore : [];
    },
    loadingStores(): boolean {
      return this.$store.getters['store/loading'];
    },
  },
  watch: {
    modelValue(newValue: boolean) {
      this.isOpen = newValue;
      if (newValue) {
        this.$store.dispatch('role/selectRole');
        this.$store.dispatch('store/selectStore');
      }
    },
    isOpen(newValue: boolean) {
      this.$emit('update:modelValue', newValue);
    },
    user: {
      handler(newUser: User) {
        this.localUser = { ...newUser };
        if (newUser.idUser) {
          this.oldPassword = newUser.passwordHash;
        } else {
          this.oldPassword = '';
        }
      },
      deep: true,
    },
  },
  methods: {
    uppercase() {
      this.localUser.userName = this.localUser.userName.toUpperCase();
    },
    close() {
      this.isOpen = false;
    },
    async saveUser() {
      const form = this.$refs.form as FormRef;
      if (!form.validate()) {
        this.toast.warning('Por favor completa todos los campos requeridos');
        return;
      }
      this.saving = true;
      try {
        const isEditing = !!this.localUser.idUser;
        let result;

        if (isEditing) {
          if (this.localUser.passwordHash !== this.oldPassword) {
            this.localUser.updatePassword = true;
            this.localUser.password = this.localUser.passwordHash;
          } else {
            this.localUser.updatePassword = false;
            this.localUser.password = this.localUser.passwordHash;
          }
          result = await this.$store.dispatch('user/editUser', {
            id: this.localUser.idUser,
            user: { ...this.localUser }
          });
        } else {
          this.localUser.password = this.localUser.passwordHash;
          result = await this.$store.dispatch('user/registerUser', { ...this.localUser });
        }

        if (result.isSuccess) {
          const successMsg = isEditing
            ? 'Usuario actualizado con éxito!'
            : 'Usuario registrado con éxito!';

          this.toast.success(successMsg);
          this.$emit('saved', { ...this.localUser });
          this.close();
        }

      } catch (error: any) {
        const isEditing = !!this.localUser.idUser;
        const customMessage = isEditing
          ? 'Error en actualizar el usuario'
          : 'Error en guardar el usuario';
        handleApiError(error, customMessage);
      } finally {
        this.saving = false;
      }
    },
  },
});
</script>