import axios from 'axios';
import { Role } from '@/models/roleModel';

export async function fetchRolesService(
  pageNumber = 1, 
  pageSize = 10, 
  order = "desc", 
  sort = "PK_ROLE", 
  textFilter?: string | null, 
  numberFilter?: number | null,
  stateFilter: number = 1,
  startDate?: string | null,
  endDate?: string | null,
  token?: string
): Promise<{ items: Role[]; totalRecords: number }> {
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
  const response = await axios.post('api/Roles', requestBody, configuration);
  return response.data.data;
}

export async function selectRoleService(token: string): Promise<Role[]> {

  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.get("api/Roles/Select", configuration);
  return response.data.data;
}

export async function fetchRoleByIdService(id: number, token: string): Promise<Role> {

  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  const response = await axios.get(`api/Roles/${id}`, configuration);
  return response.data;
}

export async function registerRoleService(role: Role, token: string): Promise<void> {
  
  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  await axios.post("api/Roles/Register", role, configuration);
}

export async function editRoleService(id: number, role: Role, token: string): Promise<void> {
  
  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  await axios.put(`api/Roles/Edit/${id}`, role, configuration);
}

export async function enableRoleService(id: number, token: string): Promise<void> {
    
  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  await axios.put(`api/Roles/Enable/${id}`, {}, configuration);
}
export async function disableRoleService(id: number, token: string): Promise<void> {

  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  await axios.put(`api/Roles/Disable/${id}`, {}, configuration);
}

export async function removeRoleService(id: number, token: string): Promise<void> {
  
  const configuration = { headers: { Authorization: `Bearer ${token}` } };
  await axios.put(`api/Roles/Remove/${id}`, {}, configuration);
}