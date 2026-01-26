import { fetchPermissionsByRole, updatePermissions } from '@/services/permissionService';
import { Permission, PermissionsByModule } from '@/interfaces/permissionInterface';
import { handleSilentError } from '@/helpers/errorHandler';

interface PermissionState {
  permissions: Permission[];
  loading: boolean;
  error: string | null;
}

const state: PermissionState = {
  permissions: [],
  loading: false,
  error: null,
};

const mutations = {
  SET_PERMISSIONS(state: PermissionState, permissions: Permission[]) {
    state.permissions = permissions;
  },
  SET_LOADING(state: PermissionState, loading: boolean) {
    state.loading = loading;
  },
  SET_ERROR(state: PermissionState, error: string | null) {
    state.error = error;
  },
};

const actions = {
  async fetchPermissionsByRole({ commit }: any, roleId: number) {
    commit("SET_LOADING", true);
    commit("SET_ERROR", null);
    try {
      const response = await fetchPermissionsByRole(roleId);
      if (response.isSuccess) {
        commit("SET_PERMISSIONS", response.data);
      } else {
        commit("SET_ERROR", response.message);
      }
    } catch (error: any) {
      const appError = handleSilentError(error);
      commit("SET_ERROR", appError.message);
      throw error;
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async updatePermissions({ commit }: any, updatedPermissions: Array<{ idPermission: number; status: boolean }>) {
    try {
      const result = await updatePermissions(updatedPermissions);
      if (result.isSuccess) {
        return { success: true, message: result.message };
      } else {
        commit("SET_ERROR", result.message || result.errors);
        return {
          success: false,
          message: result.message || "Error al actualizar permisos",
        };
      }
    } catch (error: any) {
      const appError = handleSilentError(error);
      commit("SET_ERROR", appError.message);
      throw error;
    }
  },
};

const getters = {
  permissions: (state: PermissionState) => state.permissions,
  loading: (state: PermissionState) => state.loading,
  error: (state: PermissionState) => state.error,

  permissionsByModule: (state: PermissionState): PermissionsByModule[] => {
    const grouped = state.permissions.reduce((acc, perm) => {
      if (!acc[perm.moduleName]) {
        acc[perm.moduleName] = {
          module: perm.moduleName,
          permissions: {
            crear: false,
            leer: false,
            editar: false,
            eliminar: false,
          },
        };
      }

      const actionKey = perm.actionName.toLowerCase() as
        | "crear"
        | "leer"
        | "editar"
        | "eliminar";
      acc[perm.moduleName].permissions[actionKey] = perm.status;

      return acc;
    }, {} as Record<string, PermissionsByModule>);

    return Object.values(grouped);
  },
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};