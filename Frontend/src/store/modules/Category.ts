import mainStore from "@/store";
import { jwtDecode } from "jwt-decode";
import { Category, CategoryState, BaseResponse } from '@/interfaces/categoryInterface';
import {
  fetchCategoriesService,
  selectCategoryService,
  fetchCategoryByIdService,
  registerCategoryService,
  editCategoryService,
  enableCategoryService,
  disableCategoryService,
  removeCategoryService,
} from '@/services/categoryService';

interface DecodedToken {
  exp: number;
  [key: string]: any;
};

interface RootState {
  token: string;
  [key: string]: any;
};

const state: CategoryState = {
  categories: [] as Category[],
  selectedCategory: null as Category | null,
  totalCategories: 0,
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
  SET_CATEGORIES(state: any, categories: Category[]) {
    state.categories = categories;
  },
  SET_TOTAL_CATEGORIES(state: any, total: number) {
    state.totalCategories = total;
  },
  SET_SELECTED_CATEGORY(state: any, category: Category | null) {
    state.selectedCategory = category;
  },
  SET_LOADING(state: any, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: any, error: string | null) {
    state.error = error;
  },
};

const actions = {
  async fetchCategories(
    { commit, rootState }: any,
    { 
      pageNumber = 1, 
      pageSize = 10, 
      order = "desc", 
      sort = "PK_CATEGORY", 
      textFilter = null, 
      numberFilter = null,
      stateFilter = 1,
      startDate = null,
      endDate = null
    } = {}
  ) {
    commit("SET_LOADING", true);
    commit("SET_CATEGORIES", []);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result = await fetchCategoriesService(
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
        commit("SET_CATEGORIES", result.data);
        commit("SET_TOTAL_CATEGORIES", result.totalRecords);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async downloadCategoriesExcel(
    { rootState }: any,
    { 
      pageNumber = 1, 
      pageSize = 10, 
      order = "desc", 
      sort = "PK_CATEGORY", 
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

      const blob = await fetchCategoriesService(
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
      link.setAttribute('download', `Categorias_${new Date().toISOString().split('T')[0]}.xlsx`);
      document.body.appendChild(link);
      link.click();
      
      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },

  async selectCategory({ commit, rootState }: any) {
    commit("SET_LOADING", true);
    commit("SET_CATEGORIES", []);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await selectCategoryService(token);
      if (result.isSuccess) {
        commit("SET_CATEGORIES", result.data);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async fetchCategoryById({ commit, rootState }: any, id: number) {
    commit("SET_LOADING", true);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await fetchCategoryByIdService(id, token);
      if (result.isSuccess) {
        commit("SET_SELECTED_CATEGORY", result.data);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async registerCategory({ commit, dispatch, rootState }: any, category: Category) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await registerCategoryService(category, token);
      if (result.isSuccess) {
        dispatch("fetchCategories", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async editCategory({ commit, dispatch, rootState }: any, { id, category }: { id: number; category: Category }) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await editCategoryService(id, category, token);
      if (result.isSuccess) {
        dispatch("fetchCategories", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async enableCategory({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await enableCategoryService(id, token);
      if (result.isSuccess) {
        dispatch("fetchCategories", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async disableCategory({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

       const result: BaseResponse = await disableCategoryService(id, token);
      if (result.isSuccess) {
        dispatch("fetchCategories", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async removeCategory({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }
      const result: BaseResponse = await removeCategoryService(id, token);
      if (result.isSuccess) {
        dispatch("fetchCategories", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },
};

const getters = {
  categories: (state: any) => state.categories,
  selectedCategory: (state: any) => state.selectedCategory,
  loading: (state: any) => state.loading,
  error: (state: any) => state.error,
  totalCategories: (state: any) => state.totalCategories || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};

export type { Category };