  // Core Vue
import { createApp } from 'vue'
import App from './App.vue'

// Plugins y Rutas
import router from './router'
import store from './store'
import { vuetify, i18n  } from './plugins/vuetify'
import { loadFonts } from './plugins/webfontloader'
import permissionsPlugin from './plugins/permissions'

// Externals
import axios from 'axios'
import Toast from 'vue-toastification';
import 'vue-toastification/dist/index.css';

// Configuración de Axios
axios.defaults.baseURL='https://localhost:7228/'

// Interceptor para agregar token a todas las peticiones
const token = localStorage.getItem('token')
if (token) {
  axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
}

// Interceptor para manejar errores de autorización
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      // 401 Unauthorized - Token inválido o expirado
      if (error.response.status === 401) {
        store.dispatch('logout')
        router.push({ name: 'login' })
      }
      // 403 Forbidden - Sin permisos
      if (error.response.status === 403) {
        const toast = (createApp(App).config.globalProperties as any).$toast
        if (toast) {
          toast.error(error.response.data.message || 'No tienes permisos para esta acción')
        }
      }
    }
    return Promise.reject(error)
  }
)

// Cargar fuentes
loadFonts();

// Opciones para Toast
const options = {
  position: 'top-center',
  timeout: 1500,
  closeOnClick: true,
  pauseOnFocusLoss: true,
  pauseOnHover: true,
  draggable: true,
  draggablePercent: 0.6,
  showCloseButtonOnHover: false,
  closeButton: 'button',
  icon: true,
  rtl: false,
};

// Crear la aplicación Vue
createApp(App)
  .use(router)
  .use(store)
  .use(vuetify)
  .use(i18n)
  .use(Toast, options)
  .use(permissionsPlugin)
  .mount('#app')
