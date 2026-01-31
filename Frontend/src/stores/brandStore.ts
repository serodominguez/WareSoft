import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { Brand } from '@/interfaces/brandInterface';
import { brandService } from '@/services/brandService';
import { FilterParams } from '@/interfaces/baseInterface';
import { handleSilentError } from '@/helpers/errorHandler';

export const useBrandStore = defineStore('brand', () => {
  // State
  const items = ref<Brand[]>([]);
  const selectedItem = ref<Brand | null>(null);
  const totalItems = ref(0);
  const loading = ref(false);
  const error = ref<string | null>(null);
  const lastFilterParams = ref<FilterParams | undefined>(undefined);

  // Getters
  const brands = computed(() => items.value);
  const selectedBrand = computed(() => selectedItem.value);
  const totalBrands = computed(() => totalItems.value);

  // Actions
  const fetchBrands = async (params: FilterParams = {}) => {
    loading.value = true;
    items.value = [];
    lastFilterParams.value = params;

    try {
      const result = await brandService.fetchAll(params);
      
      if (result.isSuccess) {
        items.value = result.data;
        totalItems.value = result.totalRecords;
      } else {
        error.value = result.message || result.errors;
      }
    } catch (err: any) {
      error.value = err.message;
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const downloadBrandsExcel = async (params?: FilterParams) => {
    try {
      const filterParams = params || lastFilterParams.value || {};
      await brandService.downloadExcel(filterParams);
    } catch (err: any) {
      console.error('Error al descargar Excel:', err);
      throw err;
    }
  };

  const selectBrand = async () => {
    loading.value = true;
    items.value = [];
    
    try {
      const result = await brandService.select();
      
      if (result.isSuccess) {
        items.value = result.data;
      } else {
        error.value = result.message || result.errors;
      }
    } catch (err: any) {
      error.value = err.message;
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const fetchBrandById = async (id: number) => {
    loading.value = true;
    
    try {
      const result = await brandService.fetchById(id);
      
      if (result.isSuccess) {
        selectedItem.value = result.data;
      } else {
        error.value = result.message || result.errors;
      }
    } catch (err: any) {
      error.value = err.message;
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const registerBrand = async (brand: Brand) => {
    try {
      const result = await brandService.create(brand);
      
      if (result.isSuccess) {
        // Recargar lista con los últimos filtros
        await fetchBrands(lastFilterParams.value || {});
      }
      
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  };

  const editBrand = async (id: number, brand: Brand) => {
    try {
      const result = await brandService.update(id, brand);
      
      if (result.isSuccess) {
        await fetchBrands(lastFilterParams.value || {});
      }
      
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  };

  const enableBrand = async (id: number) => {
    try {
      const result = await brandService.enable(id);
      
      if (result.isSuccess) {
        await fetchBrands(lastFilterParams.value || {});
      }
      
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  };

  const disableBrand = async (id: number) => {
    try {
      const result = await brandService.disable(id);
      
      if (result.isSuccess) {
        await fetchBrands(lastFilterParams.value || {});
      }
      
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  };

  const removeBrand = async (id: number) => {
    try {
      const result = await brandService.remove(id);
      
      if (result.isSuccess) {
        await fetchBrands(lastFilterParams.value || {});
      }
      
      return result;
    } catch (err: any) {
      return { isSuccess: false, message: err.message, errors: err };
    }
  };

  return {
    // State
    items,
    selectedItem,
    totalItems,
    loading,
    error,
    lastFilterParams,
    
    // Getters
    brands,
    selectedBrand,
    totalBrands,
    
    // Actions
    fetchBrands,
    downloadBrandsExcel,
    selectBrand,
    fetchBrandById,
    registerBrand,
    editBrand,
    enableBrand,
    disableBrand,
    removeBrand,
  };
});