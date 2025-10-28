export interface Store {
  pK_STORE: number | null;
  storE_NAME: string;
  manager: string;
  address: string;
  phonE_NUMBER: number | null;
  city: string;
  email: string;
  type: string;
  audiT_CREATE_DATE: string;
  statE_STORE: string;
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