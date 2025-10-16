<template>
  <v-navigation-drawer v-model="isOpen" app temporary>
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
  name: "NavigationDrawer",
  props: {
    modelValue: {
      type: Boolean,
      default: false
    }
  },
  emits: ['update:modelValue'],
  data() {
    return {
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
    isOpen: {
      get(): boolean {
        return this.modelValue;
      },
      set(value: boolean): void {
        this.$emit('update:modelValue', value);
      }
    }
  }
});
</script>