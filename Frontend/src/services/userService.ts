import axios from 'axios';
import { User } from '@/models/userModel';

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
  token?: string
): Promise<{ items: User[]; totalRecords: number }> {
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
  const response = await axios.post('api/Users', requestBody, configuration);
  return response.data.data;
}

export async function fetchUserByIdService(id: number, token: string): Promise<User> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  const response = await axios.get(`api/Users/${id}`, configuration);
  return response.data;
}

export async function registerUserService(user: User, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.post("api/Users/Register", user, configuration);
}

export async function editUserService(id: number, user: User, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Users/Edit/${id}`, user, configuration);
}

export async function enableUserService(id: number, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Users/Enable/${id}`, {}, configuration);
}
export async function disableUserService(id: number, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Users/Disable/${id}`, {}, configuration);
}

export async function removeUserService(id: number, token: string): Promise<void> {

  const configuration = token ? { headers: { Authorization: `Bearer ${token}` } } : {};
  await axios.put(`api/Users/Remove/${id}`, {}, configuration);
}