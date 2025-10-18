import axios from 'axios';
import { Category } from '@/models/categoryModel';

export async function fetchCategoriesService(
  pageNumber = 1, 
  pageSize = 10, 
  order = "desc", 
  sort = "PK_CATEGORY", 
  textFilter?: string | null, 
  numberFilter?: number | null,
  stateFilter: number = 1,
  startDate?: string | null,
  endDate?: string | null,
  token?: string
): Promise<{ items: Category[]; totalRecords: number }> {
  const requestBody: any = {
    numberPage: pageNumber,
    numberRecordsPage: pageSize,
    order,
    sort,
    stateFilter
  };

  if (textFilter && numberFilter) {
    requestBody.textFilter = textFilter;
    requestBody.numberFilter = numberFilter;
  }

  if (startDate) {
    requestBody.startDate = startDate;
  }

  if (endDate) {
    requestBody.endDate = endDate;
  }

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.post('api/Categories', requestBody, configuration);
  return response.data.data;
}

export async function selectCategoryService(token: string): Promise<Category[]> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.get("api/Categories/Select", configuration);
  return response.data;
}

export async function fetchCategoryByIdService(id: number, token: string): Promise<Category> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.get(`api/Categories/${id}`, configuration);
  return response.data;
}

export async function registerCategoryService(category: Category, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.post("api/Categories/Register", category, configuration);
}

export async function editCategoryService(id: number, category: Category, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Categories/Edit/${id}`, category, configuration);
}

export async function enableCategoryService(id: number, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Categories/Enable/${id}`, {}, configuration);
}
export async function disableCategoryService(id: number, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Categories/Disable/${id}`, {}, configuration);
}

export async function removeCategoryService(id: number, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Categories/Remove/${id}`, {}, configuration);
}