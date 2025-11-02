export interface Permission {
  pK_ROLE: number;
  pK_MODULE: number;
  pK_ACTION: number;
  module: string;
  action: string;
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
