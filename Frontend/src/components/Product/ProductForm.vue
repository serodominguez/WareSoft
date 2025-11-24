<template>
  <v-dialog v-model="isOpen" max-width="500px" persistent>
    <v-card>
      <v-card-title class="bg-surface-light pt-4">
        <span>{{ localProduct.idProduct ? 'Editar Producto' : 'Agregar Producto' }}</span>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-form ref="form" v-model="valid">
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

<script lang="ts">
import { Store as VuexStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { defineComponent, PropType } from 'vue';
import { Product } from '@/interfaces/productInterface';
import { handleApiError } from '@/helpers/errorHandler';

interface FormRef {
  validate: () => boolean;
}

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $store: VuexStore<any>;
  }
}

export default defineComponent({
  props: {
    modelValue: {
      type: Boolean,
      required: true,
    },
    product: {
      type: Object as PropType<Product | null>,
      default: () => ({
        idProduct: null,
        code: '',
        description: '',
        material: '',
        color: '',
        unitMeasure: '',
        idBrand: null,
        idCategory: null,
        auditCreateDate: '',
        statusProduct: ''
      }),
    },
  },
  data() {
    return {
      isOpen: this.modelValue,
      valid: false,
      saving: false,
      localProduct: { ...this.product } as Product,
      toast: useToast(),
      rules: {
        required: (value: string) => !!value || 'Este campo es requerido.',
        onlyLetters: (value: string) => !value || /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(value) || 'Solo se permiten letras.',
        onlyNumbers: (value: string) => !value || /^[0-9]+$/.test(value) || 'Solo se permiten números.',
      },
    };
  },
  computed: {
    brands() {
      const brandsFromStore = this.$store.getters['brand/brands'];
      return Array.isArray(brandsFromStore) ? brandsFromStore : [];
    },
    loadingBrands(): boolean {
      return this.$store.getters['brand/loading'];
    },
    categories() {
      const categoriesFromStore = this.$store.getters['category/categories'];
      return Array.isArray(categoriesFromStore) ? categoriesFromStore : [];
    },
    loadingCategories(): boolean {
      return this.$store.getters['category/loading'];
    },
  },
  watch: {
    modelValue(newValue: boolean) {
      this.isOpen = newValue;
      if (newValue) {
        this.$store.dispatch('brand/selectBrand');
        this.$store.dispatch('category/selectCategory');
      }
    },
    isOpen(newValue: boolean) {
      this.$emit('update:modelValue', newValue);
    },
    product: {
      handler(newProduct: Product) {
        this.localProduct = { ...newProduct };
      },
      deep: true,
    },
  },
  methods: {
    close() {
      this.isOpen = false;
    },
    async saveProduct() {
      const form = this.$refs.form as FormRef;
      if (!form.validate()) {
        this.toast.warning('Por favor completa todos los campos requeridos');
        return;
      }
      try {
        const isEditing = !!this.localProduct.idProduct;
        let result;

        if (isEditing) {
          result = await this.$store.dispatch('product/editProduct', {
            id: this.localProduct.idProduct,
            product: { ...this.localProduct }
          });
        } else {
          result = await this.$store.dispatch('product/registerProduct', { ...this.localProduct });
        }

        if (result.isSuccess) {
          const successMsg = isEditing
            ? 'Producto actualizado con éxito!'
            : 'Producto registrado con éxito!';

          this.toast.success(successMsg);
          this.$emit('saved', { ...this.localProduct });
          this.close();
        }

      } catch (error: any) {
        const isEditing = !!this.localProduct.idProduct;
        const customMessage = isEditing
          ? 'Error en actualizar el producto'
          : 'Error en guardar el producto';
        handleApiError(error, customMessage);
      } finally {
        this.saving = false;
      }
    },
  },
});
</script>