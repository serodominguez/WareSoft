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
                el ítem: {{ localCategory.categoryName }}.
            </v-card-text>
            <v-card-actions class="d-flex justify-space-between">
                <div class="d-flex">
                    <v-btn v-if="action === 0" color="indigo" dark class="mr-2" elevation="4"
                        @click="remove" :loading="processing">Eliminar</v-btn>
                    <v-btn v-if="action === 1" color="indigo" dark class="mr-2" elevation="4"
                        @click="enabled" :loading="processing">Activar</v-btn>
                    <v-btn v-if="action === 2" color="indigo" dark class="mr-2" elevation="4"
                        @click="disabled" :loading="processing">Desactivar</v-btn>
                    <v-btn color="red" elevation="4" @click="close" :disabled="processing">Cancelar</v-btn>
                </div>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>
<script lang="ts">
import { useToast } from 'vue-toastification';
import { defineComponent, PropType } from 'vue';
import { Category } from '@/interfaces/categoryInterface';
import { handleApiError } from '@/helpers/errorHandler';

export default defineComponent({
    props: {
        modelValue: {
            type: Boolean,
            required: true,
        },
        category: {
            type: Object as PropType<Category | null>,
            default: () => ({
                idCategory: null,
                categoryName: ''
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
            processing: false,
            toast: useToast(),
            localCategory: { ...this.category } as Category,
        };
    },
    watch: {
        modelValue(newValue: boolean) {
            this.isOpen = newValue;
        },
        category: {
            handler(newCategory: Category) {
                this.localCategory = { ...newCategory };
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
            this.processing = true;
            try {
                const result = await this.$store.dispatch('category/removeCategory', this.localCategory.idCategory);
                if (result.isSuccess) {
                    this.toast.success('Categoría eliminada con éxito!');
                    this.close()
                }
                ;
            } catch (error: any) {
                handleApiError(error, 'Error al eliminar la categoría');
            } finally {
                this.processing = false;
            }
        },
        async enabled() {
            this.processing = true;
            try {
                const result = await this.$store.dispatch('category/enableCategory', this.localCategory.idCategory);
                if (result.isSuccess) {
                    this.toast.success('Categoría habilitada con éxito!');
                    this.close();
                }

            } catch (error: any) {
                handleApiError(error, 'Error al habilitar la categoría');
                let errorMsg = 'Error en habilitar la categoría';
            } finally {
                this.processing = false;
            }
        },
        async disabled() {
            this.processing = true;
            try {
                const result = await this.$store.dispatch('category/disableCategory', this.localCategory.idCategory);
                if (result.isSuccess) {
                    this.toast.success('Categoría deshabilitada con éxito!');
                    this.close();
                }
            } catch (error: any) {
                handleApiError(error, 'Error al deshabilitar la categoría');
            } finally {
                this.processing = false;
            }
        },
    },
});
</script>