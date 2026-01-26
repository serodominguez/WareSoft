import { Brand } from '@/interfaces/brandInterface';
import { brandService } from '@/services/brandService';
import { BaseState, FilterParams } from '@/interfaces/baseInterface'

const state: BaseState<Brand> = {
  items: [] as Brand[],
  selectedItem: null as Brand | null,
  totalItems: 0,
  loading: false,
  error: null as string | null,
  lastFilterParams: undefined,
};

const mutations = {
  SET_ITEMS(state: BaseState<Brand>, items: Brand[]) {
    state.items = items;
  },
  SET_TOTAL_ITEMS(state: BaseState<Brand>, total: number) {
    state.totalItems = total;
  },
  SET_SELECTED_ITEMS(state: BaseState<Brand>, item: Brand | null) {
    state.selectedItem = item;
  },
  SET_LOADING(state: BaseState<Brand>, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: BaseState<Brand>, error: string | null) {
    state.error = error;
  },
  SET_LAST_FILTER_PARAMS(state: BaseState<Brand>, params: FilterParams) {
    state.lastFilterParams = params;
  },
};

const actions = {
  // Obtener lista de marcas con filtros
  async fetchBrands({ commit }: any, params: FilterParams = {}) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    commit("SET_LAST_FILTER_PARAMS", params);

    try {
      const result = await brandService.fetchAll(params);
      
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

  // Descargar Excel de marcas
  async downloadBrandsExcel({ state }: any, params?: FilterParams) {
    try {
      // Usar los últimos parámetros de filtro si no se proporcionan nuevos
      const filterParams = params || state.lastFilterParams || {};
      await brandService.downloadExcel(filterParams);
    } catch (error: any) {
      console.error('Error al descargar Excel:', error);
      throw error;
    }
  },

  //Obtener lista para select (sin paginación)
  async selectBrand({ commit }: any) {
    commit("SET_LOADING", true);
    commit("SET_ITEMS", []);
    
    try {
      const result = await brandService.select();
      
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

  // Obtener marca por ID
  async fetchBrandById({ commit }: any, id: number) {
    commit("SET_LOADING", true);
    
    try {
      const result = await brandService.fetchById(id);
      
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

  //Registrar nueva marca
  async registerBrand({ dispatch, state }: any, brand: Brand) {
    try {
      const result = await brandService.create(brand);
      
      if (result.isSuccess) {
        // Recargar lista con los últimos filtros
        dispatch("fetchBrands", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  //Editar marca existente
  async editBrand({ dispatch, state }: any, { id, brand }: { id: number; brand: Brand }) {
    try {
      const result = await brandService.update(id, brand);
      
      if (result.isSuccess) {
        dispatch("fetchBrands", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  // Habilitar marca
  async enableBrand({ dispatch, state }: any, id: number) {
    try {
      const result = await brandService.enable(id);
      
      if (result.isSuccess) {
        dispatch("fetchBrands", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  // Deshabilitar marca
  async disableBrand({ dispatch, state }: any, id: number) {
    try {
      const result = await brandService.disable(id);
      
      if (result.isSuccess) {
        dispatch("fetchBrands", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },

  // Eliminar marca (soft delete)
  async removeBrand({ dispatch, state }: any, id: number) {
    try {
      const result = await brandService.remove(id);
      
      if (result.isSuccess) {
        dispatch("fetchBrands", state.lastFilterParams || {});
      }
      
      return result;
    } catch (error: any) {
      return { isSuccess: false, message: error.message, errors: error };
    }
  },
};

const getters = {
  brands: (state: BaseState<Brand>) => state.items,
  selectedBrand: (state: BaseState<Brand>) => state.selectedItem,
  loading: (state: BaseState<Brand>) => state.loading,
  error: (state: BaseState<Brand>) => state.error,
  totalBrands: (state: BaseState<Brand>) => state.totalItems || 0,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};