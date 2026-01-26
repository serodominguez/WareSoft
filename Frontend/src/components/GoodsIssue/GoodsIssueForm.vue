<template>
  <v-card v-if="isOpen" elevation="2">
    <v-toolbar>
      <v-toolbar-title class="text-truncate" style="max-width: 100px;">Salidas</v-toolbar-title>
      <v-divider class="mx-2" inset vertical></v-divider>
      <div class="font-weight-bold" style="font-size: 16px;">{{ localIssue.code }} </div>
      <v-spacer></v-spacer>
    </v-toolbar>
    <v-card-text>
      <v-form ref="form" v-model="valid">
        <v-row>
          <v-col cols="12" md="2">
            <v-select v-if="!localIssue.idIssue" color="indigo" variant="underlined" v-model="localIssue.type"
              :items="issueTypes" label="Tipo de Salida" :rules="[rules.required]" />
            <v-text-field v-else color="indigo" variant="underlined" v-model="localIssue.type" label="Tipo de Salida"
              readonly />
          </v-col>
          <v-col cols="12" md="2">
            <v-autocomplete v-if="!localIssue.idIssue" color="indigo" variant="underlined" :items="users"
              v-model="localIssue.idUser" item-title="userName" item-value="idUser"
              :rules="[rules.required]" no-data-text="No hay datos disponibles" label="Personal"
              :loading="loadingUsers" />
            <v-text-field v-else color="indigo" variant="underlined" v-model="localIssue.userName"
              label="Personal" readonly />
          </v-col>
          <v-col class="px-2" cols="12" md="2">
            <v-btn v-if="!localIssue.idIssue" fab dark color="indigo" class="mt-3" @click="openProductModal">
              <v-icon dark>list</v-icon>
            </v-btn>
          </v-col>
        </v-row>
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
            <td v-if="!localIssue.idIssue">
              <v-text-field v-model.number="item.quantity" variant="underlined" type="number" min="0"
                :rules="[rules.required, rules.minValue]"></v-text-field>
            </td>
            <td v-else>{{ item.quantity }}</td>
            <td v-if="!localIssue.idIssue">
              <v-text-field v-model.number="item.unitPrice" variant="underlined" type="number" min="0"
                :rules="localIssue.type === 'REGULARIZACIÓN' ? [rules.required, rules.minValueOrZero] : [rules.required, rules.minValue]"></v-text-field>
            </td>
            <td v-else>{{ formatCurrency(item.unitPrice) }}</td>
            <td v-if="!localIssue.idIssue">{{ formatCurrency(item.quantity * item.unitPrice) }}</td>
            <td v-else>{{ formatCurrency(item.totalPrice) }}</td>
            <td v-if="!localIssue.idIssue" class="text-center">
              <v-btn color="red" icon="delete" variant="text" @click="removeProduct(item)" size="small"
                title="Quitar" />
            </td>
          </tr>
        </template>
      </v-data-table>
      <v-col v-if="!localIssue.idIssue" cols="12" class="d-flex justify-end">
        <strong>Total Bs.</strong>{{ formatCurrency(totalPrice) }}
      </v-col>
     <v-col v-else cols="12" class="d-flex justify-end">
        <strong>Total Bs.</strong>{{ formatCurrency(localIssue.totalAmount) }}
      </v-col>
      <v-col cols="12" md="12" lg="12" xl="12">
        <v-text-field color="indigo" variant="underlined" label="Observaciones" counter="80" :maxlength="80"
          v-model="localIssue.annotations" :readonly="!!localIssue.idIssue"></v-text-field>
      </v-col>
    </v-card-text>
    <v-card-actions>
      <v-btn v-if="!localIssue.idIssue" color="green" dark class="mb-2" elevation="4" @click="saveIssue"
        :disabled="!valid || details.length === 0" :loading="saving">
        Guardar
      </v-btn>
      <v-btn v-else-if="localIssue.statusIssue === 'Activo'" color="indigo" dark class="mb-2" elevation="4"
        @click="downloadPdf" :loading="downloading">
        Descargar PDF
      </v-btn>
      <v-btn color="red" dark class="mb-2" elevation="4" @click="close">
        {{ localIssue.idIssue ? 'Cerrar' : 'Cancelar' }}
      </v-btn>
    </v-card-actions>
    <CommonProductOut v-model="productModal" @close="productModal = false" @product-added="handleProductAdded" />
  </v-card>
</template>

<script lang="ts">
import { defineComponent, PropType } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { handleApiError } from '@/helpers/errorHandler';
import CommonProductOut from '@/components/Common/CommonProductOut.vue';
import { GoodsIssue, GoodsIssueDetail } from '@/interfaces/goodsIssueInterface';


interface FormRef {
  validate: () => boolean;
}

export default defineComponent({
  name: 'GoodsIssueForm',
  components: {
    CommonProductOut
  },
  props: {
    modelValue: {
      type: Boolean,
      required: true
    },
    issue: {
      type: Object as PropType<GoodsIssue | null>,
      default: () => ({
        idIssue: null,
        code: '',
        type: '',
        storeName: '',
        idUser: null,
        userName: '',
        annotations: '',
        auditCreateDate: '',
        statusIssue: ''
      })
    },
    issueDetails: {
      type: Array as PropType<GoodsIssueDetail[]>,
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
      localIssue: { ...this.issue } as GoodsIssue,
      details: [] as GoodsIssueDetail[],
      documentTypes: [] as string[],
      issueTypes: ['CONSIGNACIÓN', 'REGULARIZACIÓN'],
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
        { title: 'Precio U.', key: 'price', sortable: false },
        { title: 'SubTotal', key: 'subtotal', sortable: false }
      ];

      if (!this.localIssue.idIssue) {
        baseHeaders.push({ title: 'Acciones', key: 'actions', sortable: false, align: 'center' });
      }

      return baseHeaders;
    },
    totalPrice(): number {
      return this.details.reduce((sum, item) => sum + (item.quantity * item.unitPrice), 0);
    },
    users() {
      const usersFromStore = this.store.getters['user/users'];
      return Array.isArray(usersFromStore) ? usersFromStore : [];
    },
    loadingUsers(): boolean {
      return this.store.getters['user/loading'];
    }
  },
  watch: {
    modelValue(newValue: boolean) {
      this.isOpen = newValue;
    },
    isOpen(newValue: boolean) {
      this.$emit('update:modelValue', newValue);
    },
    issue: {
      handler(newIssue: GoodsIssue) {
        this.localIssue = { ...newIssue };
        this.details = [...this.issueDetails];
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
    async openProductModal() {
      this.productModal = true;
    },
    handleProductAdded(product: any) {
      const exists = this.details.find(d => d.idProduct === product.idProduct);

      if (exists) {
        this.toast.warning('Este producto ya está en la lista');
        return;
      }
      this.details.push({
        idProduct: product.idProduct,
        code: product.code,
        description: product.description,
        material: product.material,
        color: product.color,
        categoryName: product.categoryName,
        brandName: product.brandName,
        quantity: 1,
        unitPrice: product.price,
        totalPrice: 0
      });

      this.toast.success('Producto agregado a la lista');
    },
    removeProduct(product: GoodsIssueDetail) {
      const index = this.details.findIndex(d => d.idProduct === product.idProduct);

      if (index !== -1) {
        this.details.splice(index, 1);
        this.toast.error(`Producto ${product.code} eliminado de la lista`);
      }
    },
    async saveIssue() {
      const form = this.$refs.form as FormRef;

      if (!form.validate()) {
        this.toast.warning('Por favor completa todos los campos requeridos');
        return;
      }

      const invalidProducts = this.details.filter(d => {
        if (this.localIssue.type === 'REGULARIZACIÓN') {
          return d.quantity <= 0 || d.unitPrice === null || d.unitPrice === undefined || d.unitPrice < 0;
        } else {
          return d.quantity <= 0 || d.unitPrice <= 0;
        }
      });

      if (invalidProducts.length > 0) {
        if (this.localIssue.type === 'REGULARIZACIÓN') {
          this.toast.warning('Todos los productos deben tener cantidad mayor a 0 y costo mayor o igual a 0');
        } else {
          this.toast.warning('Todos los productos deben tener cantidad y costo válidos mayores a 0');
        }
        return;
      }
      
      this.saving = true;

      try {
        const issueData = {
          type: this.localIssue.type,
          totalAmount: this.totalPrice,
          annotations: this.localIssue.annotations || '',
          idUser: this.localIssue.idUser,
          idStore: this.store.state.currentUser.storeId,
          goodsIssueDetails: this.details.map((d, index) => ({
            item: index + 1,
            idProduct: d.idProduct,
            quantity: d.quantity,
            unitPrice: d.unitPrice,
            totalPrice: d.quantity * d.unitPrice
          }))
        };

        const result = await this.store.dispatch('goodsissue/registerGoodsIssue', issueData);

        if (result.isSuccess) {
          this.toast.success('Salida registrada con éxito');
          this.$emit('saved');
          this.close();
        }
      } catch (error) {
        handleApiError(error, 'Error al registrar la salida');
      } finally {
        this.saving = false;
      }
    },
    async downloadPdf() {
      if (!this.localIssue.idIssue) return;

      this.downloading = true;
      try {
        await this.$store.dispatch('goodsissue/exportGoodsIssuePdf', this.localIssue.idIssue);
        this.toast.success('PDF descargado correctamente');
      } catch (error) {
        handleApiError(error, 'Error al descargar el PDF');
      } finally {
        this.downloading = false;
      }
    },
    formatDateForApi(date: string | null): string | null {
      if (!date) return null;
      if (typeof date === 'string' && date.match(/^\d{4}-\d{2}-\d{2}/)) {
        return date.split('T')[0];
      }

      return date;
    },
    close() {
      this.isOpen = false;
      this.$emit('close');
    }
  },
  mounted() {
    this.details = [...this.issueDetails];
    this.store.dispatch('user/selectUser');
  }
});
</script>