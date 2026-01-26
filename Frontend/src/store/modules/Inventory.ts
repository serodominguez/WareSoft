import { Inventory} from '@/interfaces/inventoryInterface';
import { inventoryService } from '@/services/inventoryService';
import { BaseState, FilterParams } from '@/interfaces/baseInterface'
import { titleCase } from '@/utils/string';

const state: BaseState<Inventory> = {
  items: [] as Inventory[],
  selectedItem: null as Inventory | null,
  totalItems: 0,
  loading: false,
  error: null as string | null,
  lastFilterParams: undefined,
};

const mutations = {
  SET_ITEMS(state: BaseState<Inventory>, items: Inventory[]) {
    state.items = items;
  },
  SET_TOTAL_ITEMS(state: BaseState<Inventory>, total: number) {
    state.totalItems = total;
  },
  SET_SELECTED_ITEMS(state: BaseState<Inventory>, item: Inventory | null) {
    state.selectedItem = item;
  },
  SET_LOADING(state: BaseState<Inventory>, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: BaseState<Inventory>, error: string | null) {
    state.error = error;
  },
  SET_LAST_FILTER_PARAMS(state: BaseState<Inventory>, params: FilterParams) {
    state.lastFilterParams = params;
  },
};

const actions = {
  async fetchInventories({ commit }: any, params: FilterParams = {}) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    commit("SET_LAST_FILTER_PARAMS", params);

    try {
      const result = await inventoryService.fetchAll(params);
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


  async downloadInventoriesExcel({ state, rootState }: any, params?: FilterParams) {
    try {
      const filterParams = params || state.lastFilterParams || {};
      const storeName = rootState.currentUser?.storeName || 'Sucursal';
      const titleStoreName = titleCase(storeName);
      await inventoryService.downloadExcel(filterParams, titleStoreName);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },
  async downloadInventoriesPdf({ state, rootState }: any, params?: FilterParams) {
    try {
      const filterParams = params || state.lastFilterParams || {};
      const storeName = rootState.currentUser?.storeName || 'Sucursal';
      const titleStoreName = titleCase(storeName);
      await inventoryService.downloadPdf(filterParams, titleStoreName);
    } catch (error: any) {
      console.error('Error al descargar PDF:', error);
      throw error;
    }
  },
  async editInventoryPrice({ dispatch, state }: any, { inventory }: { inventory: Inventory }) {
    try {
      const result = await inventoryService.updatePrice(inventory);
      
      if (result.isSuccess) {
        dispatch("fetchInventories", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },
};

const getters = {
  inventories: (state: BaseState<Inventory>) => state.items,
  selectedInventory: (state: BaseState<Inventory>) => state.selectedItem,
  loading: (state: BaseState<Inventory>) => state.loading,
  error: (state: BaseState<Inventory>) => state.error,
  totalInventories: (state: BaseState<Inventory>) => state.totalItems || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};