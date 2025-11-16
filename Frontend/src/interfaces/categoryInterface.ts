export interface Category {
  idCategory: number | null;
  categoryName: string;
  description: string;
  auditCreateDate: string;
  statusCategory: string;
}

export interface CategoryState {
  categories: Category[];
  selectedCategory: Category | null;
  totalCategories: number;
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