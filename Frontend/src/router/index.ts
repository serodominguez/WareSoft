import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import store, { RootState } from '@/store';
import { normalize } from '@/helpers/utils';
import HomeView from '../views/HomeView.vue'
import BrandView from '@/views/BrandView.vue'
import CategoryView from '@/views/CategoryView.vue'
import LoginView from '@/views/LoginView.vue';
import ModuleView from '@/views/ModuleView.vue';
import PermissionView from '@/views/PermissionView.vue';
import RoleView from '@/views/RoleView.vue'
import StoreView from '@/views/StoreView.vue'
import UserView from '@/views/UserView.vue'
 
// Define los meta types para TypeScript
declare module 'vue-router' {
  interface RouteMeta {
    free?: boolean;
    requiresAuth?: boolean;
    module?: string; // Solo necesitamos el módulo, no la acción específica
  }
}

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'home',
    component: HomeView,
    meta: {
      requiresAuth: true,
    }
  },

  {
    path: '/about',
    name: 'about',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ '../views/AboutView.vue'),
    meta: {
      requiresAuth: true,
    }
  },
  {
    path: '/brand',
    name: 'brand',
    component: BrandView,
    meta: {
      requiresAuth: true,
      module: 'marcas'
    }
  },
    {
    path: '/category',
    name: 'category',
    component: CategoryView,
    meta: {
      requiresAuth: true,
      module: 'categorias'
    }
  },
  {
    path: '/login',
    name: 'login',
    component: LoginView,
    meta: {
      free: true 
    }
  },
  {
    path: '/module',
    name: 'module',
    component: ModuleView,
    meta: {
      requiresAuth: true,
      module: 'modulos'
    }
  },
  {
    path: '/permission',
    name: 'permission',
    component: PermissionView,
    meta: {
      requiresAuth: true,
      module: 'permisos'
    }
  },
  {
    path: '/role',
    name: 'role',
    component: RoleView,
  meta: {
      requiresAuth: true,
      module: 'roles'
    }
  },
  {
    path: '/store',
    name: 'store',
    component: StoreView,
    meta: {
      requiresAuth: true,
      module: 'tiendas'
    }
  },
  {
    path: '/user',
    name: 'user',
    component: UserView,
    meta: {
      requiresAuth: true,
      module: 'usuarios'
    }
  },
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
});

router.beforeEach((to, from, next) => {
  const state = store.state as RootState;
  const currentUser = state.currentUser;

  // Rutas libres (login, etc)
  if (to.matched.some(record => record.meta.free)) {
    next();
    return;
  }

  // Verificar autenticación
  if (!currentUser) {
    next({ name: 'login' });
    return;
  }

  // Rutas que requieren autenticación pero no permisos específicos (home, about)
  if (to.matched.some(record => record.meta.requiresAuth && !record.meta.module)) {
    next();
    return;
  }

  // Verificar permisos del módulo (solo verifica si tiene ALGÚN permiso en el módulo)
  const routeWithModule = to.matched.find(record => record.meta.module);
  
  if (routeWithModule && routeWithModule.meta.module) {
    const module = routeWithModule.meta.module;
    
    // Normalizar el módulo de la ruta
    const normalizedRouteModule = normalize(module);
    
    // Verificar si el usuario tiene ALGÚN permiso en este módulo
    const hasModuleAccess = currentUser.permissions.some(
      (p: { module: string; action: string }) => 
        normalize(p.module) === normalizedRouteModule
    );
    
    if (hasModuleAccess) {
      next();
    } else {
      // Redirigir a home si no tiene ningún permiso en el módulo
      next({ name: 'home' });
    }
  } else {
    next();
  }
});

export default router
