import { Store } from '@/interfaces/storeInterface';
import { storeService } from '@/services/storeService';
import { BaseState, FilterParams } from '@/interfaces/baseInterface'

const state: BaseState<Store> = {
  items: [] as Store[],
  selectedItem: null as Store | null,
  totalItems: 0,
  loading: false,
  error: null as string | null,
  lastFilterParams: undefined,
};

const mutations = {
  SET_ITEMS(state: BaseState<Store>, items: Store[]) {
    state.items = items;
  },
  SET_TOTAL_ITEMS(state: BaseState<Store>, total: number) {
    state.totalItems = total;
  },
  SET_SELECTED_ITEMS(state: BaseState<Store>, item: Store | null) {
    state.selectedItem = item;
  },
  SET_LOADING(state: BaseState<Store>, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: BaseState<Store>, error: string | null) {
    state.error = error;
  },
  SET_LAST_FILTER_PARAMS(state: BaseState<Store>, params: FilterParams) {
    state.lastFilterParams = params;
  },
};

const actions = {
  async fetchStores({ commit }: any, params: FilterParams = {}) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    commit("SET_LAST_FILTER_PARAMS", params);

    try {
      const result = await storeService.fetchAll(params);
      if (result.isSuccess) {
        commit("SET_ITEMS", result.data);
        commit("SET_TOTAL_ITEMS", result.totalRecords);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },


  async downloadStoresExcel({ state }: any, params?: FilterParams) {
    try {
      const filterParams = params || state.lastFilterParams || {};
      await storeService.downloadExcel(filterParams);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },


  async selectStore({ commit }: any) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    
    try {
      const result = await storeService.select();
      
      if (result.isSuccess) {
        commit("SET_ITEMS", result.data);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async fetchStoreById({ commit }: any, id: number) {
    commit("SET_LOADING", true);
    
    try {
      const result = await storeService.fetchById(id);
      
      if (result.isSuccess) {
        commit("SET_SELECTED_ITEM", result.data);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async registerStore({ dispatch, state }: any, store: Store) {
    try {
      const result = await storeService.create(store);
      
      if (result.isSuccess) {
        dispatch("fetchStores", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async editStore({ dispatch, state }: any, { id, store }: { id: number; store: Store }) {
    try {
      const result = await storeService.update(id, store);
      
      if (result.isSuccess) {
        dispatch("fetchStores", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async enableStore({ dispatch, state }: any, id: number) {
    try {
      const result = await storeService.enable(id);
      
      if (result.isSuccess) {
        dispatch("fetchStores", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async disableStore({ dispatch, state }: any, id: number) {
    try {
      const result = await storeService.disable(id);
      
      if (result.isSuccess) {
        dispatch("fetchStores", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async removeStore({ dispatch, state }: any, id: number) {
    try {
      const result = await storeService.remove(id);
      
      if (result.isSuccess) {
        dispatch("fetchStores", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },
};

const getters = {
  stores: (state: BaseState<Store>) => state.items,
  selectedStore: (state: BaseState<Store>) => state.selectedItem,
  loading: (state: BaseState<Store>) => state.loading,
  error: (state: BaseState<Store>) => state.error,
  totalStores: (state: BaseState<Store>) => state.totalItems || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};