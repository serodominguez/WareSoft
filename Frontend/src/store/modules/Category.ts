import axios from 'axios';
import { Category, CategoryState, PaginatedResponse } from '../interfaces/ICategory';

const state: CategoryState = {
  categories: [],
  totalRecords: 0,
  currentPage: 1,
  itemsPerPage: 10,
  selectedCategory: null,
  loading: false,
  error: null,
};

const mutations = {
  SET_CATEGORIES(state: CategoryState, payload: { items: Category[]; totalRecords: number }) {
    state.categories = payload.items;
    state.totalRecords = payload.totalRecords;
  },
  SET_CURRENT_PAGE(state: CategoryState, page: number) {
    state.currentPage = page;
  },
  SET_ITEMS_PER_PAGE(state: CategoryState, itemsPerPage: number) {
    state.itemsPerPage = itemsPerPage;
  },
  SET_SELECTED_CATEGORY(state: CategoryState, category: Category | null) {
    state.selectedCategory = category;
  },
  SET_LOADING(state: CategoryState, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: CategoryState, error: string | null) {
    state.error = error;
  },
  RESET_PAGINATION(state: CategoryState) {
    state.currentPage = 1;
    state.categories = [];
    state.totalRecords = 0;
  },
};

const actions = {
  async fetchCategories({ commit, state }: { commit: any; state: CategoryState }, paginationParams?: Partial<{ page: number; itemsPerPage: number; sort: string; order: 'asc' | 'desc' }>) {
    commit('SET_LOADING', true);
    commit('SET_ERROR', null);
    try {
      const page = paginationParams?.page || state.currentPage;
      const itemsPerPage = paginationParams?.itemsPerPage || state.itemsPerPage;
      const sort = paginationParams?.sort || 'PK_CATEGORY';
      const order = paginationParams?.order || 'asc';

      const requestBody = {
        numberPage: page,
        numberRecordsPage: itemsPerPage,
        order,
        sort,
      };

      const response = await axios.post<PaginatedResponse>('api/Categories', requestBody);
      if (response.data.isSuccess) {
        commit('SET_CATEGORIES', {
          items: response.data.data.items,
          totalRecords: response.data.data.totalRecords,
        });
        commit('SET_CURRENT_PAGE', page);
        commit('SET_ITEMS_PER_PAGE', itemsPerPage);
      } else {
        commit('SET_ERROR', response.data.message || 'Error en la consulta');
      }
    } catch (error: any) {
      commit('SET_ERROR', error.message || 'Error desconocido');
    } finally {
      commit('SET_LOADING', false);
    }
  },

  async updatePagination({ dispatch }: { dispatch: any }, params: { page: number; itemsPerPage: number; sort?: string; order?: 'asc' | 'desc' }) {
    await dispatch('fetchCategories', params);
  },

  setCurrentPage({ commit }: { commit: any }, page: number) {
    commit('SET_CURRENT_PAGE', page);
  },

  async resetPagination({ commit, dispatch }: { commit: any; dispatch: any }) {
    commit('RESET_PAGINATION');
    await dispatch('fetchCategories');
  },

  async createCategory({ dispatch }: { dispatch: any }, category: Omit<Category, 'pK_CATEGORY' | 'audiT_CREATE_DATE' | 'statE_CATEGORY'>) {
    await axios.post('api/Categories/Register', category);
    await dispatch('fetchCategories');
  },

  async updateCategory({ dispatch }: { dispatch: any }, category: Category) {
    if (category.pK_CATEGORY === null) {
      throw new Error('No se puede actualizar una categoría sin ID');
    }
    await axios.put('api/Categories/Edit', category);
    await dispatch('fetchCategories');
  },

  async enabledCategorie({ dispatch }: { dispatch: any }, id: number) {
    await axios.put(`api/Categories/Enabled/${id}`, { enabled: true });
    await dispatch('fetchCategories');
  },

  async disabledCategorie({ dispatch }: { dispatch: any }, id: number) {
    await axios.put(`api/Categories/Disabled/${id}`, { enabled: false });
    await dispatch('fetchCategories');
  },
};

const getters = {
  categories: (state: CategoryState) => state.categories,
  totalRecords: (state: CategoryState) => state.totalRecords,
  currentPage: (state: CategoryState) => state.currentPage,
  itemsPerPage: (state: CategoryState) => state.itemsPerPage,
  loading: (state: CategoryState) => state.loading,
  error: (state: CategoryState) => state.error,
  selectedCategory: (state: CategoryState) => state.selectedCategory,
  totalPages: (state: CategoryState) => Math.ceil(state.totalRecords / state.itemsPerPage),
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};
