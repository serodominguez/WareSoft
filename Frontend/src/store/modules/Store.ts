import mainStore from "@/store";
import { jwtDecode } from "jwt-decode";
import { Store, StoreState, BaseResponse } from '@/interfaces/storeInterface';
import {
  fetchStoresService,
  selectStoreService,
  fetchStoreByIdService,
  registerStoreService,
  editStoreService,
  enableStoreService,
  disableStoreService,
  removeStoreService,
} from '@/services/storeService';

interface DecodedToken {
  exp: number;
  [key: string]: any;
};

interface RootState {
  token: string;
  [key: string]: any;
};

const state: StoreState = {
  stores: [] as Store[],
  selectedStore: null as Store | null,
  totalStores: 0,
  loading: false,
  error: null as string | null,
};

const isExpired = (token: string | null): boolean => {
  if (!token) return true;
  try {
    const decodedToken = jwtDecode<DecodedToken>(token);
    const currentTime = Date.now() / 1000;
    return decodedToken.exp < currentTime;
  } catch {
    return true;
  }
};

const mutations = {
  SET_STORES(state: any, stores: Store[]) {
    state.stores = stores;
  },
  SET_TOTAL_STORES(state: any, total: number) {
    state.totalStores = total;
  },
  SET_SELECTED_STORE(state: any, store: Store | null) {
    state.selectedStore = store;
  },
  SET_LOADING(state: any, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: any, error: string | null) {
    state.error = error;
  },
};

const actions = {
  async fetchStores(
    { commit, rootState }: any,
    {
      pageNumber = 1,
      pageSize = 10,
      order = "desc",
      sort = "Id",
      textFilter = null,
      numberFilter = null,
      stateFilter = 1,
      startDate = null,
      endDate = null,
    } = {}
  ) {
    commit("SET_LOADING", true);
    commit("SET_STORES", []);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const requestBody: any = {
        numberPage: pageNumber,
        numberRecordsPage: pageSize,
        order,
        sort,
        stateFilter,
      };

      if (textFilter && numberFilter) {
        requestBody.textFilter = textFilter;
        requestBody.numberFilter = numberFilter;
      }

      if (startDate) {
        requestBody.startDate = startDate;
      }

      if (endDate) {
        requestBody.endDate = endDate;
      }

      const result = await fetchStoresService(
        pageNumber,
        pageSize,
        order,
        sort,
        textFilter,
        numberFilter,
        stateFilter,
        startDate,
        endDate,
        false,
        token
      );

      if (result.isSuccess) {
        commit("SET_STORES", result.data);
        commit("SET_TOTAL_STORES", result.totalRecords);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

   async downloadStoresExcel(
    { rootState }: any,
    { 
      pageNumber = 1, 
      pageSize = 10, 
      order = "desc", 
      sort = "Id", 
      textFilter = null, 
      numberFilter = null,
      stateFilter = 1,
      startDate = null,
      endDate = null
    } = {}
  ) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const blob = await fetchStoresService(
        pageNumber,
        pageSize,
        order,
        sort,
        textFilter,
        numberFilter,
        stateFilter,
        startDate,
        endDate,
        true,
        token
      );

      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `Tiendas_${new Date().toISOString().split('T')[0]}.xlsx`);
      document.body.appendChild(link);
      link.click();
      
      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },

  async selectStore({ commit, rootState }: any) {
    commit("SET_LOADING", true);
    commit("SET_STORES", []);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await selectStoreService(token);
            if (result.isSuccess) {
        commit("SET_STORES", result.data);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async fetchStoreById({ commit, rootState }: any, id: number) {
    commit("SET_LOADING", true);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await fetchStoreByIdService(id, token);
      if (result.isSuccess) {
        commit("SET_SELECTED_STORE", result.data);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async registerStore({ commit, dispatch, rootState }: any, store: Store) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await registerStoreService(store, token);
      if (result.isSuccess) {
        dispatch("fetchStores", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async editStore({ commit, dispatch, rootState }: any, { id, store }: { id: number; store: Store }) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }
      const result: BaseResponse = await editStoreService(id, store, token);
      if (result.isSuccess) {
        dispatch("fetchStores", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async enableStore({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await enableStoreService(id, token);
      if (result.isSuccess) {
        dispatch("fetchStores", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async disableStore({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await disableStoreService(id, token);
      if (result.isSuccess) {
        dispatch("fetchStores", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async removeStore({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await removeStoreService(id, token);
      if (result.isSuccess) {
        dispatch("fetchStores", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },
};

const getters = {
  stores: (state: any) => state.stores,
  selectedStore: (state: any) => state.selectedStore,
  loading: (state: any) => state.loading,
  error: (state: any) => state.error,
  totalStores: (state: any) => state.totalStores || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};

export type { Store };