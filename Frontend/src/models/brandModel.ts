export interface Brand {
  pK_BRAND: number | null;
  branD_NAME: string;
  audiT_CREATE_DATE: string;
  statE_BRAND: string;
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