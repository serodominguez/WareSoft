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
                el ítem: {{ localStore.storE_NAME }}.
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
import { Store } from '@/models/storeModel';

export default defineComponent({
    props: {
        modelValue: {
            type: Boolean,
            required: true,
        },
        store: {
            type: Object as PropType<Store | null>,
            default: () => ({
                pK_STORE: null,
                storE_NAME: ''
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
            localStore: { ...this.store } as Store,
        };
    },
    watch: {
        modelValue(newValue: boolean) {
            this.isOpen = newValue;
        },
        store: {
            handler(newStore: Store) {
                this.localStore = { ...newStore };
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
                await this.$store.dispatch('store/removeStore', this.localStore.pK_STORE);
                this.toast.success('Tienda eliminada con éxito!');
                this.close();
            } catch (error: any) {
                if (error.response) {
                    this.toast.error('Error al eliminar la Tienda.');
                }
            }
        },
        async enabled() {
            try {
                await this.$store.dispatch('store/enableStore', this.localStore.pK_STORE);
                this.toast.success('Tienda habilitada con éxito!');
                this.close();
            } catch (error: any) {
                if (error.response) {
                    this.toast.error('Error al habilitar la Tienda.');
                }
            }
        },
        async disabled() {
            try {
                await this.$store.dispatch('store/disableStore', this.localStore.pK_STORE);
                this.toast.success('Tienda deshabilitada con éxito!');
                this.close();
            } catch (error: any) {
                if (error.response) {
                    this.toast.error('Error al deshabilitar la Tienda.');
                }
            }
        },
    },
});
</script>