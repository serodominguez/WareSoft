import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import store, { RootState } from '@/store';
import HomeView from '../views/HomeView.vue'
import BrandView from '@/views/BrandView.vue'
import CategoryView from '@/views/CategoryView.vue'
import LoginView from '@/views/LoginView.vue';
import RoleView from '@/views/RoleView.vue'
import StoreView from '@/views/StoreView.vue'
import UserView from '@/views/UserView.vue'
 
const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'home',
    component: HomeView,
    meta: {
      administrator: true,
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
      administrator: true,
    }
  },
    {
    path: '/brand',
    name: 'brand',
    component: BrandView,
    meta: {
      administrator: true,
    }
  },
    {
    path: '/category',
    name: 'category',
    component: CategoryView,
    meta: {
      administrator: true,
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
    path: '/role',
    name: 'role',
    component: RoleView,
  meta: {
      administrator: true,
    }
  },
  {
    path: '/store',
    name: 'store',
    component: StoreView,
    meta: {
      administrator: true,
    }
  },
  {
    path: '/user',
    name: 'user',
    component: UserView,
    meta: {
      administrator: true,
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

  if(to.matched.some(record => record.meta.free)){
    next()
  } else if (currentUser && currentUser.role == 'ADMINISTRADORES'){
    if (to.matched.some(record => record.meta.administrator)){
        next()
    }
  } else if (currentUser && currentUser.role == 'ALMACENEROS'){
    if (to.matched.some(record => record.meta.warehouse)){
        next()
    }
  } else if (currentUser && currentUser.role == 'OPERARIOS'){
    if (to.matched.some(record => record.meta.operator)){
        next()
    }
  } else {
    next({
      name: 'login'
    })
  }
})

export default router
