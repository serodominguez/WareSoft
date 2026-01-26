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
          v-for="link in visibleDirectStoreLinks" 
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

        <v-list-group v-if="hasMovementsPermissions" sub-group>
          <template v-slot:activator="{ props }">
            <v-list-item v-bind="props" title="Movimientos" rounded="xl"  class="ml-n10"></v-list-item>
          </template>
          <v-list-item 
            v-for="link in visibleMovementLinks" 
            :key="link.text" 
            :to="link.route" 
            rounded="xl"
            class="ma-0 ml-n16"
          >
            <template v-slot:prepend>
              <v-icon :icon="link.icon" style="font-size: 20px; margin-right: -20px;"></v-icon>
            </template>
            <v-list-item-title v-text="link.text" style="font-size: 15px;"></v-list-item-title>
          </v-list-item>
        </v-list-group>
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
import { normalize } from '@/utils/string';

// Interface que define la estructura de un enlace del menú de navegación
interface Link {
  icon: string;      // Icono a mostrar
  text: string;      // Texto del enlace
  route: string;     // Ruta de navegación
  module: string;    // Módulo asociado para validar permisos
}

export default defineComponent({
  name: "NavigationDrawer",
  props: {
    // Permite el uso de v-model desde el componente padre
    modelValue: {
      type: Boolean,
      default: false
    }
  },
  // Emite eventos para sincronizar el estado con el componente padre
  emits: ['update:modelValue'],
  data() {
    return {
      // Enlaces directos del menú de Almacén (sin Entradas/Salidas)
      linkDirectStore: [
        { icon: 'category', text: 'Categorías', route: '/categorias', module: 'categorias' },
        { icon: 'contact_page', text: 'Clientes', route: '/clientes', module: 'clientes' },
        { icon: 'warehouse', text: 'Inventario', route: '/inventario', module: 'inventario' },
        { icon: 'copyright', text: 'Marcas', route: '/marcas', module: 'marcas' },
        { icon: 'inventory_2', text: 'Productos', route: '/productos', module: 'productos' },
        { icon: 'contacts', text: 'Proveedores', route: '/proveedores', module: 'proveedores' },
        { icon: 'store', text: 'Tiendas', route: '/tiendas', module: 'tiendas' },
      ] as Link[],
      // Enlaces del subgrupo Movimientos
      linkMovements: [
        { icon: 'add_shopping_cart', text: 'Entradas', route: '/entradas', module: 'entrada de productos' },
        { icon: 'remove_shopping_cart', text: 'Salidas', route: '/salidas', module: 'salida de productos' },
      ] as Link[],
      // Enlaces del menú de Accesos (gestión de usuarios y permisos)
      linkAccess: [
        { icon: 'manage_accounts', text: 'Permisos', route: '/permisos', module: 'permisos' },
        { icon: 'supervisor_account', text: 'Roles', route: '/roles', module: 'roles' },
        { icon: 'person', text: 'Usuarios', route: '/usuarios', module: 'usuarios' },
      ] as Link[],
      // Enlaces del menú de Configuración
      linkConfiguration: [
        { icon: 'app_registration', text: 'Módulos', route: '/modulos', module: 'modulos' },
      ] as Link[],
    };
  },
  computed: {
    // Permite la sincronización bidireccional del estado del drawer
    isOpen: {
      get(): boolean {
        return this.modelValue;
      },
      set(value: boolean): void {
        this.$emit('update:modelValue', value);
      }
    },
    // Filtra los enlaces directos de Almacén según los permisos del usuario
    visibleDirectStoreLinks(): Link[] {
      return this.linkDirectStore.filter(link => this.hasModuleAccess(link.module));
    },
    // Filtra los enlaces de Movimientos según los permisos del usuario
    visibleMovementLinks(): Link[] {
      return this.linkMovements.filter(link => this.hasModuleAccess(link.module));
    },
    // Filtra los enlaces de Accesos según los permisos del usuario
    visibleAccessLinks(): Link[] {
      return this.linkAccess.filter(link => this.hasModuleAccess(link.module));
    },
    // Filtra los enlaces de Configuración según los permisos del usuario
    visibleConfigurationLinks(): Link[] {
      return this.linkConfiguration.filter(link => this.hasModuleAccess(link.module));
    },
    // Verifica si se debe mostrar la sección de Almacén
    hasStorePermissions(): boolean {
      return this.visibleDirectStoreLinks.length > 0 || this.visibleMovementLinks.length > 0;
    },
    // Verifica si se debe mostrar el subgrupo de Movimientos
    hasMovementsPermissions(): boolean {
      return this.visibleMovementLinks.length > 0;
    },
    // Verifica si se debe mostrar la sección de Accesos
    hasAccessPermissions(): boolean {
      return this.visibleAccessLinks.length > 0;
    },
    // Verifica si se debe mostrar la sección de Configuración
    hasConfigurationPermissions(): boolean {
      return this.visibleConfigurationLinks.length > 0;
    }
  },
  methods: {
    // Verifica si el usuario tiene acceso a un módulo específico
    hasModuleAccess(module: string): boolean {
      // Módulos marcados como 'exclude' son siempre visibles (sin restricción)
      if (module === 'exclude') {
        return true;
      }
      // Obtiene el usuario actual del store
      const currentUser = this.$store.state.currentUser;
      // Si no hay usuario o no tiene permisos, deniega el acceso
      if (!currentUser || !currentUser.permissions) {
        return false;
      }
      // Normaliza el nombre del módulo para comparación consistente
      const normalizedModule = normalize(module);
      // Verifica si existe algún permiso del usuario que coincida con el módulo
      return currentUser.permissions.some(
        (p: { module: string; action: string }) => 
          normalize(p.module) === normalizedModule
      );
    }
  }
});
</script>