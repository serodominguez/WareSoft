<template>
  <nav>
    <v-app-bar class="app-bar-custom" dark app>
      <v-app-bar-nav-icon @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
      <v-toolbar-title class="text-uppercase">
        <span class="font-weight-light"></span>
        <span style="font-size: 70%"><strong>Sucursal: </strong></span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      <span style="font-size: 90%; margin-right: 10px;"><strong> Usuario: </strong></span>
      <v-btn @click="logoff" icon="logout"></v-btn>
    </v-app-bar>
  </nav>
  <NavigationDrawer v-model="drawer" />
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import NavigationDrawer from './NavigationDrawer.vue';

export default defineComponent({
  name: "Appbar",
  components: {
    NavigationDrawer
  },
  data() {
    return {
      drawer: false,
    };
  },
  methods: {
    logoff(): void {
      // Tu lógica de logoff
    },
  },
});
</script>

<style scoped>
.app-bar-custom {
  background-color: rgb(26, 32, 44);
  color: white; 
}
</style>