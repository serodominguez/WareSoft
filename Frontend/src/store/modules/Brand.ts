import mainStore from "@/store";
import { jwtDecode } from "jwt-decode";
import { Brand, BrandState } from '@/models/brandModel';
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

      const data = await fetchBrandsService(
        requestBody.numberPage,
        requestBody.numberRecordsPage,
        requestBody.order,
        requestBody.sort,
        requestBody.textFilter,
        requestBody.numberFilter,
        requestBody.stateFilter,
        requestBody.startDate,
        requestBody.endDate,
        token
      );

      commit("SET_BRANDS", data.items);
      commit("SET_TOTAL_BRANDS", data.totalRecords);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
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

      const brands = await selectBrandService( token);
      commit("SET_BRANDS", brands);
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

      const brand = await fetchBrandByIdService(id, token);
      commit("SET_SELECTED_BRAND", brand);
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

      await registerBrandService(brand, token);
      dispatch("fetchBrands", {});
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

      await editBrandService(id, brand, token);
      dispatch("fetchBrands", {});
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

      await enableBrandService(id, token);
      dispatch("fetchBrands", {});
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

      await disableBrandService(id, token);
      dispatch("fetchBrands", {});
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
      await removeBrandService(id, token);
      dispatch("fetchBrands", {});
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