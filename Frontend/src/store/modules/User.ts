import { User } from '@/interfaces/userInterface';
import { userService } from '@/services/userService';
import { BaseState, FilterParams } from '@/interfaces/baseInterface'

const state: BaseState<User> = {
  items: [] as User[],
  selectedItem: null as User | null,
  totalItems: 0,
  loading: false,
  error: null as string | null,
  lastFilterParams: undefined,
};

const mutations = {
  SET_ITEMS(state: BaseState<User>, items: User[]) {
    state.items = items;
  },
  SET_TOTAL_ITEMS(state: BaseState<User>, total: number) {
    state.totalItems = total;
  },
  SET_SELECTED_ITEMS(state: BaseState<User>, item: User | null) {
    state.selectedItem = item;
  },
  SET_LOADING(state: BaseState<User>, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: BaseState<User>, error: string | null) {
    state.error = error;
  },
  SET_LAST_FILTER_PARAMS(state: BaseState<User>, params: FilterParams) {
    state.lastFilterParams = params;
  },
};

const actions = {
  async fetchUsers({ commit }: any, params: FilterParams = {}) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    commit("SET_LAST_FILTER_PARAMS", params);

    try {
      const result = await userService.fetchAll(params);
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


  async downloadUsersExcel({ state }: any, params?: FilterParams) {
    try {
      const filterParams = params || state.lastFilterParams || {};
      await userService.downloadExcel(filterParams);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },


  async selectUser({ commit }: any) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    
    try {
      const result = await userService.select();
      
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

  async fetchUserById({ commit }: any, id: number) {
    commit("SET_LOADING", true);
    
    try {
      const result = await userService.fetchById(id);
      
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

  async registerUser({ dispatch, state }: any, user: User) {
    try {
      const result = await userService.create(user);
      
      if (result.isSuccess) {
        dispatch("fetchUsers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async editUser({ dispatch, state }: any, { id, user }: { id: number; user: User }) {
    try {
      const result = await userService.update(id, user);
      
      if (result.isSuccess) {
        dispatch("fetchUsers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async enableUser({ dispatch, state }: any, id: number) {
    try {
      const result = await userService.enable(id);
      
      if (result.isSuccess) {
        dispatch("fetchUsers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async disableUser({ dispatch, state }: any, id: number) {
    try {
      const result = await userService.disable(id);
      
      if (result.isSuccess) {
        dispatch("fetchUsers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async removeUser({ dispatch, state }: any, id: number) {
    try {
      const result = await userService.remove(id);
      
      if (result.isSuccess) {
        dispatch("fetchUsers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },
};

const getters = {
  users: (state: BaseState<User>) => state.items,
  selectedUser: (state: BaseState<User>) => state.selectedItem,
  loading: (state: BaseState<User>) => state.loading,
  error: (state: BaseState<User>) => state.error,
  totalUsers: (state: BaseState<User>) => state.totalItems || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};