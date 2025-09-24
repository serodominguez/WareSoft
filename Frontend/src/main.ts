import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import { vuetify, i18n  } from './plugins/vuetify'
import { loadFonts } from './plugins/webfontloader'
import axios from 'axios'
import Toast from 'vue-toastification';
import 'vue-toastification/dist/index.css';

// Configuración de Axios
axios.defaults.baseURL='https://localhost:7075/'

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
  .mount('#app')
