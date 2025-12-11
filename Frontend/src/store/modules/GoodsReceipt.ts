import { GoodsReceipt } from '@/interfaces/goodsReceiptInterface';
import { goodsReceiptService } from '@/services/goodsReceiptService';
import { BaseState, FilterParams } from '@/interfaces/baseInterface'

const state: BaseState<GoodsReceipt> = {
  items: [] as GoodsReceipt[],
  selectedItem: null as GoodsReceipt | null,
  totalItems: 0,
  loading: false,
  error: null as string | null,
  lastFilterParams: undefined,
};

const mutations = {
  SET_ITEMS(state: BaseState<GoodsReceipt>, items: GoodsReceipt[]) {
    state.items = items;
  },
  SET_TOTAL_ITEMS(state: BaseState<GoodsReceipt>, total: number) {
    state.totalItems = total;
  },
  SET_SELECTED_ITEMS(state: BaseState<GoodsReceipt>, item: GoodsReceipt | null) {
    state.selectedItem = item;
  },
  SET_LOADING(state: BaseState<GoodsReceipt>, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: BaseState<GoodsReceipt>, error: string | null) {
    state.error = error;
  },
  SET_LAST_FILTER_PARAMS(state: BaseState<GoodsReceipt>, params: FilterParams) {
    state.lastFilterParams = params;
  },
};

const actions = {
  async fetchGoodsReceipt({ commit }: any, params: FilterParams = {}) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    commit("SET_LAST_FILTER_PARAMS", params);

    try {
      const result = await goodsReceiptService.fetchAll(params);
      
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

  async downloadGoodsReceiptExcel({ state }: any, params?: FilterParams) {
    try {
      const filterParams = params || state.lastFilterParams || {};
      await goodsReceiptService.downloadExcel(filterParams);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },
};

const getters = {
  goodsreceipt: (state: BaseState<GoodsReceipt>) => state.items,
  selectedGoodsReceipt: (state: BaseState<GoodsReceipt>) => state.selectedItem,
  loading: (state: BaseState<GoodsReceipt>) => state.loading,
  error: (state: BaseState<GoodsReceipt>) => state.error,
  totalGoodsReceipt: (state: BaseState<GoodsReceipt>) => state.totalItems || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};