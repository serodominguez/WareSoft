import { Customer } from '@/interfaces/customerInterface';
import { customerService } from '@/services/customerService';
import { BaseState, FilterParams } from '@/interfaces/baseInterface'

const state: BaseState<Customer> = {
  items: [] as Customer[],
  selectedItem: null as Customer | null,
  totalItems: 0,
  loading: false,
  error: null as string | null,
  lastFilterParams: undefined,
};

const mutations = {
  SET_ITEMS(state: BaseState<Customer>, items: Customer[]) {
    state.items = items;
  },
  SET_TOTAL_ITEMS(state: BaseState<Customer>, total: number) {
    state.totalItems = total;
  },
  SET_SELECTED_ITEMS(state: BaseState<Customer>, item: Customer | null) {
    state.selectedItem = item;
  },
  SET_LOADING(state: BaseState<Customer>, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: BaseState<Customer>, error: string | null) {
    state.error = error;
  },
  SET_LAST_FILTER_PARAMS(state: BaseState<Customer>, params: FilterParams) {
    state.lastFilterParams = params;
  },
};

const actions = {
  async fetchCustomers({ commit }: any, params: FilterParams = {}) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    commit("SET_LAST_FILTER_PARAMS", params);

    try {
      const result = await customerService.fetchAll(params);
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


  async downloadCustomersExcel({ state }: any, params?: FilterParams) {
    try {
      const filterParams = params || state.lastFilterParams || {};
      await customerService.downloadExcel(filterParams);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },

  async fetchCustomerById({ commit }: any, id: number) {
    commit("SET_LOADING", true);
    
    try {
      const result = await customerService.fetchById(id);
      
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

  async registerCustomer({ dispatch, state }: any, customer: Customer) {
    try {
      const result = await customerService.create(customer);
      
      if (result.isSuccess) {
        dispatch("fetchCustomers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async editCustomer({ dispatch, state }: any, { id, customer }: { id: number; customer: Customer }) {
    try {
      const result = await customerService.update(id, customer);
      
      if (result.isSuccess) {
        dispatch("fetchCustomers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async enableCustomer({ dispatch, state }: any, id: number) {
    try {
      const result = await customerService.enable(id);
      
      if (result.isSuccess) {
        dispatch("fetchCustomers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async disableCustomer({ dispatch, state }: any, id: number) {
    try {
      const result = await customerService.disable(id);
      
      if (result.isSuccess) {
        dispatch("fetchCustomers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async removeCustomer({ dispatch, state }: any, id: number) {
    try {
      const result = await customerService.remove(id);
      
      if (result.isSuccess) {
        dispatch("fetchCustomers", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },
};

const getters = {
  customers: (state: BaseState<Customer>) => state.items,
  selectedCustomer: (state: BaseState<Customer>) => state.selectedItem,
  loading: (state: BaseState<Customer>) => state.loading,
  error: (state: BaseState<Customer>) => state.error,
  totalCustomers: (state: BaseState<Customer>) => state.totalItems || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};