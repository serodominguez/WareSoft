import axios from 'axios';
import { Module, BaseResponse } from '@/interfaces/moduleInterface';

export async function fetchModulesService(
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

export async function fetchModulesService(
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

export async function fetchModulesService(
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
    const response = await axios.get('api/Module', configuration);
    return response.data;
  }

  const response = await axios.get<BaseResponse>('api/Module', configuration);
  return response.data;
}

export async function fetchModuleByIdService(id: number, token: string): Promise<BaseResponse> {

  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.get<BaseResponse>(`api/Module/${id}`, configuration);
  return response.data;
}

export async function registerModuleService(module: Module, token: string): Promise<BaseResponse> {
  
  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.post<BaseResponse>("api/Module/Register", module, configuration);
  return response.data;
}

export async function editModuleService(id: number, module: Module, token: string): Promise<BaseResponse> {
  
  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.put<BaseResponse>(`api/Module/Edit/${id}`, module, configuration);
  return response.data;
}

export async function enableModuleService(id: number, token: string): Promise<BaseResponse> {
    
  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.put<BaseResponse>(`api/Module/Enable/${id}`, {}, configuration);
  return response.data;
}
export async function disableModuleService(id: number, token: string): Promise<BaseResponse> {

  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.put<BaseResponse>(`api/Module/Disable/${id}`, {}, configuration);
  return response.data;
}

export async function removeModuleService(id: number, token: string): Promise<BaseResponse> {
  
  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.put<BaseResponse>(`api/Module/Remove/${id}`, {}, configuration);
  return response.data;
}