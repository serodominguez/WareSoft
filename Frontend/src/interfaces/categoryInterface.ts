export interface Category {
  idCategory: number | null;
  categoryName: string;
  description: string;
  auditCreateDate: string;
  statusCategory: string;
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

export interface CategoryState {
  categories: Category[];
  selectedCategory: Category | null;
  totalCategories: number;
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