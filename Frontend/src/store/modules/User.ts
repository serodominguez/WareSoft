import mainStore from "@/store";
import { jwtDecode } from "jwt-decode";
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

interface DecodedToken {
  exp: number;
  [key: string]: any;
};

interface RootState {
  token: string;
  [key: string]: any;
};

const state: UserState = {
  users: [] as User[],
  selectedUser: null as User | null,
  totalUsers: 0,
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
    { commit, rootState }: any,
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

      const data = await fetchUsersService(
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

      commit("SET_USERS", data.items);
      commit("SET_TOTAL_USERS", data.totalRecords);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async fetchUserById({ commit, rootState }: any, id: number) {
    commit("SET_LOADING", true);
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      const user = await fetchUserByIdService(id, token);
      commit("SET_SELECTED_USER", user);
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async registerUser({ commit, dispatch, rootState }: any, user: User) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      await registerUserService(user, token);
      dispatch("fetchUsers", {});
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async editUser({ commit, dispatch, rootState }: any, { id, user }: { id: number; user: User }) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      await editUserService(id, user, token);
      dispatch("fetchUsers", {});
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async enableUser({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      await enableUserService(id, token);
      dispatch("fetchUsers", {});
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },
  async disableUser({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      await disableUserService(id, token);
      dispatch("fetchUsers", {});
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
  },

  async removeUser({ commit, dispatch, rootState }: any, id: number) {
    try {
      const token = rootState.token;
      if (isExpired(token)) {
        await mainStore.dispatch("logout");
        return;
      }

      await removeUserService(id, token);
      dispatch("fetchUsers", {});
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    }
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