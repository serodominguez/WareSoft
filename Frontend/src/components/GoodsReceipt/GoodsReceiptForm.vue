<template>
  <v-card v-if="isOpen" elevation="2">
    <v-toolbar>
      <v-toolbar-title class="text-truncate" style="max-width: 100px;">Entradas</v-toolbar-title>
      <v-divider class="mx-2" inset vertical></v-divider>
      <div class="font-weight-bold" style="font-size: 16px;">{{ localReceipt.code }} </div>
      <v-spacer></v-spacer>
    </v-toolbar>
    <v-card-text>
      <v-form ref="form" v-model="valid">
      <v-container class="px-4" max-width="1700px">
        <v-row justify="center">
          <v-col cols="12" md="2">
            <v-select v-if="!localReceipt.idReceipt" color="indigo" variant="underlined" v-model="localReceipt.type"
              :items="receiptTypes" label="Tipo de Entrada" :rules="[rules.required]"
              @update:modelValue="updateDocuments" />
            <v-text-field v-else color="indigo" variant="underlined" v-model="localReceipt.type" label="Tipo de Entrada"
              readonly />
          </v-col>
          <v-col cols="12" md="2">
            <v-select v-if="!localReceipt.idReceipt" color="indigo" variant="underlined"
              v-model="localReceipt.documentType" :items="documentTypes" label="Tipo de Comprobante"
              :rules="[rules.required]" />
            <v-text-field v-else color="indigo" variant="underlined" v-model="localReceipt.documentType"
              label="Tipo de Comprobante" readonly />
          </v-col>
          <v-col cols="12" md="2">
            <v-text-field v-if="!localReceipt.idReceipt" color="indigo" variant="underlined"
              v-model="localReceipt.documentNumber" :rules="documentNumberRules" counter="30" :maxlength="30"
              label="Número del Comprobante" />
            <v-text-field v-else color="indigo" variant="underlined" v-model="localReceipt.documentNumber"
              label="Número del Comprobante" readonly />
          </v-col>
          <v-col cols="12" md="2">
            <v-date-input v-if="!localReceipt.idReceipt" locale="es" placeholder="dd/mm/yyyy"
              v-model="localReceipt.documentDate" label="Fecha del Comprobante" variant="underlined" prepend-icon=""
              :rules="[rules.required]" />
            <v-text-field v-else v-model="localReceipt.documentDate" label="Fecha del Comprobante" variant="underlined"
              readonly />
          </v-col>
          <v-col cols="12" md="2">
            <v-autocomplete v-if="!localReceipt.idReceipt" color="indigo" variant="underlined" :items="suppliers"
              v-model="localReceipt.idSupplier" item-title="companyName" item-value="idSupplier"
              :rules="[rules.required]" no-data-text="No hay datos disponibles" label="Proveedor"
              :loading="loadingSuppliers" />
            <v-text-field v-else color="indigo" variant="underlined" v-model="localReceipt.companyName"
              label="Proveedor" readonly />
          </v-col>
          <v-col class="px-2" cols="12" md="2">
            <v-btn v-if="!localReceipt.idReceipt" fab dark color="indigo" class="mt-3" @click="openProductModal">
              <v-icon dark>list</v-icon>
            </v-btn>
          </v-col>
        </v-row>
        </v-container>
      </v-form>
      <v-divider class="my-4"></v-divider>
      <v-data-table :headers="headers" :items="details" class="elevation-1" hide-default-footer
        :no-data-text="'No hay productos agregados'">
        <template v-slot:item="{ item, index }">
          <tr>
            <td class="text-center">{{ index + 1 }}</td>
            <td>{{ item.code }}</td>
            <td>{{ item.description }}</td>
            <td>{{ item.material }}</td>
            <td>{{ item.color }}</td>
            <td>{{ item.categoryName }}</td>
            <td>{{ item.brandName }}</td>
            <td v-if="!localReceipt.idReceipt">
              <v-text-field v-model.number="item.quantity" variant="underlined" type="number" min="0"
                :rules="[rules.required, rules.minValue]"></v-text-field>
            </td>
            <td v-else>{{ item.quantity }}</td>
            <td v-if="!localReceipt.idReceipt">
              <v-text-field v-model.number="item.unitCost" variant="underlined" type="number" min="0"
                :rules="localReceipt.type === 'REGULARIZACIÓN' ? [rules.required, rules.minValueOrZero] : [rules.required, rules.minValue]"></v-text-field>
            </td>
            <td v-else>{{ formatCurrency(item.unitCost) }}</td>
            <td v-if="!localReceipt.idReceipt">{{ formatCurrency(item.quantity * item.unitCost) }}</td>
            <td v-else>{{ formatCurrency(item.totalCost) }}</td>
            <td v-if="!localReceipt.idReceipt" class="text-center">
              <v-btn color="red" icon="delete" variant="text" @click="removeProduct(item)" size="small"
                title="Quitar" />
            </td>
          </tr>
        </template>
      </v-data-table>
      <v-col v-if="!localReceipt.idReceipt" cols="12" class="d-flex justify-end">
        <strong>Total Bs.</strong>{{ formatCurrency(totalCost) }}
      </v-col>
     <v-col v-else cols="12" class="d-flex justify-end">
        <strong>Total Bs.</strong>{{ formatCurrency(localReceipt.totalAmount) }}
      </v-col>
      <v-col cols="12" md="12" lg="12" xl="12">
        <v-text-field color="indigo" variant="underlined" label="Observaciones" counter="80" :maxlength="80"
          v-model="localReceipt.annotations" :readonly="!!localReceipt.idReceipt"></v-text-field>
      </v-col>
    </v-card-text>
    <v-card-actions>
      <v-btn v-if="!localReceipt.idReceipt" color="green" dark class="mb-2" elevation="4" @click="saveReceipt"
        :disabled="!valid || details.length === 0" :loading="saving">
        Guardar
      </v-btn>
      <v-btn v-else-if="localReceipt.statusReceipt === 'Activo'" color="indigo" dark class="mb-2" elevation="4"
        @click="downloadPdf" :loading="downloading">
        Descargar PDF
      </v-btn>
      <v-btn color="red" dark class="mb-2" elevation="4" @click="close">
        {{ localReceipt.idReceipt ? 'Cerrar' : 'Cancelar' }}
      </v-btn>
    </v-card-actions>
    <CommonProductIn v-model="productModal" @close="productModal = false" @product-added="handleProductAdded" />
  </v-card>
</template>

<script lang="ts">
import { defineComponent, PropType } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { handleApiError } from '@/helpers/errorHandler';
import CommonProductIn from '@/components/Common/CommonProductIn.vue';
import { GoodsReceipt, GoodsReceiptDetail } from '@/interfaces/goodsReceiptInterface';


interface FormRef {
  validate: () => boolean;
}

export default defineComponent({
  name: 'GoodsReceiptForm',
  components: {
    CommonProductIn
  },
  props: {
    modelValue: {
      type: Boolean,
      required: true
    },
    receipt: {
      type: Object as PropType<GoodsReceipt | null>,
      default: () => ({
        idReceipt: null,
        code: '',
        type: '',
        storeName: '',
        documentType: '',
        documentNumber: '',
        documentDate: '',
        idSupplier: null,
        companyName: '',
        annotations: '',
        auditCreateDate: '',
        statusReceipt: ''
      })
    },
    receiptDetails: {
      type: Array as PropType<GoodsReceiptDetail[]>,
      default: () => []
    }
  },
  emits: ['update:modelValue', 'saved', 'close'],
  setup() {
    const store = useStore();
    const toast = useToast();
    return { store, toast };
  },
  data() {
    return {
      isOpen: this.modelValue,
      valid: false,
      saving: false,
      downloading: false,
      productModal: false,
      localReceipt: { ...this.receipt } as GoodsReceipt,
      details: [] as GoodsReceiptDetail[],
      documentTypes: [] as string[],
      receiptTypes: ['ADQUISICIÓN', 'REGULARIZACIÓN'],
      typesPurchases: ['FACTURA', 'RECIBO'],
      typesImports: ['ENTRADA'],
      rules: {
        required: (value: any) => !!value || 'Este campo es requerido',
        minValue: (value: number) => value > 0 || 'Debe ser mayor a 0',
        minValueOrZero: (value: number) => (value !== null && value !== undefined && value >= 0) || 'Debe ser mayor o igual a 0'
      }
    };
  },
  computed: {
    headers(): Array<{ title: string; key: string; sortable: boolean; align?: 'start' | 'end' | 'center' }> {
      const baseHeaders: Array<{ title: string; key: string; sortable: boolean; align?: 'start' | 'end' | 'center' }> = [
        { title: 'Item', key: 'item', sortable: false, align: 'center' },
        { title: 'Código', key: 'code', sortable: false },
        { title: 'Descripción', key: 'description', sortable: false },
        { title: 'Material', key: 'material', sortable: false },
        { title: 'Color', key: 'color', sortable: false },
        { title: 'Categoría', key: 'categoryName', sortable: false },
        { title: 'Marca', key: 'brandName', sortable: false },
        { title: 'Cantidad', key: 'quantity', sortable: false },
        { title: 'Costo U.', key: 'cost', sortable: false },
        { title: 'SubTotal', key: 'subtotal', sortable: false }
      ];

      if (!this.localReceipt.idReceipt) {
        baseHeaders.push({ title: 'Acciones', key: 'actions', sortable: false, align: 'center' });
      }

      return baseHeaders;
    },
    documentNumberRules() {
      if (this.localReceipt.type === 'ADQUISICIÓN') {
        return [this.rules.required];
      }
      return [];
    },
    totalCost(): number {
      return this.details.reduce((sum, item) => sum + (item.quantity * item.unitCost), 0);
    },
    suppliers() {
      const suppliersFromStore = this.store.getters['supplier/suppliers'];
      return Array.isArray(suppliersFromStore) ? suppliersFromStore : [];
    },
    loadingSuppliers(): boolean {
      return this.store.getters['supplier/loading'];
    }
  },
  watch: {
    modelValue(newValue: boolean) {
      this.isOpen = newValue;
    },
    isOpen(newValue: boolean) {
      this.$emit('update:modelValue', newValue);
    },
    receipt: {
      handler(newReceipt: GoodsReceipt) {
        this.localReceipt = { ...newReceipt };
        this.details = [...this.receiptDetails];
        this.updateDocuments();
      },
      deep: true
    }
  },
  methods: {
    formatCurrency(value: number): string {
      return new Intl.NumberFormat('es-BO', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
        useGrouping: true
      }).format(value).replace(/\./g, ',');
    },
    updateDocuments() {
      this.localReceipt.documentType = '';

      if (this.localReceipt.type === 'ADQUISICIÓN') {
        this.documentTypes = this.typesPurchases;
      } else if (this.localReceipt.type === 'REGULARIZACIÓN') {
        this.documentTypes = this.typesImports;
      } else {
        this.documentTypes = [];
      }
    },
    async openProductModal() {
      this.productModal = true;
    },
    handleProductAdded(product: any) {
      // Verifica si el producto ya está en la lista
      const exists = this.details.find(d => d.idProduct === product.idProduct);

      if (exists) {
        this.toast.warning('Este producto ya está en la lista');
        return;
      }

      // Agrega el producto a los detalles
      this.details.push({
        idProduct: product.idProduct,
        code: product.code,
        description: product.description,
        material: product.material,
        color: product.color,
        categoryName: product.categoryName,
        brandName: product.brandName,
        quantity: 1,  // Cantidad inicial
        unitCost: 0,       // Precio inicial
        totalCost: 0
      });

      this.toast.success('Producto agregado a la lista');
    },
    removeProduct(product: GoodsReceiptDetail) {
      const index = this.details.findIndex(d => d.idProduct === product.idProduct);

      if (index !== -1) {
        this.details.splice(index, 1);
        this.toast.error(`Producto ${product.code} eliminado de la lista`);
      }
    },
    async saveReceipt() {
      const form = this.$refs.form as FormRef;

      if (!form.validate()) {
        this.toast.warning('Por favor completa todos los campos requeridos');
        return;
      }

      const invalidProducts = this.details.filter(d => {
        if (this.localReceipt.type === 'REGULARIZACIÓN') {
          // Para REGULARIZACIÓN: cantidad > 0 y costo >= 0 (permite 0)
          return d.quantity <= 0 || d.unitCost === null || d.unitCost === undefined || d.unitCost < 0;
        } else {
          // Para ADQUISICIÓN: cantidad > 0 y costo > 0
          return d.quantity <= 0 || d.unitCost <= 0;
        }
      });

      if (invalidProducts.length > 0) {
        if (this.localReceipt.type === 'REGULARIZACIÓN') {
          this.toast.warning('Todos los productos deben tener cantidad mayor a 0 y costo mayor o igual a 0');
        } else {
          this.toast.warning('Todos los productos deben tener cantidad y costo válidos mayores a 0');
        }
        return;
      }
      
      this.saving = true;

      try {
        const receiptData = {
          documentDate: this.formatDateForApi(this.localReceipt.documentDate),
          type: this.localReceipt.type,
          documentType: this.localReceipt.documentType,
          documentNumber: this.localReceipt.documentNumber,
          totalAmount: this.totalCost,
          annotations: this.localReceipt.annotations || '',
          idSupplier: this.localReceipt.idSupplier,
          idStore: this.store.state.currentUser.storeId,
          goodsReceiptDetails: this.details.map((d, index) => ({
            item: index + 1,
            idProduct: d.idProduct,
            quantity: d.quantity,
            unitCost: d.unitCost,
            totalCost: d.quantity * d.unitCost
          }))
        };

        const result = await this.store.dispatch('goodsreceipt/registerGoodsReceipt', receiptData);

        if (result.isSuccess) {
          this.toast.success('Entrada registrada con éxito');
          this.$emit('saved');
          this.close();
        }
      } catch (error) {
        handleApiError(error, 'Error al registrar la entrada');
      } finally {
        this.saving = false;
      }
    },
    async downloadPdf() {
      if (!this.localReceipt.idReceipt) return;

      this.downloading = true;
      try {
        await this.$store.dispatch('goodsreceipt/exportGoodsReceiptPdf', this.localReceipt.idReceipt);
        this.toast.success('PDF descargado correctamente');
      } catch (error) {
        handleApiError(error, 'Error al descargar el PDF');
      } finally {
        this.downloading = false;
      }
    },
    formatDateForApi(date: string | null): string | null {
      if (!date) return null;

      // Si ya es un string en formato correcto (YYYY-MM-DD), retornarlo directamente
      if (typeof date === 'string' && date.match(/^\d{4}-\d{2}-\d{2}/)) {
        return date.split('T')[0]; // Por si viene con timestamp
      }

      return date;
    },
    close() {
      this.isOpen = false;
      this.$emit('close');
    }
  },
  mounted() {
    this.details = [...this.receiptDetails];
    this.store.dispatch('supplier/selectSupplier');
    if (!this.localReceipt.idReceipt) {
      this.localReceipt.documentDate = '';
      this.updateDocuments();
    }
  }
});
</script>