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
                el ítem: {{ localRole.roleName }}.
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
import { Role } from '@/interfaces/roleInterface';

export default defineComponent({
    props: {
        modelValue: {
            type: Boolean,
            required: true,
        },
        role: {
            type: Object as PropType<Role | null>,
            default: () => ({
                idRole: null,
                roleName: ''
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
            localRole: { ...this.role } as Role,
        };
    },
    watch: {
        modelValue(newValue: boolean) {
            this.isOpen = newValue;
        },
        role: {
            handler(newRole: Role) {
                this.localRole = { ...newRole };
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
                await this.$store.dispatch('role/removeRole', this.localRole.idRole);
                this.toast.success('Rol eliminado con éxito!');
                this.close();
            } catch (error: any) {
                if (error.response) {
                    this.toast.error('Error al eliminar el rol.');
                }
            }
        },
        async enabled() {
            try {
                await this.$store.dispatch('role/enableRole', this.localRole.idRole);
                this.toast.success('Rol habilitado con éxito!');
                this.close();
            } catch (error: any) {
                if (error.response) {
                    this.toast.error('Error al habilitar el rol.');
                }
            }
        },
        async disabled() {
            try {
                await this.$store.dispatch('role/disableRole', this.localRole.idRole);
                this.toast.success('Rol deshabilitado con éxito!');
                this.close();
            } catch (error: any) {
                if (error.response) {
                    this.toast.error('Error al deshabilitar el rol.');
                }
            }
        },
    },
});
</script>