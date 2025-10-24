import axios from 'axios';
import { Brand, BaseResponse } from '@/models/brandModel';

export async function fetchBrandsService(
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

export async function fetchBrandsService(
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
    const response = await axios.get('api/Brands', configuration);
    return response.data;
  }

  const response = await axios.get<BaseResponse>('api/Brands', configuration);
  return response.data;
}

export async function selectBrandService(token: string): Promise<BaseResponse> {
  
  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.get<BaseResponse>("api/Brands/Select", configuration);
  return response.data;
}

export async function fetchBrandByIdService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.get<BaseResponse>(`api/Brands/${id}`, configuration);
  return response.data;
}

export async function registerBrandService(brand: Brand, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.post<BaseResponse>("api/Brands/Register", brand, configuration);
  return response.data;
}

export async function editBrandService(id: number, brand: Brand, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Brands/Edit/${id}`, brand, configuration);
  return response.data;
}

export async function enableBrandService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Brands/Enable/${id}`, {}, configuration);
  return response.data;
}
export async function disableBrandService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Brands/Disable/${id}`, {}, configuration);
  return response.data;
}

export async function removeBrandService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Brands/Remove/${id}`, {}, configuration);
  return response.data;
}