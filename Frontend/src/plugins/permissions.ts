import { App, DirectiveBinding } from 'vue'
import store from '@/store'

interface PermissionBinding {
  module: string
  action: string
}

export default {
  install(app: App) {
    // Directiva v-permission
    app.directive('permission', {
      mounted(el: HTMLElement, binding: DirectiveBinding<PermissionBinding>) {
        const { module, action } = binding.value

        if (!module || !action) {
          console.error('v-permission requiere module y action')
          return
        }

        const hasPermission = store.getters.hasPermission(module, action)

        if (!hasPermission) {
          // Ocultar el elemento
          el.style.display = 'none'
          
          // O eliminarlo completamente (opcional)
          // el.parentNode?.removeChild(el)
        }
      },
      
      // Actualizar cuando cambie el binding
      updated(el: HTMLElement, binding: DirectiveBinding<PermissionBinding>) {
        const { module, action } = binding.value

        if (!module || !action) return

        const hasPermission = store.getters.hasPermission(module, action)

        if (!hasPermission) {
          el.style.display = 'none'
        } else {
          el.style.display = ''
        }
      }
    })

    // Helper global para verificar permisos (opcional)
    app.config.globalProperties.$hasPermission = (module: string, action: string): boolean => {
      return store.getters.hasPermission(module, action)
    }
  }
}