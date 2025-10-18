import axios from 'axios';
import { Brand } from '@/models/brandModel';

export async function fetchBrandsService(
  pageNumber = 1, 
  pageSize = 10, 
  order = "desc", 
  sort = "PK_BRAND", 
  textFilter?: string | null, 
  numberFilter?: number | null,
  stateFilter: number = 1,
  startDate?: string | null,
  endDate?: string | null,
  token?: string
): Promise<{ items: Brand[]; totalRecords: number }> {
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
  const response = await axios.post('api/Brands', requestBody, configuration);
  return response.data.data;
}

export async function selectBrandService(token: string): Promise<Brand[]> {
  
  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.get("api/Brands/Select", configuration);
  return response.data;
}

export async function fetchBrandByIdService(id: number, token: string): Promise<Brand> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.get(`api/Brands/${id}`, configuration);
  return response.data;
}

export async function registerBrandService(brand: Brand, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.post("api/Brands/Register", brand, configuration);
}

export async function editBrandService(id: number, brand: Brand, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Brands/Edit/${id}`, brand, configuration);
}

export async function enableBrandService(id: number, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Brands/Enable/${id}`, {}, configuration);
}
export async function disableBrandService(id: number, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Brands/Disable/${id}`, {}, configuration);
}

export async function removeBrandService(id: number, token: string): Promise<void> {

    const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Brands/Remove/${id}`, {}, configuration);
}