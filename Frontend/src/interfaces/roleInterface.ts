export interface Role {
  pK_ROLE: number | null;
  rolE_NAME: string;
  audiT_CREATE_DATE: string;
  statE_ROLE: string;
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