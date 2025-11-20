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
import { jwtDecode } from 'jwt-decode'
import Toast from 'vue-toastification';
import 'vue-toastification/dist/index.css';

// Configuración de Axios
axios.defaults.baseURL='https://localhost:7145/'

// Interface para el token decodificado
interface DecodedToken {
  exp: number;
  [key: string]: any;
}

// Función para verificar si el token está expirado
const isTokenExpired = (token: string): boolean => {
  try {
    const decoded = jwtDecode<DecodedToken>(token)
    const currentTime = Date.now() / 1000
    return decoded.exp < currentTime
  } catch {
    return true
  }
}

// Interceptor para agregar token y validar expiración antes de la petición
axios.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    
    if (token) {
      // Verificar si el token está expirado antes de hacer la petición
      if (isTokenExpired(token)) {
        // Limpiar token expirado
        localStorage.removeItem('token')
        store.dispatch('logout')
        router.push({ name: 'login' })
        return Promise.reject(new Error('Token expirado'))
      }
      
      // Si el token es válido, agregarlo a la petición
      config.headers.Authorization = `Bearer ${token}`
    }
    
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Interceptor para manejar errores de autorización del servidor
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      // 401 Unauthorized - Token inválido o expirado según el servidor
      if (error.response.status === 401) {
        localStorage.removeItem('token')
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
