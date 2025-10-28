import mainStore from "@/store";
import { jwtDecode } from "jwt-decode";
import { Role, RoleState, BaseResponse } from '@/interfaces/roleInterface';
import {
  fetchRolesService,
  selectRoleService,
  fetchRoleByIdService,
  registerRoleService,
  editRoleService,
  enableRoleService,
  disableRoleService,
  removeRoleService,
} from '@/services/roleService';

interface DecodedToken {
  exp: number;
  [key: string]: any;
};

interface RootState {
  token: string;
  [key: string]: any;
};

const state: RoleState = {
  roles: [] as Role[],
  selectedRole: null as Role | null,
  totalRoles: 0,
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
  SET_ROLES(state: any, roles: Role[]) {
    state.roles = roles;
  },
  SET_TOTAL_ROLES(state: any, total: number) {
    state.totalRoles = total;
  },
  SET_SELECTED_ROLE(state: any, role: Role | null) {
    state.selectedRole = role;
  },
  SET_LOADING(state: any, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: any, error: string | null) {
    state.error = error;
  },
};

const actions = {
  async fetchRoles(
    { commit, rootState }: any,
    { 
      pageNumber = 1, 
      pageSize = 10, 
      order = "desc", 
      sort = "PK_ROLE", 
      textFilter = null, 
      numberFilter = null,
      stateFilter = 1,
      startDate = null,
      endDate = null
    } = {}
  ) {
    commit("SET_LOADING", true);
    commit("SET_ROLES", []);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result = await fetchRolesService(
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
        commit("SET_ROLES", result.data);
        commit("SET_TOTAL_ROLES", result.totalRecords);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async downloadRolesExcel(
    { rootState }: any,
    { 
      pageNumber = 1, 
      pageSize = 10, 
      order = "desc", 
      sort = "PK_ROLE", 
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

      const blob = await fetchRolesService(
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
      link.setAttribute('download', `Roles_${new Date().toISOString().split('T')[0]}.xlsx`);
      document.body.appendChild(link);
      link.click();
      
      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },

  async selectRole({ commit, rootState }: any) {
    commit("SET_LOADING", true);
    commit("SET_ROLES", []);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await selectRoleService(token);
      if (result.isSuccess) {
        commit("SET_ROLES", result.data);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async fetchRoleById({ commit, rootState }: any, id: number) {
    commit("SET_LOADING", true);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await fetchRoleByIdService(id, token);
      if (result.isSuccess) {
        commit("SET_SELECTED_ROLE", result.data);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async registerRole({ commit, dispatch, rootState }: any, role: Role) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await registerRoleService(role, token);
      if (result.isSuccess) {
        dispatch("fetchRoles", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async editRole({ commit, dispatch, rootState }: any, { id, role }: { id: number; role: Role }) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await editRoleService(id, role, token);
      if (result.isSuccess) {
        dispatch("fetchRoles", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async enableRole({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await enableRoleService(id, token);
      if (result.isSuccess) {
        dispatch("fetchRoles", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },
  async disableRole({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const result: BaseResponse = await disableRoleService(id, token);
      if (result.isSuccess) {
        dispatch("fetchRoles", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async removeRole({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }
      const result: BaseResponse = await removeRoleService(id, token);
      if (result.isSuccess) {
        dispatch("fetchRoles", {});
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }

  },
};

const getters = {
  roles: (state: any) => state.roles,
  selectedRole: (state: any) => state.selectedRole,
  loading: (state: any) => state.loading,
  error: (state: any) => state.error,
  totalRoles: (state: any) => state.totalRoles || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};

export type { Role };