export interface Permission {
  pK_PERMISSION : number;
  pK_ROLE: number;
  pK_MODULE: number;
  modulE_NAME: string;
  pK_ACTION: number;
  actioN_NAME: string;
  state: boolean;
}

export interface PermissionResponse {
  isSuccess: boolean;
  data: Permission[];
  totalRecords: number | null;
  message: string;
  errors: string[] | null;
}

export interface PermissionsByModule {
  module: string;
  permissions: {
    crear: boolean;
    leer: boolean;
    editar: boolean;
    eliminar: boolean;
  };
}
