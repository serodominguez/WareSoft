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
                el ítem: {{ item }}.
            </v-card-text>
            <v-card-actions class="d-flex justify-space-between">
                <div class="d-flex">
                    <v-btn v-if="action === 0" color="green" dark class="mr-2" elevation="4" @click="handleAction"
                        :loading="processing">
                        Eliminar
                    </v-btn>
                    <v-btn v-if="action === 1" color="green" dark class="mr-2" elevation="4" @click="handleAction"
                        :loading="processing">
                        Activar
                    </v-btn>
                    <v-btn v-if="action === 2" color="green" dark class="mr-2" elevation="4" @click="handleAction"
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
    // Define todos los parámetros necesarios para ejecutar una acción CRUD
    props: {
        // Controla la visibilidad del modal (patrón v-model)
        modelValue: {
            type: Boolean,
            required: true,
        },
        // ID del item sobre el que se realizará la acción
        itemId: {
            type: Number,
            required: true,
        },
        // Nombre del item a mostrar en el mensaje de confirmación
        item: {
            type: String,
            required: true,
        },
        // Tipo de acción: 0 = Eliminar, 1 = Activar, 2 = Desactivar
        action: {
            type: Number,
            required: true,
            validator: (value: number) => [0, 1, 2].includes(value)
        },
        // Nombre del módulo de Vuex (ej: 'brand', 'product', 'category')
        moduleName: {
            type: String,
            required: true,
        },
        // Nombre de la entidad para construir el action de Vuex (ej: 'Brand', 'Product')
        entityName: {
            type: String,
            required: true,
        },
        // Nombre legible de la entidad para mensajes (ej: 'Marca', 'Producto')
        name: {
            type: String,
            required: true,
        },
        // Género gramatical para conjugar correctamente los mensajes ('male' o 'female')
        gender: {
            type: String as PropType<'male' | 'female'>,
            default: 'male',
            validator: (value: string) => ['male', 'female'].includes(value)
        }
    },
    // Eventos que este componente puede emitir
    emits: ['update:modelValue', 'action-completed'],
    data() {
        return {
            isOpen: this.modelValue,        // Estado local de visibilidad del modal
            processing: false,             // Indica si se está procesando la acción
            toast: useToast(),            // Instancia de notificaciones toast
        };
    },
    watch: {
        // Sincroniza cambios del prop modelValue con el estado local isOpen
        modelValue(newValue: boolean) {
            this.isOpen = newValue;
        }
    },
    methods: {
        // Cierra el modal
        close() {
            this.isOpen = false;
            this.$emit('update:modelValue', false);
        },
        /**
         * Maneja la ejecución de la acción seleccionada (Eliminar/Activar/Desactivar)
         * 
         * Proceso:
         * 1. Determina el género gramatical para los mensajes
         * 2. Construye dinámicamente el nombre del action de Vuex según la acción
         * 3. Ejecuta el action correspondiente en el store
         * 4. Muestra notificación de éxito o error
         * 5. Emite evento al padre para actualizar la lista
         * 6. Cierra el modal
         */
        async handleAction() {
            this.processing = true;
            // Determina el sufijo gramatical según el género (eliminado/eliminada)
            const genderSuffix = this.gender === 'female' ? 'a' : 'o';
            /**
             * Mapa de configuración para cada tipo de acción
             * Define el action de Vuex y los mensajes personalizados según la acción
             * 
             * Ejemplo para moduleName='brand', entityName='Brand':
             * - action 0: Llama a 'brand/removeBrand'
             * - action 1: Llama a 'brand/enableBrand'
             * - action 2: Llama a 'brand/disableBrand'
             */
            const actionMap = {
                0: {
                    storeAction: `${this.moduleName}/remove${this.entityName}`,
                    successMsg: `${this.name} eliminad${genderSuffix} con éxito!`,
                    errorMsg: `Error al eliminar ${this.name.toLowerCase()}`
                },
                1: {
                    storeAction: `${this.moduleName}/enable${this.entityName}`,
                    successMsg: `${this.name} habilitad${genderSuffix} con éxito!`,
                    errorMsg: `Error al habilitar ${this.name.toLowerCase()}`
                },
                2: {
                    storeAction: `${this.moduleName}/disable${this.entityName}`,
                    successMsg: `${this.name} deshabilitad${genderSuffix} con éxito!`,
                    errorMsg: `Error al deshabilitar ${this.name.toLowerCase()}`
                }
            };
            // Obtiene la configuración para la acción actual
            const currentAction = actionMap[this.action as keyof typeof actionMap];

            try {
                // Ejecuta el action de Vuex correspondiente con el ID del item
                const result = await this.$store.dispatch(currentAction.storeAction, this.itemId);
                // Si la operación fue exitosa
                if (result.isSuccess) {
                    this.toast.success(currentAction.successMsg);    // Muestra notificación de éxito
                    this.$emit('action-completed');                 // Emite evento al padre
                    this.close();                                  // Cierra el modal
                }
            } catch (error: any) {
                // Manejo de errores usando helper centralizado
                handleApiError(error, currentAction.errorMsg);
            } finally {
                this.processing = false;
            }
        },
    }
});
</script>