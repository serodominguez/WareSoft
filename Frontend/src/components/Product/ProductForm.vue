<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localProduct.idProduct ? 'Editar Producto' : 'Agregar Producto' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="formRef" v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="12" md="12" lg="12" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localProduct.code" counter="25"
                  :maxlength="25" label="Código" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localProduct.description"
                  :rules="[rules.required]" counter="50" :maxlength="50" label="Descripción" required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localProduct.material"
                  :rules="[rules.onlyLetters]" counter="25" :maxlength="25" label="Material" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localProduct.color"
                  :rules="[rules.onlyLetters]" counter="20" :maxlength="20" label="Color" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-text-field color="indigo" variant="underlined" v-model="localProduct.unitMeasure" counter="15"
                  :rules="[rules.required]" :maxlength="15" label="Unidad de Medida" required />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-autocomplete color="indigo" variant="underlined" :items="brands" v-model="localProduct.idBrand"
                  item-title="brandName" item-value="idBrand" :rules="[rules.required]"
                  no-data-text="No hay datos disponibles" label="Marca" required :loading="loadingBrands" />
              </v-col>
              <v-col cols="6" md="6" lg="6" xl="12">
                <v-autocomplete color="indigo" variant="underlined" :items="categories"
                  v-model="localProduct.idCategory" item-title="categoryName" item-value="idCategory"
                  :rules="[rules.required]" no-data-text="No hay datos disponibles" label="Categoría" required
                  :loading="loadingCategories" />
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-col xs12 sm12 md12 lg12 xl12>
        <v-card-actions>
          <v-btn color="green" dark class="mb-2" elevation="4" @click="saveProduct" :disabled="!valid"
            :loading="saving">Guardar</v-btn>
          <v-btn color="red" dark class="mb-2" elevation="4" @click="close">Cancelar</v-btn>
        </v-card-actions>
      </v-col>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { Product } from '@/interfaces/productInterface';
import { handleApiError } from '@/helpers/errorHandler';

interface FormRef {
  validate: () => Promise<{ valid: boolean }>;
}

interface Props {
  modelValue: boolean;
  product?: Product | null;
}

const props = withDefaults(defineProps<Props>(), {
  product: () => ({
    idProduct: null,
    code: '',
    description: '',
    material: '',
    color: '',
    unitMeasure: '',
    idBrand: null,
    brandName: '',
    idCategory: null,
    categoryName: '',
    auditCreateDate: '',
    statusProduct: ''
  })
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'saved': [product: Product];
}>();

const store = useStore();
const toast = useToast();

const formRef = ref<FormRef | null>(null);
const isOpen = ref(props.modelValue);
const valid = ref(false);
const saving = ref(false);
const localProduct = ref<Product>({ ...props.product } as Product);

const rules = {
  required: (value: string) => !!value || 'Este campo es requerido.',
  onlyLetters: (value: string) => !value || /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(value) || 'Solo se permiten letras.',
  onlyNumbers: (value: string) => !value || /^[0-9]+$/.test(value) || 'Solo se permiten números.',
};

const brands = computed(() => {
  const brandsFromStore = store.getters['brand/brands'];
  return Array.isArray(brandsFromStore) ? brandsFromStore : [];
});

const loadingBrands = computed((): boolean => store.getters['brand/loading']);

const categories = computed(() => {
  const categoriesFromStore = store.getters['category/categories'];
  return Array.isArray(categoriesFromStore) ? categoriesFromStore : [];
});

const loadingCategories = computed((): boolean => store.getters['category/loading']);

watch(() => props.modelValue, (newValue: boolean) => {
  isOpen.value = newValue;
  if (newValue) {
    store.dispatch('brand/selectBrand');
    store.dispatch('category/selectCategory');
  }
});

watch(isOpen, (newValue: boolean) => {
  emit('update:modelValue', newValue);
});

watch(() => props.product, (newProduct) => {
  if (newProduct) {
    localProduct.value = { ...newProduct } as Product;
  }
}, { deep: true });

const close = () => {
  isOpen.value = false;
};

const saveProduct = async () => {
  if (!formRef.value) {
    toast.warning('Error al acceder al formulario');
    return;
  }

  const validation = await formRef.value.validate();
  
  if (!validation.valid) {
    toast.warning('Por favor completa todos los campos requeridos');
    return;
  }

  saving.value = true;

  try {
    const isEditing = !!localProduct.value.idProduct;
    let result;

    if (isEditing) {
      result = await store.dispatch('product/editProduct', {
        id: localProduct.value.idProduct,
        product: { ...localProduct.value }
      });
    } else {
      result = await store.dispatch('product/registerProduct', { ...localProduct.value });
    }

    if (result.isSuccess) {
      const successMsg = isEditing
        ? 'Producto actualizado con éxito!'
        : 'Producto registrado con éxito!';

      toast.success(successMsg);
      emit('saved', { ...localProduct.value });
      close();
    }

  } catch (error: any) {
    const isEditing = !!localProduct.value.idProduct;
    const customMessage = isEditing
      ? 'Error en actualizar el producto'
      : 'Error en guardar el producto';

    handleApiError(error, customMessage);
  } finally {
    saving.value = false;
  }
};
</script>