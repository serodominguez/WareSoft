import { createStore } from 'vuex'
import { jwtDecode } from 'jwt-decode'
import router from '@/router/index'
import axios from 'axios'
import BrandMoule from '@/store/modules/Brand'
import CategoryModule from '@/store/modules/Category'
import RoleModule from '@/store/modules/Role'
import StoreModule from '@/store/modules/Store'
import UserModule from '@/store/modules/User'

interface Permission {
  module: string;
  action: string;
}

interface JwtPayload {
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name': string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string;
  pk_user: string;
  user_name: string;
  role: string;
  store_name: string;
  pk_store: string;
  nbf: number;
  exp: number;
  iss: string;
  aud: string;
}

interface CurrentUser {
  userId: number;
  userName: string;
  role: string;
  storeId: number;
  storeName: string;
  permissions: Permission[];
}

export interface RootState {
  token: string | null;
  currentUser: CurrentUser | null;
}

const store = createStore({
  state: {
    token: null as string | null,
    currentUser: null as CurrentUser | null,
  },

  getters: {
    isAuthenticated(state) {
      return !!state.token;
    },
    getCurrentUser(state) {
      return state.currentUser;
    },
    hasPermission: (state) => (module: string, action: string): boolean => {
      if (!state.currentUser || !state.currentUser.permissions) return false;
      
      return state.currentUser.permissions.some(
        p => p.module.toLowerCase() === module.toLowerCase() && 
             p.action.toLowerCase() === action.toLowerCase()
      );
    },
  },

  mutations: {
    SET_TOKEN(state, token: string | null) {
      state.token = token;
      if (token) {
        localStorage.setItem("token", token);
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      } else {
        localStorage.removeItem("token");
        delete axios.defaults.headers.common['Authorization'];
      }
    },
    SET_USER(state, user: CurrentUser | null) {
      state.currentUser = user;
      if (user) {
        localStorage.setItem("user", JSON.stringify(user));
      } else {
        localStorage.removeItem("user");
      }
    }
  },

  actions: {
    async saveToken({ commit, dispatch }, token: string) {
      commit("SET_TOKEN", token);
      
      // Decodificar el token
      const decoded = jwtDecode<JwtPayload>(token);
      
      // Obtener userId usando los claims personalizados (más simple)
      const userId = parseInt(decoded.pk_user);
      
      // Cargar permisos del usuario
      await dispatch('loadUserPermissions', { decoded, userId });
    },

    async loadUserPermissions({ commit }, { decoded, userId }: { decoded: JwtPayload, userId: number }) {
      try {
        // Llamar al endpoint de permisos
        const response = await axios.get(`/api/Permissions/User/${userId}`);
        
        if (response.data.isSuccess) {
          const user: CurrentUser = {
            userId: userId,
            userName: decoded.user_name, // ← Usar el claim personalizado
            role: decoded.role,          // ← Usar el claim personalizado
            storeId: parseInt(decoded.pk_store),
            storeName: decoded.store_name,
            permissions: response.data.data || [],
          };
          
          commit("SET_USER", user);
        } else {
          console.error('Error loading permissions:', response.data.message);
          // Crear usuario sin permisos
          const user: CurrentUser = {
            userId: userId,
            userName: decoded.user_name,
            role: decoded.role,
            storeId: parseInt(decoded.pk_store),
            storeName: decoded.store_name,
            permissions: [],
          };
          commit("SET_USER", user);
        }
      } catch (error) {
        console.error('Error loading permissions:', error);
        // Crear usuario sin permisos en caso de error
        const user: CurrentUser = {
          userId: userId,
          userName: decoded.user_name,
          role: decoded.role,
          storeId: parseInt(decoded.pk_store),
          storeName: decoded.store_name,
          permissions: [],
        };
        commit("SET_USER", user);
      }
    },

    async auto({ commit, dispatch }) {
      let token = localStorage.getItem("token");
      let userStr = localStorage.getItem("user");
      
      if (token) {
        const decodedToken = jwtDecode<JwtPayload>(token);
        const currentTime = Date.now() / 1000;

        if (decodedToken.exp < currentTime) {
          // Token expirado
          dispatch('logout');
          return;
        } else {
          commit("SET_TOKEN", token);
          
          // Si hay usuario guardado, restaurarlo
          if (userStr) {
            const user = JSON.parse(userStr) as CurrentUser;
            commit("SET_USER", user);
          } else {
            // Si no hay usuario guardado, recargar permisos
            const userId = parseInt(decodedToken.pk_user);
            await dispatch('loadUserPermissions', { decoded: decodedToken, userId });
          }
          
          router.push({ name: "home" });
        }
      } else {
        router.push({ name: "login" });
      }
    },

    logout({ commit }) {
      commit("SET_TOKEN", null);
      commit("SET_USER", null);
      router.push({ name: "login" });
    },
  },

  modules: {
    brand: BrandMoule,
    category: CategoryModule,
    role: RoleModule,
    store: StoreModule,
    user: UserModule
  },
});

export default store;