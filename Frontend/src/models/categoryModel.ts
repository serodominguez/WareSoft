export interface Category {
  pK_CATEGORY: number | null;
  categorY_NAME: string;
  description: string;
  audiT_CREATE_DATE: string;
  statE_CATEGORY: string;
}

export interface CategoryState {
  categories: Category[];
  selectedCategory: Category | null;
  totalCategories: number;
  loading: boolean;
  error: string | null;
}