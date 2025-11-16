export interface Role {
  idRole: number | null;
  roleName: string;
  auditCreateDate: string;
  statusRole: string;
}

export interface RoleState {
  roles: Role[];
  selectedRole: Role | null;
  totalRoles: number;
  loading: boolean;
  error: string | null;
}

export interface BaseResponse{
  isSuccess: boolean;
  data: any;
  totalRecords: number;
  message: any;
  errors: any;
}