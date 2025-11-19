<template>
    <v-dialog v-model="isOpen" max-width="400px" persistent>
        <v-card>
            <v-card-title class="bg-surface-light pt-4" v-if="action === 0">Eliminar Item?</v-card-title>
            <v-card-title class="bg-surface-light pt-4" v-if="action === 1">Activar Item?</v-card-title>
            <v-card-title class="bg-surface-light pt-4" v-if="action === 2">Desactivar Item?</v-card-title>
            <v-divider></v-divider>
            <v-card-text>
                Estás a punto de
                <span v-if="action === 0">eliminar</span>
                <span v-if="action === 1">activar</span>
                <span v-if="action === 2">desactivar</span>
                el ítem: {{ localProduct.description }}.
            </v-card-text>
            <v-card-actions class="d-flex justify-space-between">
                <div class="d-flex">
                    <v-btn v-if="action === 0" color="indigo" dark class="mr-2" elevation="4"
                        @click="remove">Eliminar</v-btn>
                    <v-btn v-if="action === 1" color="indigo" dark class="mr-2" elevation="4"
                        @click="enabled">Activar</v-btn>
                    <v-btn v-if="action === 2" color="indigo" dark class="mr-2" elevation="4"
                        @click="disabled">Desactivar</v-btn>
                    <v-btn color="red" elevation="4" @click="close">Cancelar</v-btn>
                </div>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>
<script lang="ts">
import { useToast } from 'vue-toastification';
import { defineComponent, PropType } from 'vue';
import { Product } from '@/interfaces/productInterface';

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
                description: ''
            }),
        },
        action: {
            type: Number,
            required: true,
        },
    },
    data() {
        return {
            isOpen: this.modelValue,
            valid: false,
            toast: useToast(),
            localProduct: { ...this.product } as Product,
        };
    },
    watch: {
        modelValue(newValue: boolean) {
            this.isOpen = newValue;
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
            this.$emit('update:modelValue', false);
        },
        async remove() {
            try {
                const result = await this.$store.dispatch('product/removeProduct', this.localProduct.idProduct);
                if (result.isSuccess) {
                    this.toast.success('Producto eliminado con éxito!');
                    this.close();
                }

            } catch (error: any) {
                let errorMsg = 'Error en eliminar el producto';

                if (error?.response?.status) {
                    errorMsg += `: Error ${error.response.status}`;
                } else if (error?.response?.data?.message) {
                    errorMsg += `: ${error.response.data.message}`;
                } else if (error?.message) {
                    errorMsg += `: ${error.message}`;
                } else {
                    errorMsg += '.';
                }

                this.toast.error(errorMsg);
            }
        },
        async enabled() {
            try {
                const result = await this.$store.dispatch('product/enableProduct', this.localProduct.idProduct);
                if (result.isSuccess) {
                    this.toast.success('Producto habilitado con éxito!');
                    this.close();
                }
            } catch (error: any) {
                let errorMsg = 'Error en habilitar el producto';

                if (error?.response?.status) {
                    errorMsg += `: Error ${error.response.status}`;
                } else if (error?.response?.data?.message) {
                    errorMsg += `: ${error.response.data.message}`;
                } else if (error?.message) {
                    errorMsg += `: ${error.message}`;
                } else {
                    errorMsg += '.';
                }

                this.toast.error(errorMsg);
            }
        },
        async disabled() {
            try {
                const result = await this.$store.dispatch('product/disableProduct', this.localProduct.idProduct);
                if (result.isSuccess) {
                    this.toast.success('Producto deshabilitado con éxito!');
                    this.close();
                }

            } catch (error: any) {
                let errorMsg = 'Error en deshabilitar el producto';

                if (error?.response?.status) {
                    errorMsg += `: Error ${error.response.status}`;
                } else if (error?.response?.data?.message) {
                    errorMsg += `: ${error.response.data.message}`;
                } else if (error?.message) {
                    errorMsg += `: ${error.message}`;
                } else {
                    errorMsg += '.';
                }

                this.toast.error(errorMsg);
            }
        },
    },
});
</script>