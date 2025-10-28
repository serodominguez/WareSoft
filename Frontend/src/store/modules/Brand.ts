import mainStore from "@/store";
import { jwtDecode } from "jwt-decode";
import { Brand, BrandState, BaseResponse } from '@/interfaces/brandInterface';
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
};

interface RootState {
  token: string;
  [key: string]: any;
};

const state: BrandState = {
  brands: [] as Brand[],
  selectedBrand: null as Brand | null,
  totalBrands: 0,
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
};

const actions = {
  async fetchBrands({ commit, rootState }: any,
    { 
      pageNumber = 1, 
      pageSize = 10, 
      order = "desc", 
      sort = "PK_BRAND", 
      textFilter = null, 
      numberFilter = null,
      stateFilter = 1,
      startDate = null,
      endDate = null
    } = {}
  ) {
    commit("SET_LOADING", true);
    commit("SET_BRANDS", []);
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
        false,
        token
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
      sort = "PK_BRAND", 
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
        true,
        token
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

      const result: BaseResponse = await selectBrandService( token);
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

      const result: BaseResponse = await fetchBrandByIdService(id, token);
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

  async registerBrand({ commit,dispatch, rootState }: any, brand: Brand) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await registerBrandService(brand, token);
      if (result.isSuccess) {
        dispatch("fetchBrands", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async editBrand({ commit, dispatch, rootState }: any, { id, brand }: { id: number; brand: Brand }) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await editBrandService(id, brand, token);
      if (result.isSuccess) {
        dispatch("fetchBrands", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async enableBrand({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await enableBrandService(id, token);
      if (result.isSuccess) {
        dispatch("fetchBrands", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async disableBrand({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await disableBrandService(id, token);
      if (result.isSuccess) {
        dispatch("fetchBrands", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async removeBrand({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }
      const result: BaseResponse = await removeBrandService(id, token);
      if (result.isSuccess) {
        dispatch("fetchBrands", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },
};

const getters = {
  brands: (state: any) => state.brands,
  selectedBrand: (state: any) => state.selectedBrand,
  loading: (state: any) => state.loading,
  error: (state: any) => state.error,
  totalBrands: (state: any) => state.totalBrands || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};

export type { Brand };