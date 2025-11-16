export interface Store {
  idStore: number | null;
  storeName: string;
  manager: string;
  address: string;
  phoneNumber: number | null;
  city: string;
  email: string;
  type: string;
  auditCreateDate: string;
  statusStore: string;
}

export interface StoreState {
  stores: Store[];
  selectedStore: Store | null;
  totalStores: number;
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