import mainStore from "@/store";
import { jwtDecode } from "jwt-decode";
import { Brand, BrandState, BaseResponse, FilterParams } from '@/interfaces/brandInterface';
import {
  fetchBrandsService,
  selectBrandService,
  fetchBrandByIdService,
  registerBrandService,
  editBrandService,
  enableBrandService,
  disableBrandService,
  removeBrandService,
} from '@/services/brandService';

interface DecodedToken {
  exp: number;
  [key: string]: any;
}

const state: BrandState = {
  brands: [] as Brand[],
  selectedBrand: null as Brand | null,
  totalBrands: 0,
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
  SET_BRANDS(state: any, brands: Brand[]) {
    state.brands = brands;
  },
  SET_TOTAL_BRANDS(state: any, total: number) {
    state.totalBrands = total;
  },
  SET_SELECTED_BRAND(state: any, brand: Brand | null) {
    state.selectedBrand = brand;
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
  async fetchBrands({ commit, rootState }: any,
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
    commit("SET_BRANDS", []);
    
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

      const result = await fetchBrandsService(
        pageNumber,
        pageSize,
        order,
        sort,
        textFilter,
        numberFilter,
        stateFilter,
        startDate,
        endDate,
        false
      );

      if (result.isSuccess) {
        commit("SET_BRANDS", result.data);
        commit("SET_TOTAL_BRANDS", result.totalRecords);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async downloadBrandsExcel(
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

      const blob = await fetchBrandsService(
        pageNumber,
        pageSize,
        order,
        sort,
        textFilter,
        numberFilter,
        stateFilter,
        startDate,
        endDate,
        true
      );

      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `Marcas_${new Date().toISOString().split('T')[0]}.xlsx`);
      document.body.appendChild(link);
      link.click();
      
      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },

  async selectBrand({ commit, rootState }: any) {
    commit("SET_LOADING", true);
    commit("SET_BRANDS", []);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await selectBrandService();
      if (result.isSuccess) {
        commit("SET_BRANDS", result.data);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async fetchBrandById({ commit, rootState }: any, id: number) {
    commit("SET_LOADING", true);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await fetchBrandByIdService(id);
      if (result.isSuccess) {
        commit("SET_SELECTED_BRAND", result.data);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async registerBrand({ commit, dispatch, rootState, state }: any, brand: Brand) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return { isSuccess: false, message: 'Sesión expirada' };
      }

      const result: BaseResponse = await registerBrandService(brand);
      if (result.isSuccess) {
        dispatch("fetchBrands", state.lastFilterParams || {});
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

  async editBrand({ commit, dispatch, rootState, state }: any, { id, brand }: { id: number; brand: Brand }) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return { isSuccess: false, message: 'Sesión expirada' };
      }

      const result: BaseResponse = await editBrandService(id, brand);
      if (result.isSuccess) {
        dispatch("fetchBrands", state.lastFilterParams || {});
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

  async enableBrand({ commit, dispatch, rootState, state }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return { isSuccess: false, message: 'Sesión expirada' };
      }

      const result: BaseResponse = await enableBrandService(id);
      if (result.isSuccess) {
        dispatch("fetchBrands", state.lastFilterParams || {});
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

  async disableBrand({ commit, dispatch, rootState, state }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return { isSuccess: false, message: 'Sesión expirada' };
      }

      const result: BaseResponse = await disableBrandService(id);
      if (result.isSuccess) {
        dispatch("fetchBrands", state.lastFilterParams || {});
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

  async removeBrand({ commit, dispatch, rootState, state }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return { isSuccess: false, message: 'Sesión expirada' };
      }
      
      const result: BaseResponse = await removeBrandService(id);
      if (result.isSuccess) {
        dispatch("fetchBrands", state.lastFilterParams || {});
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
  brands: (state: any) => state.brands,
  selectedBrand: (state: any) => state.selectedBrand,
  loading: (state: any) => state.loading,
  error: (state: any) => state.error,
  totalBrands: (state: any) => state.totalBrands || 0,
  //lastFilterParams: (state: any) => state.lastFilterParams,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};