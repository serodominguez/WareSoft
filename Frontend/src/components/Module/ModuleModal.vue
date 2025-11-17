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
                el ítem: {{ localModule.moduleName }}.
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
                idModule: null,
                moduleName: ''
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
                const result = await this.$store.dispatch('module/removeModule', this.localModule.idModule);
                if (result.isSuccess) {
                    this.toast.success('Módulo eliminado con éxito!');
                    this.close();
                }
            } catch (error: any) {
                let errorMsg = 'Error en eliminar el módulo';

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
                const result = await this.$store.dispatch('module/enableModule', this.localModule.idModule);
                if (result.isSuccess) {
                    this.toast.success('Módulo habilitado con éxito!');
                    this.close();
                }
            } catch (error: any) {
                let errorMsg = 'Error en habilitar el módulo';

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
                const result = await this.$store.dispatch('module/disableModule', this.localModule.idModule);
                if (result.isSuccess) {
                    this.toast.success('Módulo deshabilitado con éxito!');
                    this.close();
                }
            } catch (error: any) {
                let errorMsg = 'Error en deshabilitar el módulo';

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