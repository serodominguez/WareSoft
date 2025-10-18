import mainStore from "@/store";
import { jwtDecode } from "jwt-decode";
import { Category, CategoryState } from '@/models/categoryModel';
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

      const data = await fetchCategoriesService(
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

      commit("SET_CATEGORIES", data.items);
      commit("SET_TOTAL_CATEGORIES", data.totalRecords);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
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

      const categories = await selectCategoryService(token);
      commit("SET_CATEGORIES", categories);
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

      const category = await fetchCategoryByIdService(id, token);
      commit("SET_SELECTED_CATEGORY", category);
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

      await registerCategoryService(category, token);
      dispatch("fetchCategories", {});
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

      await editCategoryService(id, category, token);
      dispatch("fetchCategories", {});
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

      await enableCategoryService(id, token);
      dispatch("fetchCategories", {});
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

      await disableCategoryService(id, token);
      dispatch("fetchCategories", {});
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
      await removeCategoryService(id, token);
      dispatch("fetchCategories", {});
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