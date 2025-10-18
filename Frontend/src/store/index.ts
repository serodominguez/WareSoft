import { createStore } from 'vuex'
import { jwtDecode } from 'jwt-decode'
import router from '@/router/index'
import BrandMoule from '@/store/modules/Brand'
import CategoryModule from '@/store/modules/Category'
import RoleModule from '@/store/modules/Role'
import StoreModule from '@/store/modules/Store'
import UserModule from '@/store/modules/User'

interface JwtPayload {
  exp: number;
  role: string;
}

export interface RootState {
  token: string | null;
  currentUser: JwtPayload | null;
}

const store = createStore({
state: {
    token: null,
    currentUser: null,
  },

  getters: {
    isAuthenticated(state) {
      return !!state.token;
    },
    getCurrentUser (state) {
      return state.currentUser ;
    }
  },

  mutations: {
    SET_TOKEN(state, token) {
      state.token = token
    },
    SET_USER(state, user) {
      state.currentUser = user
    }
  },

  actions: {
    saveToken({ commit }, token) {
      commit("SET_TOKEN", token);
      commit("SET_USER", jwtDecode<JwtPayload>(token));
      localStorage.setItem("token", token);
    },
    auto({ commit, dispatch }) {
      let token = localStorage.getItem("token");
      if (token) {
        const decodedToken = jwtDecode<JwtPayload>(token);
        const currentTime = Date.now() / 1000;

        if (decodedToken.exp < currentTime) {
          dispatch('logoff');
          return;
        } else {
          commit("SET_TOKEN", token);
          commit("SET_USER", jwtDecode<JwtPayload>(token));
          router.push({ name: "home" });
        }
      } else {
            router.push({ name: "login" });
      }
    },
    logout({ commit }) {
      commit("SET_TOKEN", null);
      commit("SET_USER", null);
      localStorage.removeItem("token");
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