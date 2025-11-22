import axios from 'axios';
import router from '@/router/index';
import { createStore } from 'vuex';
import { jwtDecode } from 'jwt-decode';
import { normalize } from '@/helpers/utils';
import Brand from '@/store/modules/Brand';
import Category from '@/store/modules/Category';
import Module from '@/store/modules/Module';
import Permission from '@/store/modules/Permission';
import Product from './modules/Product';
import Role from '@/store/modules/Role';
import Store from '@/store/modules/Store';
import User from '@/store/modules/User';

interface UserPermission {
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
  permissions: UserPermission[];
}

export interface RootState {
  token: string | null;
  currentUser: CurrentUser | null;
}

const createUserFromToken = (
  decoded: JwtPayload,
  userId: number,
  permissions: UserPermission[] = []
): CurrentUser => ({
  userId,
  userName: decoded.userName,
  role: decoded.role,
  storeId: parseInt(decoded.storeId, 10),
  storeName: decoded.storeName,
  permissions,
});

const isTokenExpired = (token: string): boolean => {
  try {
    const decoded = jwtDecode<JwtPayload>(token);
    return decoded.exp < Date.now() / 1000;
  } catch {
    return true;
  }
};

const store = createStore<RootState>({
  state: {
    token: null,
    currentUser: null,
  },

  getters: {
    isAuthenticated(state): boolean {
      return !!state.token;
    },

    getCurrentUser(state): CurrentUser | null {
      return state.currentUser;
    },

    hasPermission:
      (state) =>
      (module: string, action: string): boolean => {
        if (!state.currentUser?.permissions) return false;

        const normalizedModule = normalize(module);
        const normalizedAction = normalize(action);

        return state.currentUser.permissions.some(
          (p) =>
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
        axios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
      } else {
        localStorage.removeItem("token");
        delete axios.defaults.headers.common["Authorization"];
      }
    },

    SET_USER(state, user: CurrentUser | null) {
      state.currentUser = user;

      if (user) {
        localStorage.setItem("user", JSON.stringify(user));
      } else {
        localStorage.removeItem("user");
      }
    },
  },

  actions: {
    // Inicialización bloqueante
    async initializeAuth({ commit, dispatch }) {
      const token = localStorage.getItem("token");

      if (!token) {
        return; // No hay token, continuar sin usuario
      }

      if (isTokenExpired(token)) {
        // Token expirado, limpiar
        localStorage.removeItem("token");
        localStorage.removeItem("user");
        return;
      }

      try {
        // Restaurar token
        commit("SET_TOKEN", token);

        const decoded = jwtDecode<JwtPayload>(token);
        const userId = parseInt(decoded.userId, 10);

        // Cargar permisos de forma síncrona
        await dispatch("loadUserPermissions", { decoded, userId });
      } catch (error) {
        console.error("Error inicializando auth:", error);
        // Si falla, limpiar todo
        commit("SET_TOKEN", null);
        commit("SET_USER", null);
        localStorage.removeItem("token");
        localStorage.removeItem("user");
      }
    },

    async saveToken({ commit, dispatch }, token: string) {
      try {
        commit("SET_TOKEN", token);

        const decoded = jwtDecode<JwtPayload>(token);
        const userId = parseInt(decoded.userId, 10);

        await dispatch("loadUserPermissions", { decoded, userId });
      } catch (error) {
        console.error("Error al guardar token:", error);
        commit("SET_TOKEN", null);
        throw error;
      }
    },

    async loadUserPermissions(
      { commit },
      { decoded, userId }: { decoded: JwtPayload; userId: number }
    ) {
      try {
        const response = await axios.get("/api/Permission/User");

        const permissions = response.data.isSuccess
          ? response.data.data || []
          : [];

        const user = createUserFromToken(decoded, userId, permissions);
        commit("SET_USER", user);

        if (!response.data.isSuccess) {
          console.warn("Permisos no disponibles:", response.data.message);
        }
      } catch (error) {
        console.error("Error cargando permisos:", error);

        // Usuario sin permisos en caso de error
        const user = createUserFromToken(decoded, userId);
        commit("SET_USER", user);
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
    product: Product,
    store: Store,
    user: User,
  },
});

export default store;