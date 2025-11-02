import axios from 'axios';
import { Brand, BaseResponse } from '@/interfaces/brandInterface';

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
  download: true
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
  download?: false
): Promise<BaseResponse>;

export async function fetchBrandsService(
  pageNumber = 1, 
  pageSize = 10, 
  order = "desc", 
  sort = "PK_ENTITY", 
  textFilter?: string | null, 
  numberFilter?: number | null,
  stateFilter: number = 1,
  startDate?: string | null,
  endDate?: string | null,
  download: boolean = false
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

  const configuration: any = { params };

  if (download) {
    configuration.responseType = 'blob';
    const response = await axios.get('api/Brands', configuration);
    return response.data;
  }

  const response = await axios.get<BaseResponse>('api/Brands', configuration);
  return response.data;
}

export async function selectBrandService(): Promise<BaseResponse> {
  const response = await axios.get<BaseResponse>("api/Brands/Select");
  return response.data;
}

export async function fetchBrandByIdService(id: number): Promise<BaseResponse> {
  const response = await axios.get<BaseResponse>(`api/Brands/${id}`);
  return response.data;
}

export async function registerBrandService(brand: Brand): Promise<BaseResponse> {
  const response = await axios.post<BaseResponse>("api/Brands/Register", brand);
  return response.data;
}

export async function editBrandService(id: number, brand: Brand): Promise<BaseResponse> {
  const response = await axios.put<BaseResponse>(`api/Brands/Edit/${id}`, brand);
  return response.data;
}

export async function enableBrandService(id: number): Promise<BaseResponse> {
  const response = await axios.put<BaseResponse>(`api/Brands/Enable/${id}`, {});
  return response.data;
}

export async function disableBrandService(id: number): Promise<BaseResponse> {
  const response = await axios.put<BaseResponse>(`api/Brands/Disable/${id}`, {});
  return response.data;
}

export async function removeBrandService(id: number): Promise<BaseResponse> {
  const response = await axios.put<BaseResponse>(`api/Brands/Remove/${id}`, {});
  return response.data;
}