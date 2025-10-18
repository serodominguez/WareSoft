import axios from 'axios';
import { Store } from '@/models/storeModel';

export async function fetchStoresService(
  pageNumber = 1, 
  pageSize = 10, 
  order = "desc", 
  sort = "PK_STORE", 
  textFilter?: string | null, 
  numberFilter?: number | null,
  stateFilter: number = 1,
  startDate?: string | null,
  endDate?: string | null,
  token?: string
): Promise<{ items: Store[]; totalRecords: number }> {
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
  const response = await axios.post('api/Stores', requestBody, configuration);
  return response.data.data;
}

export async function selectStoreService(token: string): Promise<Store[]> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.get("api/Stores/Select", configuration);
  return response.data.data;
}

export async function fetchStoreByIdService(id: number, token: string): Promise<Store> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.get(`api/Stores/${id}`, configuration);
  return response.data;
}

export async function registerStoreService(store: Store, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.post("api/Stores/Register", store, configuration);
}

export async function editStoreService(id: number, store: Store, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Stores/Edit/${id}`, store, configuration);
}

export async function enableStoreService(id: number, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Stores/Enable/${id}`, {}, configuration);
}
export async function disableStoreService(id: number, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Stores/Disable/${id}`, {}, configuration);
}

export async function removeStoreService(id: number, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Stores/Remove/${id}`, {}, configuration);
}