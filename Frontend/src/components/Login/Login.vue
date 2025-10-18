<template>
  <v-container fluid class="fill-height">
    <v-row justify="center">
      <v-col cols="12" sm="8" md="6" lg="5" xl="4">
        <v-card>
          <v-toolbar class="tool-bar-custom" dark>
            <v-toolbar-title>Inicio de sesión</v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-text-field v-model="user" color="indigo" label="Usuario" variant="underlined" @keyup="uppercase"
              required></v-text-field>
            <v-text-field v-model="password" color="indigo" label="Contraseña" variant="underlined"
              :append-icon="show ? 'visibility' : 'visibility_off'" :type="show ? 'text' : 'password'"
              @click:append="show = !show" @keyup.enter="login()" required></v-text-field>
            <v-alert v-if="errorMessage" type="error">{{ errorMessage }}</v-alert>
          </v-card-text>
          <v-card-actions class="px-3 pb-3">
            <v-spacer></v-spacer>
            <v-btn @click="login" color="indigo" elevation="4">Ingresar</v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
<script setup lang="ts">
import axios, { AxiosError } from 'axios';
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useStore } from 'vuex';

interface ApiResponse {
  isSuccess: boolean;
  data: string;
  message: string;
  errors: any;
}

interface ErrorResponse {
  message: string;
}

const router = useRouter();
const store = useStore();

const show = ref(false);
const user = ref('');
const password = ref('');
const errorMessage = ref('');

const uppercase = () => {
  user.value = user.value.toUpperCase();
};

const validateFields = (): boolean => {
  let isValid = true;
  
  errorMessage.value = '';
  
  if (!user.value.trim()) {
    errorMessage.value = 'El usuario es requerido';
    isValid = false;
  }
  
  if (!password.value.trim()) {
    errorMessage.value = 'La contraseña es requerida';
    isValid = false;
  }
  
  return isValid;
};

const login = () => {

    if (!validateFields()) {
    return;
  }
  
  axios.post<ApiResponse>('api/Users/Generate/Token', {
    userName: user.value,
    password: password.value,
  })
    .then((response) => {
      const token = response.data.data;
      
      if (token && response.data.isSuccess) {
        store.dispatch("saveToken", token);
        router.push({ name: "home" });
      } else {
        errorMessage.value = response.data.message || 'Token no recibido. Inténtalo de nuevo.';
      }
    })
    .catch((error: AxiosError<ErrorResponse>) => {
      if (error.response && error.response.data && error.response.data.message) {
        errorMessage.value = error.response.data.message;
      } else {
        errorMessage.value = 'Error de autenticación. Verifica tus credenciales.';
      }
    });
};
</script>'
<style scoped>
.tool-bar-custom {
  background-color: rgb(26, 32, 44);
  color: white; 
}
</style>