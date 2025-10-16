import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import BrandView from '@/views/BrandView.vue'
import CategoryView from '@/views/CategoryView.vue'
import RoleView from '@/views/RoleView.vue'
import StoreView from '@/views/StoreView.vue'
import UserView from '@/views/UserView.vue'

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'home',
    component: HomeView
  },
  {
    path: '/about',
    name: 'about',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ '../views/AboutView.vue')
  },
    {
    path: '/brand',
    name: 'brand',
    component: BrandView
  },
  {
    path: '/category',
    name: 'category',
    component: CategoryView
  },
  {
    path: '/role',
    name: 'role',
    component: RoleView
  },
  {
    path: '/store',
    name: 'store',
    component: StoreView
  },
  {
    path: '/user',
    name: 'user',
    component: UserView
  },
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
