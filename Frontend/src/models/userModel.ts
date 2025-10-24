export interface User {
  pK_USER: number | null;
  useR_NAME: string;
  password: string;
  passworD_HASH: string;
  names: string;
  lasT_NAMES: string;
  identificatioN_NUMBER: string;
  phonE_NUMBER: number | null;
  pK_ROLE: number | null;
  rolE_NAME: string,
  pK_STORE: number | null;
  storE_NAME: string,
  audiT_CREATE_DATE: string;
  statE_USER: string;
  updatE_PASSWORD: boolean;
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