<template>
  <div>
    <GoodsIssueList v-if="!form" :goodsissue="goodsissue" :loading="loading"
      :totalGoodsIssue="totalGoodsIssue" :downloadingExcel="downloadingExcel" :canCreate="canCreate"
      :canRead="canRead" :canEdit="canEdit" :canDelete="canDelete" :items-per-page="itemsPerPage"
      v-model:drawer="drawer" v-model:selectedFilter="selectedFilter" v-model:state="state"
      v-model:startDate="startDate" v-model:endDate="endDate" @open-form="openForm" @open-modal="openModal"
      @view-goodsissue="openForm" @fetch-goodsissue="fetchGoodsIssue" @search-goodsissue="searchGoodsIssue"
      @update-items-per-page="updateItemsPerPage" @change-page="changePage" @download-excel="downloadExcel"
      @print-pdf="printPdf" />

    <GoodsIssueForm v-if="form" v-model="form" :issue="selectedGoodsIssue"
      :issueDetails="selectedIssueDetails" @saved="handleSaved" @close="closeForm" />

    <CommonModal v-model="modal" :itemId="selectedGoodsIssue?.idIssue || 0" :item="selectedGoodsIssue?.code || ''"
      :action="action" moduleName="goodsissue" entityName="GoodsIssue" name="Salida" gender="female"
      @action-completed="handleActionCompleted" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useStore } from 'vuex';
import { useToast } from 'vue-toastification';
import { GoodsIssue } from '@/interfaces/goodsIssueInterface';
import { formatDate } from '@/utils/date';
import { handleApiError, handleSilentError } from '@/helpers/errorHandler';
import GoodsIssueList from '@/components/GoodsIssue/GoodsIssueList.vue';
import GoodsIssueForm from '@/components/GoodsIssue/GoodsIssueForm.vue';
import CommonModal from '@/components/Common/CommonModal.vue';

export default defineComponent({
  name: 'GoodsIssueView',
  components: {
    GoodsIssueList,
    GoodsIssueForm,
    CommonModal
  },
  setup() {
    const store = useStore();
    const toast = useToast();
    return { store, toast };
  },
  data() {
    return {
      currentPage: 1,
      itemsPerPage: 10,
      search: null as string | null,
      selectedFilter: 'Código',
      drawer: false,
      state: 'Activos',
      startDate: null,
      endDate: null,
      form: false,
      modal: false,
      selectedGoodsIssue: null as GoodsIssue | null,
      action: 0,
      downloadingExcel: false
    };
  },
  computed: {
    goodsissue() {
      return this.store.getters['goodsissue/goodsissue'];
    },
    selectedIssueDetails() {
      return this.store.getters['goodsissue/selectedIssueDetails'];
    },
    loading() {
      return this.store.getters['goodsissue/loading'];
    },
    totalGoodsIssue() {
      return this.store.getters['goodsissue/totalGoodsIssue'];
    },
    stateFilter(): number {
      return this.state === 'Activos' ? 1 : 0;
    },
    canCreate(): boolean {
      return this.$store.getters.hasPermission('salida de productos', 'crear');
    },
    canRead(): boolean {
      return this.$store.getters.hasPermission('salida de productos', 'leer');
    },
    canEdit(): boolean {
      return this.$store.getters.hasPermission('salida de productos', 'editar');
    },
    canDelete(): boolean {
      return this.$store.getters.hasPermission('salida de productos', 'eliminar');
    }
  },
  methods: {
    openModal(payload: { goodsissue: GoodsIssue, action: number }) {
      this.selectedGoodsIssue = payload.goodsissue;
      this.action = payload.action;
      this.modal = true;
    },

    async openForm(goodsissue?: GoodsIssue) {
      if (goodsissue?.idIssue) {
        try {
          await this.store.dispatch('goodsissue/fetchGoodsIssueById', goodsissue.idIssue);
          this.selectedGoodsIssue = this.store.getters['goodsissue/selectedGoodsIssue'];
        } catch (error) {
          handleApiError(error, 'Error al cargar los detalles de la salida');
          return;
        }
      } else {
        this.selectedGoodsIssue = {
          idIssue: null,
          code: '',
          type: '',
          storeName: '',
          idUser: null,
          userName: '',
          totalAmount: 0,
          annotations: '',
          auditCreateDate: '',
          statusIssue: ''
        };
      }

      this.form = true;
    },

    closeForm() {
      this.store.commit('goodsissue/SET_SELECTED_ISSUE_DETAILS', []);
      this.store.commit('goodsissue/SET_SELECTED_ITEM', null);
      this.selectedGoodsIssue = null;
      this.form = false;
    },

    async fetchGoodsIssue(params?: any) {
      try {
        await this.store.dispatch('goodsissue/fetchGoodsIssue', params || {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          stateFilter: this.stateFilter,
          sort: 'IdIssue',
          order: 'desc'
        });
      } catch (error) {
        handleSilentError(error);
      }
    },

    getFilterParams(params: any) {
      const filterMap: { [key: string]: number } = {
        "Código": 1,
        "Tienda": 2,
        "Personal": 3
      };
      const numberFilterValue = filterMap[params.selectedFilter || this.selectedFilter];
      const textFilterValue = params.search?.trim() || null;
      const startDateStr = params.startDate ? formatDate(params.startDate) : null;
      const endDateStr = params.endDate ? formatDate(params.endDate) : null;

      return {
        textFilter: textFilterValue,
        numberFilter: numberFilterValue,
        stateFilter: this.stateFilter,
        startDate: startDateStr,
        endDate: endDateStr
      };
    },

    async searchGoodsIssue(params: any) {
      this.search = params.search;
      this.selectedFilter = params.selectedFilter;
      this.state = params.state;
      this.startDate = params.startDate;
      this.endDate = params.endDate;

      try {
        await this.store.dispatch("goodsissue/fetchGoodsIssue", {
          pageNumber: 1,
          pageSize: this.itemsPerPage,
          sort: 'IdIssue',
          order: 'desc',
          ...this.getFilterParams(params)
        });
        this.currentPage = 1;
      } catch (error) {
        handleApiError(error, 'Error al buscar salidas');
      }
    },

    refreshGoodsIssue() {
      if (this.search?.trim()) {
        this.searchGoodsIssue({
          search: this.search,
          selectedFilter: this.selectedFilter,
          state: this.state,
          startDate: this.startDate,
          endDate: this.endDate
        });
      } else {
        this.fetchGoodsIssue();
      }
    },

    updateItemsPerPage(itemsPerPage: number) {
      this.itemsPerPage = itemsPerPage;
      this.currentPage = 1;
      this.refreshGoodsIssue();
    },

    changePage(page: number) {
      this.currentPage = page;
      this.refreshGoodsIssue();
    },

    async downloadExcel(params: any) {
      this.downloadingExcel = true;
      try {
        await this.store.dispatch("goodsissue/downloadGoodsIssueExcel", {
          pageNumber: this.currentPage,
          pageSize: this.itemsPerPage,
          sort: 'IdIssue',
          order: 'desc',
          ...this.getFilterParams(params)
        });
        this.toast.success('Archivo descargado correctamente');
      } catch (error) {
        handleApiError(error, 'Error al descargar el archivo Excel');
      } finally {
        this.downloadingExcel = false;
      }
    },
    
    async printPdf(goodsissue: GoodsIssue) {
      if (!goodsissue.idIssue) return;

      try {
        const result = await this.store.dispatch('goodsissue/openGoodsIssuePdf', goodsissue.idIssue);

        if (result.isSuccess) {
          this.toast.success('PDF abierto correctamente');
        } else {
          this.toast.error('Error al abrir el PDF');
        }
      } catch (error) {
        handleApiError(error, 'Error al abrir el PDF');
      }
    },

    handleSaved() {
      this.fetchGoodsIssue();
    },

    handleActionCompleted() {
      this.fetchGoodsIssue();
    }
  },
  mounted() {
    this.fetchGoodsIssue();
  }
});
</script>