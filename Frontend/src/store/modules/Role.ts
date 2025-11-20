import { Role } from '@/interfaces/roleInterface';
import { roleService } from '@/services/roleService';
import { BaseState, FilterParams } from '@/interfaces/baseInterface'

const state: BaseState<Role> = {
  items: [] as Role[],
  selectedItem: null as Role | null,
  totalItems: 0,
  loading: false,
  error: null as string | null,
  lastFilterParams: undefined,
};

const mutations = {
  SET_ITEMS(state: BaseState<Role>, items: Role[]) {
    state.items = items;
  },
  SET_TOTAL_ITEMS(state: BaseState<Role>, total: number) {
    state.totalItems = total;
  },
  SET_SELECTED_ITEMS(state: BaseState<Role>, item: Role | null) {
    state.selectedItem = item;
  },
  SET_LOADING(state: BaseState<Role>, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: BaseState<Role>, error: string | null) {
    state.error = error;
  },
  SET_LAST_FILTER_PARAMS(state: BaseState<Role>, params: FilterParams) {
    state.lastFilterParams = params;
  },
};

const actions = {
  async fetchRoles({ commit }: any, params: FilterParams = {}) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    commit("SET_LAST_FILTER_PARAMS", params);

    try {
      const result = await roleService.fetchAll(params);
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


  async downloadRolesExcel({ state }: any, params?: FilterParams) {
    try {
      const filterParams = params || state.lastFilterParams || {};
      await roleService.downloadExcel(filterParams);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },


  async selectRole({ commit }: any) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    
    try {
      const result = await roleService.select();
      
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

  async fetchRoleById({ commit }: any, id: number) {
    commit("SET_LOADING", true);
    
    try {
      const result = await roleService.fetchById(id);
      
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

  async registerRole({ dispatch, state }: any, role: Role) {
    try {
      const result = await roleService.create(role);
      
      if (result.isSuccess) {
        dispatch("fetchRoles", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async editRole({ dispatch, state }: any, { id, role }: { id: number; role: Role }) {
    try {
      const result = await roleService.update(id, role);
      
      if (result.isSuccess) {
        dispatch("fetchRoles", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async enableRole({ dispatch, state }: any, id: number) {
    try {
      const result = await roleService.enable(id);
      
      if (result.isSuccess) {
        dispatch("fetchRoles", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async disableRole({ dispatch, state }: any, id: number) {
    try {
      const result = await roleService.disable(id);
      
      if (result.isSuccess) {
        dispatch("fetchRoles", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async removeRole({ dispatch, state }: any, id: number) {
    try {
      const result = await roleService.remove(id);
      
      if (result.isSuccess) {
        dispatch("fetchRoles", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },
};

const getters = {
  roles: (state: BaseState<Role>) => state.items,
  selectedRole: (state: BaseState<Role>) => state.selectedItem,
  loading: (state: BaseState<Role>) => state.loading,
  error: (state: BaseState<Role>) => state.error,
  totalRoles: (state: BaseState<Role>) => state.totalItems || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};