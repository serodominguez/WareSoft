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
                el ítem: {{ localUser.userName }}.
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
import { User } from '@/interfaces/userInterface';

export default defineComponent({
    props: {
        modelValue: {
            type: Boolean,
            required: true,
        },
        user: {
            type: Object as PropType<User | null>,
            default: () => ({
                idUser: null,
                userName: ''
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
            localUser: { ...this.user } as User,
        };
    },
    watch: {
        modelValue(newValue: boolean) {
            this.isOpen = newValue;
        },
        user: {
            handler(newUser: User) {
                this.localUser = { ...newUser };
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
                await this.$store.dispatch('user/removeUser', this.localUser.idUser);
                this.toast.success('Usuario eliminado con éxito!');
                this.close();
            } catch (error: any) {
                if (error.response) {
                    this.toast.error('Error al eliminar el usuario.');
                }
            }
        },
        async enabled() {
            try {
                await this.$store.dispatch('user/enableUser', this.localUser.idUser);
                this.toast.success('Usuario habilitado con éxito!');
                this.close();
            } catch (error: any) {
                if (error.response) {
                    this.toast.error('Error al habilitar el usuario.');
                }
            }
        },
        async disabled() {
            try {
                await this.$store.dispatch('user/disableUser', this.localUser.idUser);
                this.toast.success('Usuario deshabilitado con éxito!');
                this.close();
            } catch (error: any) {
                if (error.response) {
                    this.toast.error('Error al deshabilitar el usuario.');
                }
            }
        },
    },
});
</script>