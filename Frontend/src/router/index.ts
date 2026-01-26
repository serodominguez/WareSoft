import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import store, { RootState } from '@/store';
import { normalize } from '@/utils/string';
import HomeView from '../views/HomeView.vue'
import BrandView from '@/views/BrandView.vue'
import CategoryView from '@/views/CategoryView.vue'
import CustomerView from '@/views/CustomerView.vue';
import GoodsIssueView from '@/views/GoodsIssueView.vue';
import GoodsReceiptView from '@/views/GoodsReceiptView.vue';
import InventoryView from '@/views/InventoryView.vue';
import LoginView from '@/views/LoginView.vue';
import ModuleView from '@/views/ModuleView.vue';
import PermissionView from '@/views/PermissionView.vue';
import ProductView from '@/views/ProductView.vue';
import RoleView from '@/views/RoleView.vue'
import StoreView from '@/views/StoreView.vue'
import SupplierView from '@/views/SupplierView.vue';
import UserView from '@/views/UserView.vue'

 
declare module 'vue-router' {
  interface RouteMeta {
    free?: boolean;
    requiresAuth?: boolean;
    module?: string;
  }
}

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "home",
    component: HomeView,
    meta: {
      requiresAuth: true,
    },
  },
  {
    path: "/about",
    name: "about",
    component: () => import("../views/AboutView.vue"),
    meta: {
      requiresAuth: true,
    },
  },
  {
    path: "/marcas",
    name: "brand",
    component: BrandView,
    meta: {
      requiresAuth: true,
      module: "marcas",
    },
  },
  {
    path: "/categorias",
    name: "category",
    component: CategoryView,
    meta: {
      requiresAuth: true,
      module: "categorias",
    },
  },
  {
    path: "/clientes",
    name: "customer",
    component: CustomerView,
    meta: {
      requiresAuth: true,
      module: "clientes",
    },
  },
  {
    path: "/salidas",
    name: "goodsissue",
    component: GoodsIssueView,
    meta: {
      requiresAuth: true,
      module: "salida de productos",
    },
  },
  {
    path: "/entradas",
    name: "goodsreceipt",
    component: GoodsReceiptView,
    meta: {
      requiresAuth: true,
      module: "entrada de productos",
    },
  },
  {
    path: "/inventario",
    name: "inventory",
    component: InventoryView,
    meta: {
      requiresAuth: true,
      module: "inventario",
    },
  },
  {
    path: "/inicio",
    name: "login",
    component: LoginView,
    meta: {
      free: true,
    },
  },
  {
    path: "/modulos",
    name: "module",
    component: ModuleView,
    meta: {
      requiresAuth: true,
      module: "modulos",
    },
  },
  {
    path: "/permisos",
    name: "permission",
    component: PermissionView,
    meta: {
      requiresAuth: true,
      module: "permisos",
    },
  },
  {
    path: "/productos",
    name: "product",
    component: ProductView,
    meta: {
      requiresAuth: true,
      module: "productos",
    },
  },
  {
    path: "/roles",
    name: "role",
    component: RoleView,
    meta: {
      requiresAuth: true,
      module: "roles",
    },
  },
  {
    path: "/tiendas",
    name: "store",
    component: StoreView,
    meta: {
      requiresAuth: true,
      module: "tiendas",
    },
  },
  {
    path: "/proveedores",
    name: "supplier",
    component: SupplierView,
    meta: {
      requiresAuth: true,
      module: "proveedores",
    },
  },
  {
    path: "/usuarios",
    name: "user",
    component: UserView,
    meta: {
      requiresAuth: true,
      module: "usuarios",
    },
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
});

router.beforeEach((to, from, next) => {

  const state = store.state as RootState
  const currentUser = state.currentUser

  // Rutas libres
  if (to.matched.some(record => record.meta.free)) {
    if (currentUser && to.name === 'login') {
      next({ name: 'home' })
      return
    }
    next()
    return
  }

  // Verificar autenticación
  if (!currentUser) {
    next({ name: 'login' })
    return
  }

  // Rutas que requieren autenticación pero no permisos específicos
  if (to.matched.some(record => record.meta.requiresAuth && !record.meta.module)) {
    next()
    return
  }

  // Verificar permisos del módulo
  const routeWithModule = to.matched.find(record => record.meta.module)
  
  if (routeWithModule && routeWithModule.meta.module) {
    const module = routeWithModule.meta.module
    const normalizedRouteModule = normalize(module)
    
    const hasModuleAccess = currentUser.permissions.some(
      (p: { module: string; action: string }) => 
        normalize(p.module) === normalizedRouteModule
    )
    
    if (hasModuleAccess) {
      next()
    } else {
      next({ name: 'home' })
    }
  } else {
    next()
  }
});

export default router