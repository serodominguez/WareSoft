import { Product } from '@/interfaces/productInterface';
import { productService } from '@/services/productService';
import { BaseState, FilterParams } from '@/interfaces/baseInterface'

const state: BaseState<Product> = {
  items: [] as Product[],
  selectedItem: null as Product | null,
  totalItems: 0,
  loading: false,
  error: null as string | null,
  lastFilterParams: undefined,
};

const mutations = {
  SET_ITEMS(state: BaseState<Product>, items: Product[]) {
    state.items = items;
  },
  SET_TOTAL_ITEMS(state: BaseState<Product>, total: number) {
    state.totalItems = total;
  },
  SET_SELECTED_ITEMS(state: BaseState<Product>, item: Product | null) {
    state.selectedItem = item;
  },
  SET_LOADING(state: BaseState<Product>, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: BaseState<Product>, error: string | null) {
    state.error = error;
  },
  SET_LAST_FILTER_PARAMS(state: BaseState<Product>, params: FilterParams) {
    state.lastFilterParams = params;
  },
};

const actions = {
  async fetchProducts({ commit }: any, params: FilterParams = {}) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    commit("SET_LAST_FILTER_PARAMS", params);

    try {
      const result = await productService.fetchAll(params);
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


  async downloadProductsExcel({ state }: any, params?: FilterParams) {
    try {
      const filterParams = params || state.lastFilterParams || {};
      await productService.downloadExcel(filterParams);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },

  async fetchProductById({ commit }: any, id: number) {
    commit("SET_LOADING", true);
    
    try {
      const result = await productService.fetchById(id);
      
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

  async registerProduct({ dispatch, state }: any, product: Product) {
    try {
      const result = await productService.create(product);
      
      if (result.isSuccess) {
        dispatch("fetchProducts", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async editProduct({ dispatch, state }: any, { id, product }: { id: number; product: Product }) {
    try {
      const result = await productService.update(id, product);
      
      if (result.isSuccess) {
        dispatch("fetchProducts", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async enableProduct({ dispatch, state }: any, id: number) {
    try {
      const result = await productService.enable(id);
      
      if (result.isSuccess) {
        dispatch("fetchProducts", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async disableProduct({ dispatch, state }: any, id: number) {
    try {
      const result = await productService.disable(id);
      
      if (result.isSuccess) {
        dispatch("fetchProducts", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async removeProduct({ dispatch, state }: any, id: number) {
    try {
      const result = await productService.remove(id);
      
      if (result.isSuccess) {
        dispatch("fetchProducts", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },
};

const getters = {
  products: (state: BaseState<Product>) => state.items,
  selectedProduct: (state: BaseState<Product>) => state.selectedItem,
  loading: (state: BaseState<Product>) => state.loading,
  error: (state: BaseState<Product>) => state.error,
  totalProducts: (state: BaseState<Product>) => state.totalItems || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};