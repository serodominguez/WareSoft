export interface User {
  idUser: number | null;
  userName: string;
  password: string;
  passwordHash: string;
  names: string;
  lastNames: string;
  identificationNumber: string;
  phoneNumber: number | null;
  idRole: number | null;
  roleName: string,
  idStore: number | null;
  storeName: string,
  auditCreateDate: string;
  statusUser: string;
  updatePassword: boolean;
}

export interface UserState {
  users: User[];
  selectedUser: User | null;
  totalUsers: number;
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