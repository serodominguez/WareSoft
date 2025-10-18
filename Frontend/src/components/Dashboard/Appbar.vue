<template>
  <nav>
    <v-app-bar class="app-bar-custom" dark app>
      <v-app-bar-nav-icon v-if="loggedin" @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
      <v-toolbar-title v-if="loggedin" class="text-uppercase">
        <span class="font-weight-light"></span>
        <span style="font-size: 70%"><strong>Sucursal: {{ $store.state.currentUser.store_name }} </strong></span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      <span v-if="loggedin" style="font-size: 90%; margin-right: 10px;"><strong> Usuario:  {{ $store.state.currentUser .user_name }}</strong></span>
      <v-btn v-if="loggedin" @click="logout" icon="logout"></v-btn>
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
  computed: {
    loggedin() {
      return this.$store.state.currentUser;
    },
    isAdministrator() {
      return (
        this.$store.state.currentUser &&
        this.$store.state.currentUser.role === "ADMINISTRADORES"
      );
    },
    isWarehouse() {
      return (
        this.$store.state.currentUser &&
        this.$store.state.currentUser.role === "ALMACENEROS"
      );
    },
    isOperator() {
      return (
        this.$store.state.currentUser &&
        this.$store.state.currentUser.role === "OPERARIOS"
      );
    }
  },
  created(){
    this.$store.dispatch("auto");
  },
  methods: {
    logout(): void {
      this.$store.dispatch("logout");
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