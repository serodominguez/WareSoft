import { Category } from '@/interfaces/categoryInterface';
import { categoryService } from '@/services/categoryService';
import { BaseState, FilterParams } from '@/interfaces/baseInterface'

const state: BaseState<Category> = {
  items: [] as Category[],
  selectedItem: null as Category | null,
  totalItems: 0,
  loading: false,
  error: null as string | null,
  lastFilterParams: undefined,
};

const mutations = {
  SET_ITEMS(state: BaseState<Category>, items: Category[]) {
    state.items = items;
  },
  SET_TOTAL_ITEMS(state: BaseState<Category>, total: number) {
    state.totalItems = total;
  },
  SET_SELECTED_ITEMS(state: BaseState<Category>, item: Category | null) {
    state.selectedItem = item;
  },
  SET_LOADING(state: BaseState<Category>, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: BaseState<Category>, error: string | null) {
    state.error = error;
  },
  SET_LAST_FILTER_PARAMS(state: BaseState<Category>, params: FilterParams) {
    state.lastFilterParams = params;
  },
};

const actions = {
  async fetchCategories({ commit }: any, params: FilterParams = {}) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    commit("SET_LAST_FILTER_PARAMS", params);

    try {
      const result = await categoryService.fetchAll(params);
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


  async downloadCategoriesExcel({ state }: any, params?: FilterParams) {
    try {
      const filterParams = params || state.lastFilterParams || {};
      await categoryService.downloadExcel(filterParams);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },


  async selectCategory({ commit }: any) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    
    try {
      const result = await categoryService.select();
      
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

  async fetchCategoryById({ commit }: any, id: number) {
    commit("SET_LOADING", true);
    
    try {
      const result = await categoryService.fetchById(id);
      
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

  async registerCategory({ dispatch, state }: any, category: Category) {
    try {
      const result = await categoryService.create(category);
      
      if (result.isSuccess) {
        dispatch("fetchCategories", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async editCategory({ dispatch, state }: any, { id, category }: { id: number; category: Category }) {
    try {
      const result = await categoryService.update(id, category);
      
      if (result.isSuccess) {
        dispatch("fetchCategories", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async enableCategory({ dispatch, state }: any, id: number) {
    try {
      const result = await categoryService.enable(id);
      
      if (result.isSuccess) {
        dispatch("fetchCategories", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async disableCategory({ dispatch, state }: any, id: number) {
    try {
      const result = await categoryService.disable(id);
      
      if (result.isSuccess) {
        dispatch("fetchCategories", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async removeCategory({ dispatch, state }: any, id: number) {
    try {
      const result = await categoryService.remove(id);
      
      if (result.isSuccess) {
        dispatch("fetchCategories", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },
};

const getters = {
  categories: (state: BaseState<Category>) => state.items,
  selectedCategory: (state: BaseState<Category>) => state.selectedItem,
  loading: (state: BaseState<Category>) => state.loading,
  error: (state: BaseState<Category>) => state.error,
  totalCategories: (state: BaseState<Category>) => state.totalItems || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};