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
                el ítem: {{ itemName }}.
            </v-card-text>
            <v-card-actions class="d-flex justify-space-between">
                <div class="d-flex">
                    <v-btn v-if="action === 0" color="indigo" dark class="mr-2" elevation="4" @click="handleAction"
                        :loading="processing">
                        Eliminar
                    </v-btn>
                    <v-btn v-if="action === 1" color="indigo" dark class="mr-2" elevation="4" @click="handleAction"
                        :loading="processing">
                        Activar
                    </v-btn>
                    <v-btn v-if="action === 2" color="indigo" dark class="mr-2" elevation="4" @click="handleAction"
                        :loading="processing">
                        Desactivar
                    </v-btn>
                    <v-btn color="red" elevation="4" @click="close" :disabled="processing">
                        Cancelar
                    </v-btn>
                </div>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
import { useToast } from 'vue-toastification';
import { defineComponent, PropType } from 'vue';
import { handleApiError } from '@/helpers/errorHandler';

export default defineComponent({
    name: 'CommonModal',
    props: {
        modelValue: {
            type: Boolean,
            required: true,
        },
        itemId: {
            type: Number,
            required: true,
        },
        itemName: {
            type: String,
            required: true,
        },
        action: {
            type: Number,
            required: true,
            validator: (value: number) => [0, 1, 2].includes(value)
        },
        moduleName: {
            type: String,
            required: true,
        },
        entityName: {
            type: String,
            required: true,
        },
        gender: {
            type: String as PropType<'male' | 'female'>,
            default: 'male',
            validator: (value: string) => ['male', 'female'].includes(value)
        }
    },
    emits: ['update:modelValue', 'action-completed'],
    data() {
        return {
            isOpen: this.modelValue,
            processing: false,
            toast: useToast(),
        };
    },
    watch: {
        modelValue(newValue: boolean) {
            this.isOpen = newValue;
        }
    },
    methods: {
        close() {
            this.isOpen = false;
            this.$emit('update:modelValue', false);
        },

        async handleAction() {
            this.processing = true;

            const genderSuffix = this.gender === 'female' ? 'a' : 'o';

            const actionMap = {
                0: {
                    storeAction: `${this.moduleName}/remove${this.entityName}`,
                    successMsg: `${this.entityName} eliminad${genderSuffix} con éxito!`,
                    errorMsg: `Error al eliminar ${this.entityName.toLowerCase()}`
                },
                1: {
                    storeAction: `${this.moduleName}/enable${this.entityName}`,
                    successMsg: `${this.entityName} habilitad${genderSuffix} con éxito!`,
                    errorMsg: `Error al habilitar ${this.entityName.toLowerCase()}`
                },
                2: {
                    storeAction: `${this.moduleName}/disable${this.entityName}`,
                    successMsg: `${this.entityName} deshabilitad${genderSuffix} con éxito!`,
                    errorMsg: `Error al deshabilitar ${this.entityName.toLowerCase()}`
                }
            };

            const currentAction = actionMap[this.action as keyof typeof actionMap];

            try {
                const result = await this.$store.dispatch(currentAction.storeAction, this.itemId);

                if (result.isSuccess) {
                    this.toast.success(currentAction.successMsg);
                    this.$emit('action-completed');
                    this.close();
                }
            } catch (error: any) {
                handleApiError(error, currentAction.errorMsg);
            } finally {
                this.processing = false;
            }
        },
    }
});
</script>