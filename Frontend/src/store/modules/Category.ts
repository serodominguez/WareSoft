import { Category, CategoryState } from '@/models/categoryModel';
import {
  fetchCategoriesService,
  selectCategoryService,
  fetchCategoryByIdService,
  registerCategoryService,
  editCategoryService,
  removeCategoryService,
} from '@/services/categoryService';

const state: CategoryState = {
  categories: [] as Category[],
  selectedCategory: null as Category | null,
  totalCategories: 0,
  loading: false,
  error: null as string | null,
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
  async fetchCategories({ commit }: any,{ pageNumber = 1, pageSize = 5, order = "asc", sort = "PK_CATEGORY", textFilter = null, numberFilter = null,} = {}) {
    commit("SET_LOADING", true);
    try {
      const requestBody: any = {
        numberPage: pageNumber,
        numberRecordsPage: pageSize,
        order,
        sort,
      };

      if (textFilter && numberFilter) {
        requestBody.textFilter = textFilter;
        requestBody.numberFilter = numberFilter;
      }

      const data = await fetchCategoriesService(
        requestBody.numberPage,
        requestBody.numberRecordsPage,
        requestBody.order,
        requestBody.sort,
        requestBody.textFilter,
        requestBody.numberFilter
      );

      commit("SET_CATEGORIES", data.items);
      commit("SET_TOTAL_CATEGORIES", data.totalRecords);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async selectCategory({ commit }: any) {
    commit("SET_LOADING", true);
    try {
      const categories = await selectCategoryService();
      commit("SET_CATEGORIES", categories);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async fetchCategoryById({ commit }: any, id: number) {
    commit("SET_LOADING", true);
    try {
      const category = await fetchCategoryByIdService(id);
      commit("SET_SELECTED_CATEGORY", category);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async registerCategory({ dispatch }: any, category: Category) {
    await registerCategoryService(category);
    dispatch("fetchCategories", {});
  },

  async editCategory({ dispatch }: any, { id, category }: { id: number; category: Category }) {
    await editCategoryService(id, category);
    dispatch("fetchCategories", {});
  },

  async removeCategory({ dispatch }: any, id: number) {
    await removeCategoryService(id);
    dispatch("fetchCategories", {});
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
