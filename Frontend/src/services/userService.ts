import axios from 'axios';
import { User, BaseResponse } from '@/models/userModel';

export async function fetchUsersService(
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

export async function fetchUsersService(
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

export async function fetchUsersService(
  pageNumber = 1, 
  pageSize = 10, 
  order = "desc", 
  sort = "PK_USER", 
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
    const response = await axios.get('api/Users', configuration);
    return response.data;
  }

  const response = await axios.get<BaseResponse>('api/Users', configuration);
  return response.data;
}

export async function fetchUserByIdService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.get<BaseResponse>(`api/Users/${id}`, configuration);
  return response.data;
}

export async function registerUserService(user: User, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.post<BaseResponse>("api/Users/Register", user, configuration);
  return response.data;
}

export async function editUserService(id: number, user: User, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Users/Edit/${id}`, user, configuration);
  return response.data;
}

export async function enableUserService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Users/Enable/${id}`, {}, configuration);
  return response.data;
}
export async function disableUserService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Users/Disable/${id}`, {}, configuration);
  return response.data;
}

export async function removeUserService(id: number, token: string): Promise<BaseResponse> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.put<BaseResponse>(`api/Users/Remove/${id}`, {}, configuration);
  return response.data;
}