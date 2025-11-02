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
                el ítem: {{ localModule.modulE_NAME }}.
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
import { Module } from '@/interfaces/moduleInterface';

export default defineComponent({
    props: {
        modelValue: {
            type: Boolean,
            required: true,
        },
        module: {
            type: Object as PropType<Module | null>,
            default: () => ({
                pK_MODULE: null,
                modulE_NAME: ''
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
            localModule: { ...this.module } as Module,
        };
    },
    watch: {
        modelValue(newValue: boolean) {
            this.isOpen = newValue;
        },
        module: {
            handler(newModule: Module) {
                this.localModule = { ...newModule };
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
                await this.$store.dispatch('module/removeModule', this.localModule.pK_MODULE);
                this.toast.success('Módulo eliminado con éxito!');
                this.close();
            } catch (error: any) {
                if (error.response) {
                    this.toast.error('Error al eliminar el módulo.');
                }
            }
        },
        async enabled() {
            try {
                await this.$store.dispatch('module/enableModule', this.localModule.pK_MODULE);
                this.toast.success('Módulo habilitado con éxito!');
                this.close();
            } catch (error: any) {
                if (error.response) {
                    this.toast.error('Error al habilitar el módulo.');
                }
            }
        },
        async disabled() {
            try {
                await this.$store.dispatch('module/disableModule', this.localModule.pK_MODULE);
                this.toast.success('Módulo deshabilitado con éxito!');
                this.close();
            } catch (error: any) {
                if (error.response) {
                    this.toast.error('Error al deshabilitar el módulo.');
                }
            }
        },
    },
});
</script>