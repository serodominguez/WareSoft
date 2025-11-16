import axios from 'axios';
import { Store, BaseResponse } from '@/interfaces/storeInterface';

export async function fetchStoresService(
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

export async function fetchStoresService(
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

export async function fetchStoresService(
  pageNumber = 1, 
  pageSize = 10, 
  order = "desc", 
  sort = "Id", 
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
    params.textFilter = textFilter;
    params.numberFilter = numberFilter;
  }

  if (startDate) {
    params.startDate = startDate;
  }

  if (endDate) {
    params.endDate = endDate;
  }

  const configuration: any = {
    headers: token ? { Authorization: `Bearer ${token}` } : {},
    params: params
  };

  if (download) {
    configuration.responseType = 'blob';
    const response = await axios.get('api/Store', configuration);
    return response.data;
  }

  const response = await axios.get<BaseResponse>('api/Store', configuration);
  return response.data;
}

export async function selectStoreService(token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.get<BaseResponse>("api/Store/Select", configuration);
  return response.data;
}

export async function fetchStoreByIdService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.get<BaseResponse>(`api/Store/${id}`, configuration);
  return response.data;
}

export async function registerStoreService(store: Store, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.post<BaseResponse>("api/Store/Register", store, configuration);
  return response.data;
}

export async function editStoreService(id: number, store: Store, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Store/Edit/${id}`, store, configuration);
  return response.data;
}

export async function enableStoreService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Store/Enable/${id}`, {}, configuration);
  return response.data;
}
export async function disableStoreService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Store/Disable/${id}`, {}, configuration);
  return response.data;
}

export async function removeStoreService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Store/Remove/${id}`, {}, configuration);
  return response.data;
}