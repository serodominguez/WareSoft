import axios from 'axios';
import router from '@/router/index';
import { createStore } from 'vuex';
import { jwtDecode } from 'jwt-decode';
import { normalize } from '@/utils/string';
import Brand from '@/store/modules/Brand';
import Category from '@/store/modules/Category';
import Customer from '@/store/modules/Customer';
import GoodsIssue from './modules/GoodsIssue';
import GoodsReceipt from './modules/GoodsReceipt';
import Inventory from './modules/Inventory';
import Module from '@/store/modules/Module';
import Permission from '@/store/modules/Permission';
import Product from './modules/Product';
import Role from '@/store/modules/Role';
import Store from '@/store/modules/Store';
import Supplier from './modules/Supplier';
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
  authInitialized: boolean;
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

// Helper para obtener usuario cacheado de localStorage
const getCachedUser = (): CurrentUser | null => {
  try {
    const cached = localStorage.getItem('user');
    if (cached) {
      return JSON.parse(cached) as CurrentUser;
    }
  } catch (error) {
    console.error('Error parseando usuario cacheado:', error);
  }
  return null;
};

// Helper para limpiar completamente la sesión
const clearSession = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("user");
  delete axios.defaults.headers.common["Authorization"];
};

const store = createStore<RootState>({
  state: {
    token: null,
    currentUser: null,
    authInitialized: false,
  },

  getters: {
    isAuthenticated(state): boolean {
      return !!state.token;
    },

    getCurrentUser(state): CurrentUser | null {
      return state.currentUser;
    },

    authInitialized(state): boolean {
      return state.authInitialized;
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
        clearSession();
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

    SET_AUTH_INITIALIZED(state, initialized: boolean) {
      state.authInitialized = initialized;
    },
  },

  actions: {
    // Inicializa la autenticación al cargar la app
    async initializeAuth({ commit }) {
      try {
        const token = localStorage.getItem("token");

        // Sin token → continuar sin usuario
        if (!token) {
          commit("SET_AUTH_INITIALIZED", true);
          return;
        }

        // Token expirado → limpiar todo
        if (isTokenExpired(token)) {
          commit("SET_TOKEN", null);
          commit("SET_USER", null);
          commit("SET_AUTH_INITIALIZED", true);
          return;
        }

        // Token válido → restaurar sesión desde localStorage
        commit("SET_TOKEN", token);

        const cachedUser = getCachedUser();
        
        if (cachedUser) {
          commit("SET_USER", cachedUser);
        } else {
          const decoded = jwtDecode<JwtPayload>(token);
          const userId = parseInt(decoded.userId, 10);
          const userWithoutPermissions = createUserFromToken(decoded, userId, []);
          commit("SET_USER", userWithoutPermissions);
          console.warn('Usuario restaurado sin permisos desde token');
        }

        commit("SET_AUTH_INITIALIZED", true);
      } catch (error) {
        console.error("Error inicializando auth:", error);
        commit("SET_TOKEN", null);
        commit("SET_USER", null);
        commit("SET_AUTH_INITIALIZED", true);
      }
    },

    // Guarda el token y carga permisos del servidor (solo en LOGIN)
    async saveToken({ commit, dispatch }, token: string) {
      try {
        commit("SET_TOKEN", token);

        const decoded = jwtDecode<JwtPayload>(token);
        const userId = parseInt(decoded.userId, 10);

        // Cargar permisos frescos desde el servidor
        await dispatch("loadUserPermissions", { decoded, userId });
      } catch (error) {
        console.error("Error al guardar token:", error);
        commit("SET_TOKEN", null);
        throw error;
      }
    },

    // Solo se llama en LOGIN o cuando se necesita refrescar manualmente
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
      } catch (error) {
        console.error("Error cargando permisos:", error);

        const user = createUserFromToken(decoded, userId, []);
        commit("SET_USER", user);
      }
    },

    // Cierra sesión y limpia todo
    logout({ commit }) {
      commit("SET_TOKEN", null);
      commit("SET_USER", null);
      router.push({ name: "login" });
    },
  },

  modules: {
    brand: Brand,
    category: Category,
    customer: Customer,
    goodsissue: GoodsIssue,
    goodsreceipt: GoodsReceipt,
    inventory: Inventory,
    module: Module,
    permission: Permission,
    product: Product,
    role: Role,
    store: Store,
    supplier: Supplier,
    user: User,
  },
});

export default store;