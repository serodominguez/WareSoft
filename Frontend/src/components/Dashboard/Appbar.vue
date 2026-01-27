<template>
  <nav>
    <v-app-bar class="app-bar-custom" dark app>
      <v-app-bar-nav-icon v-if="currentUser" @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
      <v-toolbar-title v-if="currentUser" class="text-uppercase">
        <span class="font-weight-light"></span>
        <span style="font-size: 70%"><strong>Sucursal: {{ $store.state.currentUser.storeName }} </strong></span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      <span v-if="currentUser" style="font-size: 90%; margin-right: 10px;"><strong> Usuario:  {{ $store.state.currentUser.userName}}</strong></span>
      <v-btn v-if="currentUser" @click="logout" icon="logout"></v-btn>
    </v-app-bar>
  </nav>
  <NavigationDrawer v-model="drawer" />
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import NavigationDrawer from './NavigationDrawer.vue';

// Inicialización del store
const store = useStore();

// Estado reactivo
const drawer = ref(false);

// Computed property para el usuario actual
const currentUser = computed(() => store.state.currentUser);

// Método para cerrar sesión
const logout = (): void => {
  store.dispatch("logout");
};
</script>
<style scoped>
.app-bar-custom {
  background-color: rgb(26, 32, 44);
  color: white; 
}
</style>