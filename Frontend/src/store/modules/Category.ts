import axios from 'axios';
import { Category, CategoryState } from '@/store/interfaces/ICategory';


// Estado inicial
const state: CategoryState = {
  categories: [],
  selectedCategory: null,
  loading: false,
  error: null,
};

// Mutations
const mutations = {
  SET_CATEGORIES(state: CategoryState, categories: Category[]) {
    state.categories = categories;
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
};

// Actions
const actions = {
  async fetchCategories({ commit }: any) {
    commit('SET_LOADING', true);
    try {
      const response = await axios.get<Category[]>('api/Categories');
      commit('SET_CATEGORIES', response.data);
    } catch (error: any) {
      commit('SET_ERROR', error.message || 'Error desconocido');
    } finally {
      commit('SET_LOADING', false);
    }
  },

  async createCategory({ dispatch }: any, category: Omit<Category, 'id'>) {
    await axios.post('api/Categories/Register', category);
    dispatch('fetchCategories');
  },

  async updateCategory({ dispatch }: any, category: Category) {
    await axios.put('api/Categories/Edit', category);
    dispatch('fetchCategories');
  },

  async enabledCategorie({ dispatch }: any, id: number) {
    await axios.put(`api/Categories/Enabled/${id}`, { enabled: true });
    dispatch('fetchCategories');
  },

  async disabledCategorie({ dispatch }: any, id: number) {
    await axios.put(`api/Categories/Disabled/${id}`, { enabled: false });
    dispatch('fetchCategories');
  },
};

// Getters
const getters = {
  categories: (state: CategoryState) => state.categories,
  selectedCategory: (state: CategoryState) => state.selectedCategory,
  loading: (state: CategoryState) => state.loading,
  error: (state: CategoryState) => state.error,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};