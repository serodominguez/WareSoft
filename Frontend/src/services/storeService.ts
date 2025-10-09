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
  endDate?: string | null
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
  
  const response = await axios.post('api/Stores', requestBody);
  return response.data.data;
}

export async function selectStoreService(): Promise<Store[]> {
  const response = await axios.get("api/Stores/Select");
  return response.data;
}

export async function fetchStoreByIdService(id: number): Promise<Store> {
  const response = await axios.get(`api/Stores/${id}`);
  return response.data;
}

export async function registerStoreService(store: Store): Promise<void> {
  console.log(store);
  await axios.post("api/Stores/Register", store);
}

export async function editStoreService(id: number, store: Store): Promise<void> {
  await axios.put(`api/Stores/Edit/${id}`, store);
}

export async function enableStoreService(id: number): Promise<void> {
  await axios.put(`api/Stores/Enable/${id}`);
}
export async function disableStoreService(id: number): Promise<void> {
  await axios.put(`api/Stores/Disable/${id}`);
}

export async function removeStoreService(id: number): Promise<void> {
  await axios.put(`api/Stores/Remove/${id}`);
}