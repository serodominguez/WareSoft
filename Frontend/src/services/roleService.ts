import axios from 'axios';
import { Role, BaseResponse } from '@/interfaces/roleInterface';

export async function fetchRolesService(
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

export async function fetchRolesService(
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

export async function fetchRolesService(
  pageNumber = 1, 
  pageSize = 10, 
  order = "desc", 
  sort = "PK_ENTITY", 
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
    const response = await axios.get('api/Roles', configuration);
    return response.data;
  }

  const response = await axios.get<BaseResponse>('api/Roles', configuration);
  return response.data;
}

export async function selectRoleService(token: string): Promise<BaseResponse> {

  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.get<BaseResponse>("api/Roles/Select", configuration);
  return response.data;
}

export async function fetchRoleByIdService(id: number, token: string): Promise<BaseResponse> {

  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.get<BaseResponse>(`api/Roles/${id}`, configuration);
  return response.data;
}

export async function registerRoleService(role: Role, token: string): Promise<BaseResponse> {
  
  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.post<BaseResponse>("api/Roles/Register", role, configuration);
  return response.data;
}

export async function editRoleService(id: number, role: Role, token: string): Promise<BaseResponse> {
  
  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.put<BaseResponse>(`api/Roles/Edit/${id}`, role, configuration);
  return response.data;
}

export async function enableRoleService(id: number, token: string): Promise<BaseResponse> {
    
  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.put<BaseResponse>(`api/Roles/Enable/${id}`, {}, configuration);
  return response.data;
}
export async function disableRoleService(id: number, token: string): Promise<BaseResponse> {

  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.put<BaseResponse>(`api/Roles/Disable/${id}`, {}, configuration);
  return response.data;
}

export async function removeRoleService(id: number, token: string): Promise<BaseResponse> {
  
  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.put<BaseResponse>(`api/Roles/Remove/${id}`, {}, configuration);
  return response.data;
}