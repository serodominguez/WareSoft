export interface Category {
  id: number;
  name: string;
  enabled: boolean;
}

export interface CategoryState {
  categories: Category[];
  selectedCategory: Category | null;
  loading: boolean;
  error: string | null;
}