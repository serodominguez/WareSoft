import { Store, StoreState } from '@/models/storeModel';
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

const state: StoreState = {
  stores: [] as Store[],
  selectedStore: null as Store | null,
  totalStores: 0,
  loading: false,
  error: null as string | null,
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
    { commit }: any,
    { 
      pageNumber = 1, 
      pageSize = 10, 
      order = "desc", 
      sort = "PK_STORE", 
      textFilter = null, 
      numberFilter = null,
      stateFilter = 1,
      startDate = null,
      endDate = null
    } = {}
  ) {
    commit("SET_LOADING", true);
    try {
      const requestBody: any = {
        numberPage: pageNumber,
        numberRecordsPage: pageSize,
        order,
        sort,
        stateFilter
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

      const data = await fetchStoresService(
        requestBody.numberPage,
        requestBody.numberRecordsPage,
        requestBody.order,
        requestBody.sort,
        requestBody.textFilter,
        requestBody.numberFilter,
        requestBody.stateFilter,
        requestBody.startDate,
        requestBody.endDate
      );

      commit("SET_STORES", data.items);
      commit("SET_TOTAL_STORES", data.totalRecords);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async selectStore({ commit }: any) {
    commit("SET_LOADING", true);
    try {
      const stores = await selectStoreService();
      commit("SET_STORES", stores);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async fetchStoreById({ commit }: any, id: number) {
    commit("SET_LOADING", true);
    try {
      const store = await fetchStoreByIdService(id);
      commit("SET_SELECTED_STORE", store);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async registerStore({ dispatch }: any, store: Store) {
    await registerStoreService(store);
    dispatch("fetchStores", {});
  },

  async editStore({ dispatch }: any, { id, store }: { id: number; store: Store }) {
    await editStoreService(id, store);
    dispatch("fetchStores", {});
  },

  async enableStore({ dispatch }: any, id: number) {
    await enableStoreService(id);
    dispatch("fetchStores", {});
  },
  async disableStore({ dispatch }: any, id: number) {
    await disableStoreService(id);
    dispatch("fetchStores", {});
  },

  async removeStore({ dispatch }: any, id: number) {
    await removeStoreService(id);
    dispatch("fetchStores", {});
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