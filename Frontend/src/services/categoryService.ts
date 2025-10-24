import axios from 'axios';
import { Category, BaseResponse } from '@/models/categoryModel';

export async function fetchCategoriesService(
  pageNumber: number,
  pageSize: number,
  order: string,
  sort: string,
  textFilter: string | null | undefined,
  numberFilter: number | null | undefined,
  stateFilter: number,
  startDate: string | null | undefined,
  endDate: string | null | undefined,
  download: true,
  token?: string
): Promise<Blob>;

export async function fetchCategoriesService(
  pageNumber: number,
  pageSize: number,
  order: string,
  sort: string,
  textFilter: string | null | undefined,
  numberFilter: number | null | undefined,
  stateFilter: number,
  startDate: string | null | undefined,
  endDate: string | null | undefined,
  download?: false,
  token?: string
): Promise<BaseResponse>;

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
  download: boolean = false,
  token?: string
): Promise<BaseResponse | Blob> {
  const params: any = {
    NumberPage: pageNumber,
    NumberRecordsPage: pageSize,
    Order: order,
    Sort: sort,
    StateFilter: stateFilter,
    Download: download
  };

  if (textFilter && numberFilter) {
    params.TextFilter = textFilter;
    params.NumberFilter = numberFilter;
  }

  if (startDate) {
    params.StartDate = startDate;
  }

  if (endDate) {
    params.EndDate = endDate;
  }

  const configuration: any = {
    headers: token ? { Authorization: `Bearer ${token}` } : {},
    params: params
  };

  if (download) {
    configuration.responseType = 'blob';
    const response = await axios.get('api/Categories', configuration);
    return response.data;
  }

  const response = await axios.get<BaseResponse>('api/Categories', configuration);
  return response.data;
}

export async function selectCategoryService(token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.get<BaseResponse>("api/Categories/Select", configuration);
  return response.data;
}

export async function fetchCategoryByIdService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.get<BaseResponse>(`api/Categories/${id}`, configuration);
  return response.data;
}

export async function registerCategoryService(category: Category, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.post<BaseResponse>("api/Categories/Register", category, configuration);
  return response.data;
}

export async function editCategoryService(id: number, category: Category, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Categories/Edit/${id}`, category, configuration);
  return response.data;
}

export async function enableCategoryService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Categories/Enable/${id}`, {}, configuration);
  return response.data;;
}
export async function disableCategoryService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Categories/Disable/${id}`, {}, configuration);
  return response.data;
}

export async function removeCategoryService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Categories/Remove/${id}`, {}, configuration);
  return response.data;
}