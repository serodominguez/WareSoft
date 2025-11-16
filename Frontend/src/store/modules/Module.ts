import mainStore from "@/store";
import { jwtDecode } from "jwt-decode";
import { Module, ModuleState, BaseResponse } from '@/interfaces/moduleInterface';
import {
  fetchModulesService,
  fetchModuleByIdService,
  registerModuleService,
  editModuleService,
  enableModuleService,
  disableModuleService,
  removeModuleService,
} from '@/services/moduleService';

interface DecodedToken {
  exp: number;
  [key: string]: any;
};

interface RootState {
  token: string;
  [key: string]: any;
};

const state: ModuleState = {
  modules: [] as Module[],
  selectedModule: null as Module | null,
  totalModules: 0,
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
  SET_MODULES(state: any, modules: Module[]) {
    state.modules = modules;
  },
  SET_TOTAL_MODULES(state: any, total: number) {
    state.totalModules = total;
  },
  SET_SELECTED_MODULE(state: any, module: Module | null) {
    state.selectedModule = module;
  },
  SET_LOADING(state: any, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: any, error: string | null) {
    state.error = error;
  },
};

const actions = {
  async fetchModules(
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
    commit("SET_MODULES", []);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result = await fetchModulesService(
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
        commit("SET_MODULES", result.data);
        commit("SET_TOTAL_MODULES", result.totalRecords);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async downloadModulesExcel(
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

      const blob = await fetchModulesService(
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
      link.setAttribute('download', `Módulos_${new Date().toISOString().split('T')[0]}.xlsx`);
      document.body.appendChild(link);
      link.click();
      
      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },

  async fetchModuleById({ commit, rootState }: any, id: number) {
    commit("SET_LOADING", true);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await fetchModuleByIdService(id, token);
      if (result.isSuccess) {
        commit("SET_SELECTED_MODULE", result.data);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async registerModule({ commit, dispatch, rootState }: any, module: Module) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await registerModuleService(module, token);
      if (result.isSuccess) {
        dispatch("fetchModules", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async editModule({ commit, dispatch, rootState }: any, { id, module }: { id: number; module: Module }) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await editModuleService(id, module, token);
      if (result.isSuccess) {
        dispatch("fetchModules", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async enableModule({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await enableModuleService(id, token);
      if (result.isSuccess) {
        dispatch("fetchModules", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },
  async disableModule({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await disableModuleService(id, token);
      if (result.isSuccess) {
        dispatch("fetchModules", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async removeModule({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }
      const result: BaseResponse = await removeModuleService(id, token);
      if (result.isSuccess) {
        dispatch("fetchModules", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }

  },
};

const getters = {
  modules: (state: any) => state.modules,
  selectedModule: (state: any) => state.selectedModule,
  loading: (state: any) => state.loading,
  error: (state: any) => state.error,
  totalModules: (state: any) => state.totalModules || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};

export type { Module };