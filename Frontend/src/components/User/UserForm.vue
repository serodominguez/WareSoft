<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localUser.pK_USER ? 'Editar Usuario' : 'Agregar Usuario' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localUser.useR_NAME"
                  :rules="[rules.required, rules.onlyLetters]" counter="20" :maxlength="20" @keyup="uppercase" label="Usuario"
                  required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localUser.passworD_HASH" type="password"
                  :rules="[rules.required]" label="Contraseña" clearable required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localUser.names"
                  :rules="[rules.required, rules.onlyLetters]" counter="30" :maxlength="30" @keyup="uppercase" label="Nombres"
                  required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localUser.lasT_NAMES"
                  :rules="[rules.required, rules.onlyLetters]" counter="50" :maxlength="50" @keyup="uppercase" label="Apellidos"
                  required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localUser.identificatioN_NUMBER" counter="8"
                  :maxlength="8" @keyup="uppercase" label="Carnet" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="primary" variant="underlined" v-model="localUser.phonE_NUMBER" counter="8"
                  :rules="[rules.onlyNumbers]" :maxlength="8" label="Teléfono" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-autocomplete color="primary" variant="underlined" :items="roles" v-model="localUser.pK_ROLE"
                  item-title="rolE_NAME" item-value="pK_ROLE" :rules="[rules.required]"
                  no-data-text="No hay datos disponibles" label="Rol" required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-autocomplete color="primary" variant="underlined" :items="stores" v-model="localUser.pK_STORE"
                  item-title="storE_NAME" item-value="pK_STORE" :rules="[rules.required]"
                  no-data-text="No hay datos disponibles" label="Tienda" required />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="indigo" dark class="mb-2" elevation="4" @click="saveUser" :disabled="!valid">Guardar</v-btn>
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
import { User } from '@/models/userModel';

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
        pK_USER: null,
        useR_NAME: '',
        password: '',
        passworD_HASH: '',
        names: '',
        lasT_NAMES: '',
        identificatioN_NUMBER: '',
        phonE_NUMBER: null,
        pK_ROLE: null,
        pK_STORE: null,
        audiT_CREATE_DATE: '',
        statE_USER: '',
        updatE_PASSWORD: false
      }),
    },
  },
  data() {
    return {
      isOpen: this.modelValue,
      valid: false,
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
    stores() {
      const storesFromStore = this.$store.getters['store/stores'];
      return Array.isArray(storesFromStore) ? storesFromStore : []; 
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
        if (newUser.pK_USER) {
          this.oldPassword = newUser.passworD_HASH;
        } else {
          this.oldPassword = '';
        }
      },
      deep: true,
    },
  },
  methods: {
    uppercase() {
      this.localUser.useR_NAME = this.localUser.useR_NAME.toUpperCase();
      this.localUser.names = this.localUser.names.toUpperCase();
      this.localUser.lasT_NAMES = this.localUser.lasT_NAMES.toUpperCase();
    },
    close() {
      this.isOpen = false;
    },
    async saveUser() {
      const form = this.$refs.form as FormRef;
      if (form.validate()) {
        try {
          if (this.localUser.pK_USER) {
            if(this.localUser.passworD_HASH !== this.oldPassword){
              this.localUser.updatE_PASSWORD = true;
              this.localUser.password = this.localUser.passworD_HASH;
            } else {
              this.localUser.updatE_PASSWORD = false;
              this.localUser.password = this.localUser.passworD_HASH;
            }
            await this.$store.dispatch('user/editUser', {
              id: this.localUser.pK_USER,
              user: { ...this.localUser }
            });
            this.toast.success('Usuario actualizado con éxito!');
          } else {
            this.localUser.password = this.localUser.passworD_HASH;
            await this.$store.dispatch('user/registerUser', { ...this.localUser });
            this.toast.success('Usuario agregado con éxito!');
          }
          this.$emit('saved', { ...this.localUser });
          this.close();
        } catch (error: any) {
          if (error.response) {
            this.toast.error('Error en generar el usuario.');
          }
        }
      }
    },
  },
});
</script>