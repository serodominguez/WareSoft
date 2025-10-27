export {}

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $hasPermission: (module: string, action: string) => boolean
  }
}