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

const state: BrandState = {
  brands: [] as Brand[],
  selectedBrand: null as Brand | null,
  totalBrands: 0,
  loading: false,
  error: null as string | null,
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
  async fetchBrands(
    { commit }: any,
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

      const data = await fetchBrandsService(
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

      commit("SET_BRANDS", data.items);
      commit("SET_TOTAL_BRANDS", data.totalRecords);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async selectBrand({ commit }: any) {
    commit("SET_LOADING", true);
    try {
      const brands = await selectBrandService();
      commit("SET_BRANDS", brands);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async fetchBrandById({ commit }: any, id: number) {
    commit("SET_LOADING", true);
    try {
      const brand = await fetchBrandByIdService(id);
      commit("SET_SELECTED_BRAND", brand);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async registerBrand({ dispatch }: any, brand: Brand) {
    await registerBrandService(brand);
    dispatch("fetchBrands", {});
  },

  async editBrand({ dispatch }: any, { id, brand }: { id: number; brand: Brand }) {
    await editBrandService(id, brand);
    dispatch("fetchBrands", {});
  },

  async enableBrand({ dispatch }: any, id: number) {
    await enableBrandService(id);
    dispatch("fetchBrands", {});
  },
  async disableBrand({ dispatch }: any, id: number) {
    await disableBrandService(id);
    dispatch("fetchBrands", {});
  },

  async removeBrand({ dispatch }: any, id: number) {
    await removeBrandService(id);
    dispatch("fetchBrands", {});
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