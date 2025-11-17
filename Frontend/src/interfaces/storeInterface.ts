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

export interface StoreState {
  stores: Store[];
  selectedStore: Store | null;
  totalStores: number;
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