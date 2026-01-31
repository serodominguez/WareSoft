import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { Inventory } from '@/interfaces/inventoryInterface';
import { inventoryService } from '@/services/inventoryService';
import { FilterParams } from '@/interfaces/baseInterface';
import { titleCase } from '@/utils/string';
import { useAuthStore } from '@/stores/auth';

export const useInventoryStore = defineStore('inventory', () => {
  const items = ref<Inventory[]>([]);
  const selectedItem = ref<Inventory | null>(null);
  const totalItems = ref<number>(0);
  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  const inventories = computed(() => items.value);
  const selectedInventory = computed(() => selectedItem.value);
  const totalInventories = computed(() => totalItems.value || 0);

  async function fetchInventories(params: FilterParams = {}) {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const resultado = await inventoryService.fetchAll(params);
      if (resultado.isSuccess) {
        items.value = resultado.data;
        totalItems.value = resultado.totalRecords;
      } else {
        error.value = resultado.message || resultado.errors;
      }
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function downloadInventoriesExcel(params?: FilterParams) {
    try {
      const authStore = useAuthStore();
      const filtrosParams = params || lastFilterParams.value || {};
      const nombreSucursal = titleCase(authStore.currentUser?.storeName || 'Sucursal');
      await inventoryService.downloadExcel(filtrosParams, nombreSucursal);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  }

  async function downloadInventoriesPdf(params?: FilterParams) {
    try {
      const authStore = useAuthStore();
      const filtrosParams = params || lastFilterParams.value || {};
      const nombreSucursal = titleCase(authStore.currentUser?.storeName || 'Sucursal');
      await inventoryService.downloadPdf(filtrosParams, nombreSucursal);
    } catch (err: any) {
      console.error('Error al descargar PDF:', err);
      throw err;
    }
  }

  async function editInventoryPrice(inventory: Inventory) {
    try {
      const resultado = await inventoryService.updatePrice(inventory);
      if (resultado.isSuccess) {
        await fetchInventories(lastFilterParams.value || {});
      }
      return resultado;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  }

  return {
    items,
    selectedItem,
    totalItems,
    loading,
    error,
    lastFilterParams,

    inventories,
    selectedInventory,
    totalInventories,

    fetchInventories,
    downloadInventoriesExcel,
    downloadInventoriesPdf,
    editInventoryPrice,
  };
});