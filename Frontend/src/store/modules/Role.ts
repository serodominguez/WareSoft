import { Role, RoleState } from '@/models/roleModel';
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

const state: RoleState = {
  roles: [] as Role[],
  selectedRole: null as Role | null,
  totalRoles: 0,
  loading: false,
  error: null as string | null,
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
    { commit }: any,
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

      const data = await fetchRolesService(
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

      commit("SET_ROLES", data.items);
      commit("SET_TOTAL_ROLES", data.totalRecords);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async selectRole({ commit }: any) {
    commit("SET_LOADING", true);
    try {
      const roles = await selectRoleService();
      commit("SET_ROLES", roles);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async fetchRoleById({ commit }: any, id: number) {
    commit("SET_LOADING", true);
    try {
      const role = await fetchRoleByIdService(id);
      commit("SET_SELECTED_ROLE", role);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async registerRole({ dispatch }: any, role: Role) {
    await registerRoleService(role);
    dispatch("fetchRoles", {});
  },

  async editRole({ dispatch }: any, { id, role }: { id: number; role: Role }) {
    await editRoleService(id, role);
    dispatch("fetchRoles", {});
  },

  async enableRole({ dispatch }: any, id: number) {
    await enableRoleService(id);
    dispatch("fetchRoles", {});
  },
  async disableRole({ dispatch }: any, id: number) {
    await disableRoleService(id);
    dispatch("fetchRoles", {});
  },

  async removeRole({ dispatch }: any, id: number) {
    await removeRoleService(id);
    dispatch("fetchRoles", {});
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