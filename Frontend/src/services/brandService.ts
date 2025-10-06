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
  endDate?: string | null
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
  
  const response = await axios.post('api/Brands', requestBody);
  return response.data.data;
}

export async function selectBrandService(): Promise<Brand[]> {
  const response = await axios.get("api/Brands/Select");
  return response.data;
}

export async function fetchBrandByIdService(id: number): Promise<Brand> {
  const response = await axios.get(`api/Brands/${id}`);
  return response.data;
}

export async function registerBrandService(brand: Brand): Promise<void> {
  await axios.post("api/Brands/Register", brand);
}

export async function editBrandService(id: number, brand: Brand): Promise<void> {
  await axios.put(`api/Brands/Edit/${id}`, brand);
}

export async function enableBrandService(id: number): Promise<void> {
  await axios.put(`api/Brands/Enable/${id}`);
}
export async function disableBrandService(id: number): Promise<void> {
  await axios.put(`api/Brands/Disable/${id}`);
}

export async function removeBrandService(id: number): Promise<void> {
  await axios.put(`api/Brands/Remove/${id}`);
}