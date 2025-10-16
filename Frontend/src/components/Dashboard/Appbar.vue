<template>
  <nav>
    <v-app-bar class="app-bar-custom"  dark app>
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
  <v-navigation-drawer v-model="drawer" fixed app temporary>
    <v-list>
      <v-list-item variant="plain" :to="{ name: 'about' }">
        <v-list-item prepend-icon="home" title="Home"></v-list-item>
      </v-list-item>
      <v-list-group>
        <template v-slot:activator="{ props }">
          <v-list-item v-bind="props" title="Almacén"></v-list-item>
        </template>
        <v-list-item rounded="xl" class="ma-0 ml-n10" v-for="link in linkStore" :key="link.text" :to="link.route">
          <template v-slot:prepend>
            <v-icon :icon="link.icon" style="font-size: 20px;"></v-icon>
          </template>
          <v-list-item-title v-text="link.text" style="font-size: 15px;"></v-list-item-title>
        </v-list-item>
      </v-list-group>
      <v-list-group>
        <template v-slot:activator="{ props }">
          <v-list-item v-bind="props" title="Accesos"></v-list-item>
        </template>
        <v-list-item rounded="xl" class="ma-0 ml-n10" v-for="link in linkAccess" :key="link.text" :to="link.route">
          <template v-slot:prepend>
            <v-icon :icon="link.icon" style="font-size: 20px;"></v-icon>
          </template>
          <v-list-item-title v-text="link.text" style="font-size: 15px;"></v-list-item-title>
        </v-list-item>
      </v-list-group>
      <v-list-group>
        <template v-slot:activator="{ props }">
          <v-list-item v-bind="props" title="Configuración"></v-list-item>
        </template>
        <v-list-item rounded="xl" class="ma-0 ml-n10" v-for="link in linkConfiguration" :key="link.text" :to="link.route">
          <template v-slot:prepend>
            <v-icon :icon="link.icon" style="font-size: 20px;"></v-icon>
          </template>
          <v-list-item-title v-text="link.text" style="font-size: 15px;"></v-list-item-title>
        </v-list-item>
      </v-list-group>
    </v-list>
  </v-navigation-drawer>
</template>
<script lang="ts">
import { defineComponent } from 'vue';
export default defineComponent({
  name: "Appbar",
  data() {
    return {
      drawer: false,
      linkStore: [
        { icon: 'category', text: 'Categorías', route: '/category' },
        { icon: 'copyright', text: 'Marcas', route: '/brand' },
      ] as Array<{ icon: string; text: string; route: string }>,
      linkAccess: [
        { icon: 'manage_accounts', text: 'Roles', route: '/role' },
        { icon: 'person', text: 'Usuarios', route: '/user' },
      ] as Array<{ icon: string; text: string; route: string }>,
      linkConfiguration: [
        { icon: 'store', text: 'Tiendas', route: '/store' },
      ] as Array<{ icon: string; text: string; route: string }>,
    };
  },
  computed: {
    
  },
  created() {
   
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