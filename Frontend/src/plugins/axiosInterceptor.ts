import axios, { AxiosError, AxiosResponse, InternalAxiosRequestConfig } from 'axios';
import store from '@/store';
import router from '@/router';
import { ErrorHandler } from '@/helpers/errorHandler';
import { ErrorType } from '@/interfaces/errorInterface';

// Configuración de Axios con interceptores
export function setupAxiosInterceptors() {

    //Request Interceptor
  axios.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
      // Agregar token JWT a todas las peticiones
      const token = localStorage.getItem('token');
      if (token && config.headers) {
        config.headers.Authorization = `Bearer ${token}`;
      }

      // Agregar timestamp para debugging
/*       if (process.env.NODE_ENV === 'development') {
        console.log(`[API Request] ${config.method?.toUpperCase()} ${config.url}`, {
          params: config.params,
          data: config.data,
        });
      } */

      return config;
    },
    (error: AxiosError) => {
      // Error en configuración de request
      console.error('[Request Error]', error);
      return Promise.reject(error);
    }
  );

  //Response Interceptor
  axios.interceptors.response.use(
    (response: AxiosResponse) => {
      // Log de respuestas exitosas en desarrollo
/*       if (process.env.NODE_ENV === 'development') {
        console.log(`[API Response] ${response.config.url}`, response.data);
      } */

      return response;
    },
    async (error: AxiosError) => {
      const statusCode = error.response?.status;

      // Error por código
      // 401 - Sesión expirada
      if (statusCode === 401) {
        // Evitar loops infinitos si ya estamos en login
        if (router.currentRoute.value.name !== 'login') {
          // Normalizar error con mensaje personalizado
          ErrorHandler.handle(error, {
            showToast: true,
            customMessage: 'Tu sesión ha expirado. Por favor, inicia sesión nuevamente',
          });

          // Limpiar store y redirigir
          await store.dispatch('logout');
          router.push({ name: 'login' });
        }
        return Promise.reject(error);
      }

      // 403 - Sin permisos
      if (statusCode === 403) {
        ErrorHandler.handle(error, {
          showToast: true,
          customMessage: 'No tienes permisos para realizar esta acción',
        });
        return Promise.reject(error);
      }

      // 404 - Recurso no encontrado
      if (statusCode === 404) {
        // Solo mostrar toast, no manejar especialmente
        ErrorHandler.handleSilent(error);
        return Promise.reject(error);
      }

      // 500+ - Errores del servidor
      if (statusCode && statusCode >= 500) {
        ErrorHandler.handle(error, {
          showToast: true,
          customMessage: 'Error del servidor. Intenta nuevamente en unos momentos',
        });
        return Promise.reject(error);
      }

      // Errores de red
      if (error.message === 'Network Error' || !error.response) {
        ErrorHandler.handle(error, {
          showToast: true,
          customMessage: 'Sin conexión a internet. Verifica tu conexión',
        });
        return Promise.reject(error);
      }

      // Timeout
      if (error.code === 'ECONNABORTED') {
        ErrorHandler.handle(error, {
          showToast: true,
          customMessage: 'La solicitud tardó demasiado. Intenta nuevamente',
        });
        return Promise.reject(error);
      }

      // Otros errores
      // No manejar aquí, dejar que el componente decida
      // (Permite manejo personalizado en cada componente)
      return Promise.reject(error);
    }
  );
}

// Configuración de timeout global

export function configureAxiosDefaults() {
  // Base URL
  axios.defaults.baseURL = process.env.VUE_APP_API_URL || 'https://localhost:7145/';

  // Timeout (30 segundos)
  axios.defaults.timeout = 30000;

  // Headers por defecto
  axios.defaults.headers.common['Content-Type'] = 'application/json';
  axios.defaults.headers.common['Accept'] = 'application/json';

  // Configurar para enviar cookies
  axios.defaults.withCredentials = false;
}