export interface Brand {
  idBrand: number | null;
  brandName: string;
  auditCreateDate: string;
  statusBrand: string;
}

export interface BrandState {
  brands: Brand[];
  selectedBrand: Brand | null;
  totalBrands: number;
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