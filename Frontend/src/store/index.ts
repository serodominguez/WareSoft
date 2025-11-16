import axios from 'axios'
import router from '@/router/index'
import { createStore } from 'vuex'
import { jwtDecode } from 'jwt-decode'
import { normalize } from '@/helpers/utils'
import Brand from '@/store/modules/Brand'
import Category from '@/store/modules/Category'
import Module from '@/store/modules/Module'
import Permission from '@/store/modules/Permission'
import Role from '@/store/modules/Role'
import Store from '@/store/modules/Store'
import User from '@/store/modules/User'

interface Permission {
  module: string;
  action: string;
}

interface JwtPayload {
  userId: string;       
  userName: string;     
  role: string;         
  storeName: string; 
  storeId: string;  
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
      
      const normalizedModule = normalize(module);
      const normalizedAction = normalize(action);

      return state.currentUser.permissions.some(
        (p: Permission) =>
          normalize(p.module) === normalizedModule &&
          normalize(p.action) === normalizedAction
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
      const userId = parseInt(decoded.userId);
      
      // Cargar permisos del usuario
      await dispatch('loadUserPermissions', { decoded, userId });
    },

    async loadUserPermissions({ commit }, { decoded, userId }: { decoded: JwtPayload, userId: number }) {
      try {
        // Llamar al endpoint de permisos
        const response = await axios.get(`/api/Permission/User`);
        
        if (response.data.isSuccess) {
          const user: CurrentUser = {
            userId: userId,
            userName: decoded.userName,
            role: decoded.role,          
            storeId: parseInt(decoded.storeId),
            storeName: decoded.storeName,
            permissions: response.data.data || [],
          };
          
          commit("SET_USER", user);
        } else {
          console.error('Error loading permissions:', response.data.message);
          // Crear usuario sin permisos
          const user: CurrentUser = {
            userId: userId,
            userName: decoded.userName,
            role: decoded.role,
            storeId: parseInt(decoded.storeId),
            storeName: decoded.storeName,
            permissions: [],
          };
          commit("SET_USER", user);
        }
      } catch (error) {
        console.error('Error loading permissions:', error);
        // Crear usuario sin permisos en caso de error
        const user: CurrentUser = {
          userId: userId,
          userName: decoded.userName,
          role: decoded.role,
          storeId: parseInt(decoded.storeId),
          storeName: decoded.storeName,
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
            const userId = parseInt(decodedToken.userId);
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
    brand: Brand,
    category: Category,
    role: Role,
    module: Module,
    permission: Permission,
    store: Store,
    user: User
  },
});

export default store;