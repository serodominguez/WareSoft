import axios from 'axios';
import { Category } from '@/models/categoryModel';

export async function fetchCategoriesService(
  pageNumber = 1, 
  pageSize = 5, 
  order = "desc", 
  sort = "PK_CATEGORY", 
  textFilter?: string | null, 
  numberFilter?: number | null
): Promise<{ items: Category[]; totalRecords: number }> {
  const requestBody: any = {
    numberPage: pageNumber,
    numberRecordsPage: pageSize,
    order,
    sort,
  };

    if (textFilter && numberFilter) {
    requestBody.textFilter = textFilter;
    requestBody.numberFilter = numberFilter;
  }
  
  const response = await axios.post('api/Categories', requestBody);
  return response.data.data;
}

export async function selectCategoryService(): Promise<Category[]> {
  const response = await axios.get("api/Categories/Select");
  return response.data;
}

export async function fetchCategoryByIdService(id: number): Promise<Category> {
  const response = await axios.get(`api/Categories/${id}`);
  return response.data;
}

export async function registerCategoryService(category: Category): Promise<void> {
  await axios.post("api/Categories/Register", category);
}

export async function editCategoryService(id: number, category: Category): Promise<void> {
  await axios.put(`api/Categories/Edit/${id}`, category);
}

export async function removeCategoryService(id: number): Promise<void> {
  await axios.put(`api/Categories/Remove/${id}`);
}
