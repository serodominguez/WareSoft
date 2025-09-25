export interface Category {
  pK_CATEGORY: number | null;
  categorY_NAME: string;
  description?: string;
  audiT_CREATE_DATE?: string;
  state: boolean;
  statE_CATEGORY?: string;
}
export interface PaginatedResponse {
  isSuccess: boolean;
  data: {
    totalRecords: number;
    items: Category[];
  };
  message: string;
  errors: any;
}
export interface CategoryState {
  categories: Category[];
  totalRecords: number;
  currentPage: number;
  itemsPerPage: number;
  selectedCategory: Category | null;
  loading: boolean;
  error: string | null;
}