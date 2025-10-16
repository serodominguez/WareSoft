import { User, UserState } from '@/models/userModel';
import {
  fetchUsersService,
  fetchUserByIdService,
  registerUserService,
  editUserService,
  enableUserService,
  disableUserService,
  removeUserService,
} from '@/services/userService';

const state: UserState = {
  users: [] as User[],
  selectedUser: null as User | null,
  totalUsers: 0,
  loading: false,
  error: null as string | null,
};

const mutations = {
  SET_USERS(state: any, users: User[]) {
    state.users = users;
  },
  SET_TOTAL_USERS(state: any, total: number) {
    state.totalUsers = total;
  },
  SET_SELECTED_USER(state: any, user: User | null) {
    state.selectedUser = user;
  },
  SET_LOADING(state: any, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: any, error: string | null) {
    state.error = error;
  },
};

const actions = {
  async fetchUsers(
    { commit }: any,
    { 
      pageNumber = 1, 
      pageSize = 10, 
      order = "desc", 
      sort = "PK_USER", 
      textFilter = null, 
      numberFilter = null,
      stateFilter = 1,
      startDate = null,
      endDate = null
    } = {}
  ) {
    commit("SET_LOADING", true);
    commit("SET_USERS", []);
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

      const data = await fetchUsersService(
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

      commit("SET_USERS", data.items);
      commit("SET_TOTAL_USERS", data.totalRecords);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async fetchUserById({ commit }: any, id: number) {
    commit("SET_LOADING", true);
    try {
      const user = await fetchUserByIdService(id);
      commit("SET_SELECTED_USER", user);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async registerUser({ dispatch }: any, user: User) {
    await registerUserService(user);
    dispatch("fetchUsers", {});
  },

  async editUser({ dispatch }: any, { id, user }: { id: number; user: User }) {
    await editUserService(id, user);
    dispatch("fetchUsers", {});
  },

  async enableUser({ dispatch }: any, id: number) {
    await enableUserService(id);
    dispatch("fetchUsers", {});
  },
  async disableUser({ dispatch }: any, id: number) {
    await disableUserService(id);
    dispatch("fetchUsers", {});
  },

  async removeUser({ dispatch }: any, id: number) {
    await removeUserService(id);
    dispatch("fetchUsers", {});
  },
};

const getters = {
  users: (state: any) => state.users,
  selectedUser: (state: any) => state.selectedUser,
  loading: (state: any) => state.loading,
  error: (state: any) => state.error,
  totalUsers: (state: any) => state.totalUsers || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};

export type { User };