import { Module } from '@/interfaces/moduleInterface';
import { moduleService } from '@/services/moduleService';
import { BaseState, FilterParams } from '@/interfaces/baseInterface'

const state: BaseState<Module> = {
  items: [] as Module[],
  selectedItem: null as Module | null,
  totalItems: 0,
  loading: false,
  error: null as string | null,
  lastFilterParams: undefined,
};

const mutations = {
  SET_ITEMS(state: BaseState<Module>, items: Module[]) {
    state.items = items;
  },
  SET_TOTAL_ITEMS(state: BaseState<Module>, total: number) {
    state.totalItems = total;
  },
  SET_SELECTED_ITEMS(state: BaseState<Module>, item: Module | null) {
    state.selectedItem = item;
  },
  SET_LOADING(state: BaseState<Module>, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: BaseState<Module>, error: string | null) {
    state.error = error;
  },
  SET_LAST_FILTER_PARAMS(state: BaseState<Module>, params: FilterParams) {
    state.lastFilterParams = params;
  },
};

const actions = {
  async fetchModules({ commit }: any, params: FilterParams = {}) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    commit("SET_LAST_FILTER_PARAMS", params);

    try {
      const result = await moduleService.fetchAll(params);
      
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

  async downloadModulesExcel({ state }: any, params?: FilterParams) {
    try {
      const filterParams = params || state.lastFilterParams || {};
      await moduleService.downloadExcel(filterParams);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },

  async fetchModuleById({ commit }: any, id: number) {
    commit("SET_LOADING", true);
    
    try {
      const result = await moduleService.fetchById(id);
      
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

  async registerModule({ dispatch, state }: any, module: Module) {
    try {
      const result = await moduleService.create(module);
      
      if (result.isSuccess) {
        dispatch("fetchModules", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async editModule({ dispatch, state }: any, { id, module }: { id: number; module: Module }) {
    try {
      const result = await moduleService.update(id, module);
      
      if (result.isSuccess) {
        dispatch("fetchModules", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async enableModule({ dispatch, state }: any, id: number) {
    try {
      const result = await moduleService.enable(id);
      
      if (result.isSuccess) {
        dispatch("fetchModules", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async disableModule({ dispatch, state }: any, id: number) {
    try {
      const result = await moduleService.disable(id);
      
      if (result.isSuccess) {
        dispatch("fetchModules", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async removeModule({ dispatch, state }: any, id: number) {
    try {
      const result = await moduleService.remove(id);
      
      if (result.isSuccess) {
        dispatch("fetchModule", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },
};

const getters = {
  modules: (state: BaseState<Module>) => state.items,
  selectedModule: (state: BaseState<Module>) => state.selectedItem,
  loading: (state: BaseState<Module>) => state.loading,
  error: (state: BaseState<Module>) => state.error,
  totalModules: (state: BaseState<Module>) => state.totalItems || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};