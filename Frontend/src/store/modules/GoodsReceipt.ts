import { GoodsReceipt, GoodsReceiptDetail } from '@/interfaces/goodsReceiptInterface';
import { goodsReceiptService } from '@/services/goodsReceiptService';
import { BaseState, FilterParams } from '@/interfaces/baseInterface'

interface GoodsReceiptState extends BaseState<GoodsReceipt> {
  selectedReceiptDetails: GoodsReceiptDetail[];
}

const state: GoodsReceiptState = {
  items: [] as GoodsReceipt[],
  selectedItem: null as GoodsReceipt | null,
  selectedReceiptDetails: [] as GoodsReceiptDetail[],
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
  SET_SELECTED_ITEM(state: BaseState<GoodsReceipt>, item: GoodsReceipt | null) {
    state.selectedItem = item;
  },
  SET_SELECTED_RECEIPT_DETAILS(state: GoodsReceiptState, details: GoodsReceiptDetail[]) {
    state.selectedReceiptDetails = details;
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
  // Obtener lista de entradas
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

  // Descargar Excel
  async downloadGoodsReceiptExcel({ state }: any, params?: FilterParams) {
    try {
      const filterParams = params || state.lastFilterParams || {};
      await goodsReceiptService.downloadExcel(filterParams);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },

  // Obtener entrada por ID con detalles
  async fetchGoodsReceiptById({ commit }: any, receiptId: number) {
    commit("SET_LOADING", true);

    try {
      const result = await goodsReceiptService.getReceiptWithDetails(receiptId);

      if (result.isSuccess) {
        commit("SET_SELECTED_ITEM", result.data);

        // Mapear los detalles al formato esperado por el componente
        const mappedDetails = result.data.goodsReceiptDetails?.map((detail: any) => ({
          idProduct: detail.idProduct,
          code: detail.code,
          description: detail.description,
          material: detail.material,
          color: detail.color,
          categoryName: detail.categoryName,
          brandName: detail.brandName,
          quantity: detail.quantity,
          unitCost: detail.unitCost,
          totalCost: detail.totalCost
        })) || [];

        commit("SET_SELECTED_RECEIPT_DETAILS", mappedDetails);
      } else {
        commit("SET_ERROR", result.message || result.errors);
      }
    } catch (error: any) {
      commit("SET_ERROR", error.message);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  // Registrar nueva entrada
  async registerGoodsReceipt({ dispatch, state }: any, receiptData: any) {
    try {
      const result = await goodsReceiptService.register(receiptData);

      if (result.isSuccess) {
        // Recargar lista con los últimos filtros
        dispatch("fetchGoodsReceipt", state.lastFilterParams || {});
      }

      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  // Anular entrada
  async disableGoodsReceipt({ dispatch, state }: any, receiptId: number) {
    try {
      const result = await goodsReceiptService.disable(receiptId);

      if (result.isSuccess) {
        // Recargar lista con los últimos filtros
        dispatch("fetchGoodsReceipt", state.lastFilterParams || {});
      }

      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  // Exportar PDF
  async exportGoodsReceiptPdf(_: any, receiptId: number) {
    try {
      const { blob, filename } = await goodsReceiptService.exportPdf(receiptId);

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

  // Abrir PDF en nueva pestaña
  async openGoodsReceiptPdf(_: any, receiptId: number) {
    try {
      const { blob, filename } = await goodsReceiptService.exportPdf(receiptId);

      // Crear URL del blob
      const url = window.URL.createObjectURL(blob);

      // Abrir en nueva pestaña
      window.open(url, '_blank');

      // Limpiar URL después de un tiempo
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
  goodsreceipt: (state: BaseState<GoodsReceipt>) => state.items,
  selectedGoodsReceipt: (state: BaseState<GoodsReceipt>) => state.selectedItem,
  selectedReceiptDetails: (state: GoodsReceiptState) => state.selectedReceiptDetails,
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