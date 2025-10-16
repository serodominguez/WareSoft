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
  endDate?: string | null
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
  
  const response = await axios.post('api/Users', requestBody);
  return response.data.data;
}

export async function fetchUserByIdService(id: number): Promise<User> {
  const response = await axios.get(`api/Users/${id}`);
  return response.data;
}

export async function registerUserService(user: User): Promise<void> {
  await axios.post("api/Users/Register", user);
}

export async function editUserService(id: number, user: User): Promise<void> {
  await axios.put(`api/Users/Edit/${id}`, user);
}

export async function enableUserService(id: number): Promise<void> {
  await axios.put(`api/Users/Enable/${id}`);
}
export async function disableUserService(id: number): Promise<void> {
  await axios.put(`api/Users/Disable/${id}`);
}

export async function removeUserService(id: number): Promise<void> {
  await axios.put(`api/Users/Remove/${id}`);
}