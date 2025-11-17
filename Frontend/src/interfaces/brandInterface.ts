export interface Brand {
  idBrand: number | null;
  brandName: string;
  auditCreateDate: string;
  statusBrand: string;
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

export interface BrandState {
  brands: Brand[];
  selectedBrand: Brand | null;
  totalBrands: number;
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