<template>
  <v-card v-if="isOpen" elevation="2">
    <v-toolbar>
      <v-toolbar-title class="text-truncate" style="max-width: 100px;">Entradas</v-toolbar-title>
      <v-divider class="mx-2" inset vertical></v-divider>
      <v-spacer></v-spacer>
    </v-toolbar>
    <v-card-text>
      <v-form ref="form" v-model="valid">
        <v-row>
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
              label="Número del Comprobante" @keyup="uppercase" />
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
      </v-form>
      <v-divider class="my-4"></v-divider>
      <v-data-table :headers="headers" :items="details" class="elevation-1" hide-default-footer>
        <template v-slot:item="{ item }">
          <tr>
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
              <v-text-field v-model.number="item.cost" variant="underlined" type="number" min="0" step="0.01"
                :rules="[rules.required, rules.minValue]"></v-text-field>
            </td>
            <td v-else>{{ formatCurrency(item.cost) }}</td>
            <td>{{ formatCurrency(item.quantity * item.cost) }}</td>
          </tr>
        </template>
      </v-data-table>
      <v-col cols="12" class="d-flex justify-end">
        <strong>Total Bs.</strong>{{ formatCurrency(totalCost) }}
      </v-col>
      <v-col cols="12" md="12" lg="12" xl="12">
        <v-text-field color="indigo" variant="underlined" label="Observaciones" counter="80" :maxlength="80"
          v-model="localReceipt.annotations" @keyup="uppercase" :readonly="!!localReceipt.idReceipt"></v-text-field>
      </v-col>
      <v-alert v-if="alert" type="warning" class="mt-3">
        No hay productos en la lista. Por favor, añade productos.
      </v-alert>
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
    <CommonProductIn v-model="productModal" @close="productModal = false" />
  </v-card>
</template>

<script lang="ts">
import { defineComponent, PropType } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import axios from 'axios';
import { handleApiError } from '@/helpers/errorHandler';
import CommonProductIn from '@/components/Common/CommonProductIn.vue';

interface GoodsReceiptDetail {
  idProduct: number;
  code: string;
  description: string;
  material: string;
  color: string;
  categoryName: string;
  brandName: string;
  quantity: number;
  cost: number;
}

interface GoodsReceiptForm {
  idReceipt: number | null;
  code: string;
  type: string;
  documentType: string;
  documentNumber: string;
  documentDate: Date | null;
  idSupplier: number | null;
  companyName: string;
  annotations: string;
  statusReceipt: string;
}

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
      type: Object as PropType<GoodsReceiptForm | null>,
      default: () => ({
        idReceipt: null,
        code: '',
        type: '',
        documentType: '',
        documentNumber: '',
        documentDate: null,
        idSupplier: null,
        companyName: '',
        annotations: '',
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
      alert: false,
      productModal: false,
      localReceipt: { ...this.receipt } as GoodsReceiptForm,
      details: [] as GoodsReceiptDetail[],
      documentTypes: [] as string[],
      receiptTypes: ['ADQUISICIÓN', 'REGULARIZACIÓN'],
      typesPurchases: ['FACTURA', 'RECIBO'],
      typesImports: ['ENTRADA'],
      rules: {
        required: (value: any) => !!value || 'Este campo es requerido',
        minValue: (value: number) => value > 0 || 'Debe ser mayor a 0'
      }
    };
  },
  computed: {
    headers() {
      const baseHeaders = [
        { title: 'Código', key: 'code', sortable: false },
        { title: 'Descripción', key: 'description', sortable: false },
        { title: 'Material', key: 'material', sortable: false },
        { title: 'Color', key: 'color', sortable: false },
        { title: 'Categoría', key: 'categoryName', sortable: false },
        { title: 'Marca', key: 'brandName', sortable: false },
        { title: 'Cantidad', key: 'quantity', sortable: false },
        { title: 'Precio U.', key: 'cost', sortable: false },
        { title: 'SubTotal', key: 'subtotal', sortable: false }
      ];

      return baseHeaders;
    },
    documentNumberRules() {
      if (this.localReceipt.type === 'ADQUISICIÓN') {
        return [this.rules.required];
      }
      return [];
    },
    totalCost(): number {
      return this.details.reduce((sum, item) => sum + (item.quantity * item.cost), 0);
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
      handler(newReceipt: GoodsReceiptForm) {
        this.localReceipt = { ...newReceipt };
        this.details = [...this.receiptDetails];
        this.updateDocuments();
      },
      deep: true
    }
  },
  methods: {
    formatCurrency(value: number): string {
      return value.toLocaleString('es-BO', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      });
    },
    uppercase() {
      if (this.localReceipt.documentNumber) {
        this.localReceipt.documentNumber = this.localReceipt.documentNumber.toUpperCase();
      }
      if (this.localReceipt.annotations) {
        this.localReceipt.annotations = this.localReceipt.annotations.toUpperCase();
      }
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
      await this.store.dispatch('product/fetchProducts', {
        pageNumber: 1,
        pageSize: 100,
        stateFilter: 1
      });
    },
    async saveReceipt() {
      const form = this.$refs.form as FormRef;
      
      if (!form.validate()) {
        this.toast.warning('Por favor completa todos los campos requeridos');
        return;
      }

      if (this.details.length === 0) {
        this.alert = true;
        return;
      }

      const invalidProducts = this.details.filter(d => d.quantity <= 0 || d.cost <= 0);
      if (invalidProducts.length > 0) {
        this.toast.warning('Todos los productos deben tener cantidad y costo válidos');
        return;
      }

      this.saving = true;
      this.alert = false;

      try {
        const receiptData = {
          documentDate: this.formatDateForApi(this.localReceipt.documentDate),
          type: this.localReceipt.type,
          documentType: this.localReceipt.documentType,
          documentNumber: this.localReceipt.documentNumber,
          annotations: this.localReceipt.annotations || '',
          idSupplier: this.localReceipt.idSupplier,
          idUser: this.store.state.currentUser.userId,
          idStore: this.store.state.currentUser.storeId,
          details: this.details.map(d => ({
            idProduct: d.idProduct,
            quantity: d.quantity,
            cost: d.cost
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
        const response = await axios.get(
          `api/GoodsReceipt/ExportPdf/${this.localReceipt.idReceipt}`,
          { responseType: 'blob' }
        );

        const url = window.URL.createObjectURL(new Blob([response.data]));
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', `Ingreso_${this.localReceipt.code}.pdf`);
        document.body.appendChild(link);
        link.click();
        link.parentNode?.removeChild(link);
        window.URL.revokeObjectURL(url);

        this.toast.success('PDF descargado correctamente');
      } catch (error) {
        handleApiError(error, 'Error al descargar el PDF');
      } finally {
        this.downloading = false;
      }
    },
    formatDateForApi(date: Date | null): string | null {
      if (!date) return null;
      
      const d = new Date(date);
      const year = d.getFullYear();
      const month = String(d.getMonth() + 1).padStart(2, '0');
      const day = String(d.getDate()).padStart(2, '0');
      
      return `${year}-${month}-${day}`;
    },
    close() {
      this.isOpen = false;
      this.$emit('close');
    }
  },
  mounted() {
    this.details = [...this.receiptDetails];
    
    this.store.dispatch('supplier/fetchSuppliers', {
      pageNumber: 1,
      pageSize: 100,
      stateFilter: 1
    });

    if (!this.localReceipt.idReceipt) {
      this.localReceipt.documentDate = null;
      this.updateDocuments();
    }
  }
});
</script>