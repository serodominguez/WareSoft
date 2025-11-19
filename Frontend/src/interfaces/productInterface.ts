export interface Product {
  idProduct: number | null;
  code: string;
  description: string;
  material: string;
  color: string;
  unitMeasure: string;
  idBrand: number | null;
  brandName: string,
  idCategory: number | null;
  categoryName: string,
  auditCreateDate: string;
  statusProduct: string;
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

export interface ProductState {
  products: Product[];
  selectedProduct: Product | null;
  totalProducts: number;
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