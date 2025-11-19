import mainStore from "@/store";
import { jwtDecode } from "jwt-decode";
import { Product, ProductState, BaseResponse, FilterParams } from '@/interfaces/productInterface';
import {
  fetchProductsService,
  fetchProductByIdService,
  registerProductService,
  editProductService,
  enableProductService,
  disableProductService,
  removeProductService,
} from '@/services/productService';

interface DecodedToken {
  exp: number;
  [key: string]: any;
};

interface RootState {
  token: string;
  [key: string]: any;
};

const state: ProductState = {
  products: [] as Product[],
  selectedProduct: null as Product | null,
  totalProducts: 0,
  loading: false,
  error: null as string | null,
  lastFilterParams: undefined,
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
  SET_PRODUCTS(state: any, products: Product[]) {
    state.products = products;
  },
  SET_TOTAL_PRODUCTS(state: any, total: number) {
    state.totalProducts = total;
  },
  SET_SELECTED_PRODUCT(state: any, product: Product | null) {
    state.selectedProduct = product;
  },
  SET_LOADING(state: any, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: any, error: string | null) {
    state.error = error;
  },
  SET_LAST_FILTER_PARAMS(state: any, params: FilterParams) {
    state.lastFilterParams = params;
  },
};

const actions = {
  async fetchProducts(
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
      endDate = null
    } = {}
  ) {
    commit("SET_LOADING", true);
    commit("SET_PRODUCTS", []);

    const filterParams = {
      pageNumber,
      pageSize,
      order,
      sort,
      textFilter,
      numberFilter,
      stateFilter,
      startDate,
      endDate
    };
    commit("SET_LAST_FILTER_PARAMS", filterParams);

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

      const result = await fetchProductsService(
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
        commit("SET_PRODUCTS", result.data);
        commit("SET_TOTAL_PRODUCTS", result.totalRecords);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

async downloadProductsExcel(
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

      const blob = await fetchProductsService(
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
      link.setAttribute('download', `Productos_${new Date().toISOString().split('T')[0]}.xlsx`);
      document.body.appendChild(link);
      link.click();
      
      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },

  async fetchProductById({ commit, rootState }: any, id: number) {
    commit("SET_LOADING", true);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await fetchProductByIdService(id, token);
      if (result.isSuccess) {
        commit("SET_SELECTED_PRODUCT", result.data);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async registerProduct({ commit, dispatch, rootState, state }: any, product: Product) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await registerProductService(product, token);
      if (result.isSuccess) {
        dispatch("fetchProducts", state.lastFilterParams || {});
        return result;
      } else {
        commit("SET_ERROR", result.message || result.errors);
        return result;
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async editProduct({ commit, dispatch, rootState, state }: any, { id, product }: { id: number; product: Product }) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await editProductService(id, product, token);
      if (result.isSuccess) {
        dispatch("fetchProducts", state.lastFilterParams || {});
        return result;
      } else {
        commit("SET_ERROR", result.message || result.errors);
        return result;
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async enableProduct({ commit, dispatch, rootState, state }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await enableProductService(id, token);
      if (result.isSuccess) {
        dispatch("fetchProducts", state.lastFilterParams || {});
        return result;
      } else {
        commit("SET_ERROR", result.message || result.errors);
        return result;
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
      return { isSuccess: false, message: error.message, errors: error };
    }
  },
  async disableProduct({ commit, dispatch, rootState, state }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await disableProductService(id, token);
      if (result.isSuccess) {
        dispatch("fetchProducts", state.lastFilterParams || {});
        return result;
      } else {
        commit("SET_ERROR", result.message || result.errors);
        return result;
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async removeProduct({ commit, dispatch, rootState, state }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await removeProductService(id, token);
      if (result.isSuccess) {
        dispatch("fetchProducts", state.lastFilterParams || {});
        return result;
      } else {
        commit("SET_ERROR", result.message || result.errors);
        return result;
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
      return { isSuccess: false, message: error.message, errors: error };
    }
  },
};

const getters = {
  products: (state: any) => state.products,
  selectedProduct: (state: any) => state.selectedProduct,
  loading: (state: any) => state.loading,
  error: (state: any) => state.error,
  totalProducts: (state: any) => state.totalProducts || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};
