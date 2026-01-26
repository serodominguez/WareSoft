import { Supplier } from '@/interfaces/supplierInterface';
import { supplierService } from '@/services/supplierService';
import { BaseState, FilterParams } from '@/interfaces/baseInterface'

const state: BaseState<Supplier> = {
  items: [] as Supplier[],
  selectedItem: null as Supplier | null,
  totalItems: 0,
  loading: false,
  error: null as string | null,
  lastFilterParams: undefined,
};

const mutations = {
  SET_ITEMS(state: BaseState<Supplier>, items: Supplier[]) {
    state.items = items;
  },
  SET_TOTAL_ITEMS(state: BaseState<Supplier>, total: number) {
    state.totalItems = total;
  },
  SET_SELECTED_ITEMS(state: BaseState<Supplier>, item: Supplier | null) {
    state.selectedItem = item;
  },
  SET_LOADING(state: BaseState<Supplier>, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: BaseState<Supplier>, error: string | null) {
    state.error = error;
  },
  SET_LAST_FILTER_PARAMS(state: BaseState<Supplier>, params: FilterParams) {
    state.lastFilterParams = params;
  },
};

const actions = {
  async fetchSuppliers({ commit }: any, params: FilterParams = {}) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    commit("SET_LAST_FILTER_PARAMS", params);

    try {
      const result = await supplierService.fetchAll(params);
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


  async downloadSuppliersExcel({ state }: any, params?: FilterParams) {
    try {
      const filterParams = params || state.lastFilterParams || {};
      await supplierService.downloadExcel(filterParams);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },

  async selectSupplier({ commit }: any) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    
    try {
      const result = await supplierService.select();
      
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

  async fetchSupplierById({ commit }: any, id: number) {
    commit("SET_LOADING", true);
    
    try {
      const result = await supplierService.fetchById(id);
      
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

  async registerSupplier({ dispatch, state }: any, supplier: Supplier) {
    try {
      const result = await supplierService.create(supplier);
      
      if (result.isSuccess) {
        dispatch("fetchSuppliers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async editSupplier({ dispatch, state }: any, { id, supplier }: { id: number; supplier: Supplier }) {
    try {
      const result = await supplierService.update(id, supplier);
      
      if (result.isSuccess) {
        dispatch("fetchSuppliers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async enableSupplier({ dispatch, state }: any, id: number) {
    try {
      const result = await supplierService.enable(id);
      
      if (result.isSuccess) {
        dispatch("fetchSuppliers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async disableSupplier({ dispatch, state }: any, id: number) {
    try {
      const result = await supplierService.disable(id);
      
      if (result.isSuccess) {
        dispatch("fetchSuppliers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async removeSupplier({ dispatch, state }: any, id: number) {
    try {
      const result = await supplierService.remove(id);
      
      if (result.isSuccess) {
        dispatch("fetchSuppliers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },
};

const getters = {
  suppliers: (state: BaseState<Supplier>) => state.items,
  selectedSupplier: (state: BaseState<Supplier>) => state.selectedItem,
  loading: (state: BaseState<Supplier>) => state.loading,
  error: (state: BaseState<Supplier>) => state.error,
  totalSuppliers: (state: BaseState<Supplier>) => state.totalItems || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};