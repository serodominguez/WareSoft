import { GoodsIssue, GoodsIssueDetail } from '@/interfaces/goodsIssueInterface';
import { goodsIssueService } from '@/services/goodsIssueService';
import { BaseState, FilterParams } from '@/interfaces/baseInterface'

interface GoodsIssueState extends BaseState<GoodsIssue> {
  selectedIssueDetails: GoodsIssueDetail[];
}

const state: GoodsIssueState = {
  items: [] as GoodsIssue[],
  selectedItem: null as GoodsIssue | null,
  selectedIssueDetails: [] as GoodsIssueDetail[],
  totalItems: 0,
  loading: false,
  error: null as string | null,
  lastFilterParams: undefined,
};

const mutations = {
  SET_ITEMS(state: BaseState<GoodsIssue>, items: GoodsIssue[]) {
    state.items = items;
  },
  SET_TOTAL_ITEMS(state: BaseState<GoodsIssue>, total: number) {
    state.totalItems = total;
  },
  SET_SELECTED_ITEM(state: BaseState<GoodsIssue>, item: GoodsIssue | null) {
    state.selectedItem = item;
  },
  SET_SELECTED_ISSUE_DETAILS(state: GoodsIssueState, details: GoodsIssueDetail[]) {
    state.selectedIssueDetails = details;
  },
  SET_LOADING(state: BaseState<GoodsIssue>, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: BaseState<GoodsIssue>, error: string | null) {
    state.error = error;
  },
  SET_LAST_FILTER_PARAMS(state: BaseState<GoodsIssue>, params: FilterParams) {
    state.lastFilterParams = params;
  },
};

const actions = {
  async fetchGoodsIssue({ commit }: any, params: FilterParams = {}) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    commit("SET_LAST_FILTER_PARAMS", params);

    try {
      const result = await goodsIssueService.fetchAll(params);

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

  async downloadGoodsIssueExcel({ state }: any, params?: FilterParams) {
    try {
      const filterParams = params || state.lastFilterParams || {};
      await goodsIssueService.downloadExcel(filterParams);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },

  async fetchGoodsIssueById({ commit }: any, issueId: number) {
    commit("SET_LOADING", true);

    try {
      const result = await goodsIssueService.getIssueWithDetails(issueId);

      if (result.isSuccess) {
        commit("SET_SELECTED_ITEM", result.data);

        const mappedDetails = result.data.goodsIssueDetails?.map((detail: any) => ({
          idProduct: detail.idProduct,
          code: detail.code,
          description: detail.description,
          material: detail.material,
          color: detail.color,
          categoryName: detail.categoryName,
          brandName: detail.brandName,
          quantity: detail.quantity,
          unitPrice: detail.unitPrice,
          totalPrice: detail.totalPrice
        })) || [];

        commit("SET_SELECTED_ISSUE_DETAILS", mappedDetails);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async registerGoodsIssue({ dispatch, state }: any, issueData: any) {
    try {
      const result = await goodsIssueService.register(issueData);

      if (result.isSuccess) {
        dispatch("fetchGoodsIssue", state.lastFilterParams || {});
      }

      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async disableGoodsIssue({ dispatch, state }: any, issueId: number) {
    try {
      const result = await goodsIssueService.disable(issueId);

      if (result.isSuccess) {
        dispatch("fetchGoodsIssue", state.lastFilterParams || {});
      }

      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async exportGoodsIssuePdf(_: any, issueId: number) {
    try {
      const { blob, filename } = await goodsIssueService.exportPdf(issueId);

      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', filename);
      document.body.appendChild(link);
      link.click();
      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);

      return { isSuccess: true };
    } catch (error: any) {
      console.error('Error al exportar PDF:', error);
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  async openGoodsIssuePdf(_: any, issueId: number) {
    try {
      const { blob, filename } = await goodsIssueService.exportPdf(issueId);
      const url = window.URL.createObjectURL(blob);
      window.open(url, '_blank');
      setTimeout(() => {
        window.URL.revokeObjectURL(url);
      }, 100);

      return { isSuccess: true };
    } catch (error: any) {
      console.error('Error al abrir PDF:', error);
      return { isSuccess: false, message: error.message, errors: error };
    }
  }
};

const getters = {
  goodsissue: (state: BaseState<GoodsIssue>) => state.items,
  selectedGoodsIssue: (state: BaseState<GoodsIssue>) => state.selectedItem,
  selectedIssueDetails: (state: GoodsIssueState) => state.selectedIssueDetails,
  loading: (state: BaseState<GoodsIssue>) => state.loading,
  error: (state: BaseState<GoodsIssue>) => state.error,
  totalGoodsIssue: (state: BaseState<GoodsIssue>) => state.totalItems || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};