<template>
  <v-container fluid class="fill-height">
    <v-row justify="center">
      <v-col cols="12" sm="8" md="6" lg="5" xl="4">
        <v-card>
          <v-toolbar dark color="blue-darken-4">
            <v-toolbar-title>Inicio de sesión</v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-text-field v-model="user" color="primary" label="Usuario" variant="underlined" @keyup="uppercase"
              required></v-text-field>
            <v-text-field v-model="password" color="primary" label="Contraseña" variant="underlined"
              :append-icon="show ? 'visibility' : 'visibility_off'" :type="show ? 'text' : 'password'"
              @click:append="show = !show" @keyup.enter="login()" required></v-text-field>
            <v-alert v-if="errorMessage" type="error">{{ errorMessage }}</v-alert>
          </v-card-text>
          <v-card-actions class="px-3 pb-3">
            <v-spacer></v-spacer>
            <v-btn @click="login" color="primary" elevation="4">Ingresar</v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
<script lang="ts">
import axios, { AxiosError } from 'axios';
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

interface ApiResponse {
  isSuccess: boolean;
  data: string;  // El token está aquí
  message: string;
  errors: any;
}

interface ErrorResponse {
  message: string;
}

export default defineComponent({
  setup() {
    const store = useStore();
    const router = useRouter();
    
    return {
      store,
      router
    }
  },
  data() {
    return {
      show: false,
      user: '',
      password: '',
      errorMessage: ''
    }
  },
  methods: {
    uppercase() {
      this.user = this.user.toUpperCase();
    },
    login() {
      axios.post<ApiResponse>('api/Users/Generate/Token', {
        userName: this.user,
        password: this.password,
      })
        .then((response) => {
          const token = response.data.data;
          
          if (token && response.data.isSuccess) {
            this.store.dispatch("saveToken", token);
            this.router.push({ name: "home" });
          } else {
            this.errorMessage = response.data.message || 'Token no recibido. Inténtalo de nuevo.';
          }
        })
        .catch((error: AxiosError<ErrorResponse>) => {
          if (error.response && error.response.data && error.response.data.message) {
            this.errorMessage = error.response.data.message;
          } else {
            this.errorMessage = 'Error de autenticación. Verifica tus credenciales.';
          }
        });
    }
  }
})
</script>