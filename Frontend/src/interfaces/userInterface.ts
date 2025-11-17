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

export interface FilterParams {
  pageNumber?: number;
  pageSize?: number;
  order?: string;
  sort?: string;
  textFilter?: string | null;
  numberFilter?: number | null;
  stateFilter?: number;
  startDate?: string | null;
  endDate?: string | null;
}

export interface UserState {
  users: User[];
  selectedUser: User | null;
  totalUsers: number;
  loading: boolean;
  error: string | null;
  lastFilterParams?: FilterParams;
}

export interface BaseResponse{
  isSuccess: boolean;
  data: any;
  totalRecords: number;
  message: any;
  errors: any;
}