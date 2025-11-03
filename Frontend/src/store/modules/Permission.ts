
import { Module } from 'vuex';
import { fetchPermissionsByRole } from '@/services/permissionService';
import { Permission, PermissionsByModule } from '@/interfaces/permissionInterface';

interface PermissionState {
  permissions: Permission[];
  loading: boolean;
  error: string | null;
}

const permissionModule: Module<PermissionState, any> = {
  namespaced: true,
  
  state: {
    permissions: [],
    loading: false,
    error: null,
  },
  
  mutations: {
    SET_PERMISSIONS(state, permissions: Permission[]) {
      state.permissions = permissions;
    },
    SET_LOADING(state, loading: boolean) {
      state.loading = loading;
    },
    SET_ERROR(state, error: string | null) {
      state.error = error;
    },
  },
  
  actions: {
    async fetchPermissionsByRole({ commit }, roleId: number) {
      commit('SET_LOADING', true);
      commit('SET_ERROR', null);
      try {
        const response = await fetchPermissionsByRole(roleId);
        if (response.isSuccess) {
          commit('SET_PERMISSIONS', response.data);
        } else {
          commit('SET_ERROR', response.message);
        }
      } catch (error: any) {
        commit('SET_ERROR', error.message || 'Error al cargar permisos');
      } finally {
        commit('SET_LOADING', false);
      }
    },
  },
  
  getters: {
    permissions: (state) => state.permissions,
    loading: (state) => state.loading,
    error: (state) => state.error,
    
    permissionsByModule: (state): PermissionsByModule[] => {
      const grouped = state.permissions.reduce((acc, perm) => {
        if (!acc[perm.modulE_NAME]) {
          acc[perm.modulE_NAME] = {
            module: perm.modulE_NAME,
            permissions: {
              crear: false,
              leer: false,
              editar: false,
              eliminar: false,
            },
          };
        }
        
        const actionKey = perm.actioN_NAME.toLowerCase() as 'crear' | 'leer' | 'editar' | 'eliminar';
        acc[perm.modulE_NAME].permissions[actionKey] = perm.state;
        
        return acc;
      }, {} as Record<string, PermissionsByModule>);
      
      return Object.values(grouped);
    },
  },
};

export default permissionModule;