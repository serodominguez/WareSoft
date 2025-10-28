<template>
  <v-navigation-drawer v-model="isOpen" app temporary>
    <v-list>
      <v-list-item variant="plain" :to="{ name: 'about' }">
        <v-list-item prepend-icon="home" title="Home"></v-list-item>
      </v-list-item>

      <v-list-group v-if="hasStorePermissions">
        <template v-slot:activator="{ props }">
          <v-list-item v-bind="props" title="Almacén"></v-list-item>
        </template>
        <v-list-item 
          v-for="link in visibleStoreLinks" 
          :key="link.text" 
          :to="link.route"
          rounded="xl" 
          class="ma-0 ml-n10"
        >
          <template v-slot:prepend>
            <v-icon :icon="link.icon" style="font-size: 20px;"></v-icon>
          </template>
          <v-list-item-title v-text="link.text" style="font-size: 15px;"></v-list-item-title>
        </v-list-item>
      </v-list-group>

      <v-list-group v-if="hasAccessPermissions">
        <template v-slot:activator="{ props }">
          <v-list-item v-bind="props" title="Accesos"></v-list-item>
        </template>
        <v-list-item 
          v-for="link in visibleAccessLinks" 
          :key="link.text" 
          :to="link.route"
          rounded="xl" 
          class="ma-0 ml-n10"
        >
          <template v-slot:prepend>
            <v-icon :icon="link.icon" style="font-size: 20px;"></v-icon>
          </template>
          <v-list-item-title v-text="link.text" style="font-size: 15px;"></v-list-item-title>
        </v-list-item>
      </v-list-group>

      <v-list-group v-if="hasConfigurationPermissions">
        <template v-slot:activator="{ props }">
          <v-list-item v-bind="props" title="Configuración"></v-list-item>
        </template>
        <v-list-item 
          v-for="link in visibleConfigurationLinks" 
          :key="link.text" 
          :to="link.route"
          rounded="xl" 
          class="ma-0 ml-n10"
        >
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
import { normalize } from '@/helpers/utils';

interface Link {
  icon: string;
  text: string;
  route: string;
  module: string;
}

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
        { icon: 'category', text: 'Categorías', route: '/category', module: 'categorias' },
        { icon: 'copyright', text: 'Marcas', route: '/brand', module: 'marcas' },
      ] as Link[],
      linkAccess: [
        { icon: 'manage_accounts', text: 'Roles', route: '/role', module: 'roles' },
        { icon: 'person', text: 'Usuarios', route: '/user', module: 'usuarios' },
      ] as Link[],
      linkConfiguration: [
        { icon: 'store', text: 'Tiendas', route: '/store', module: 'tiendas' },
      ] as Link[],
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
    },
    
    visibleStoreLinks(): Link[] {
      return this.linkStore.filter(link => this.hasModuleAccess(link.module));
    },
    
    visibleAccessLinks(): Link[] {
      return this.linkAccess.filter(link => this.hasModuleAccess(link.module));
    },
    
    visibleConfigurationLinks(): Link[] {
      return this.linkConfiguration.filter(link => this.hasModuleAccess(link.module));
    },
    
    hasStorePermissions(): boolean {
      return this.visibleStoreLinks.length > 0;
    },
    
    hasAccessPermissions(): boolean {
      return this.visibleAccessLinks.length > 0;
    },
    
    hasConfigurationPermissions(): boolean {
      return this.visibleConfigurationLinks.length > 0;
    }
  },
  methods: {
    hasModuleAccess(module: string): boolean {
      const currentUser = this.$store.state.currentUser;
      
      if (!currentUser || !currentUser.permissions) {
        return false;
      }
      
      const normalizedModule = normalize(module);
      
      return currentUser.permissions.some(
        (p: { module: string; action: string }) => 
          normalize(p.module) === normalizedModule
      );
    }
  }
});
</script>