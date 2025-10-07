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
  endDate?: string | null
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
  
  const response = await axios.post('api/Roles', requestBody);
  return response.data.data;
}

export async function selectRoleService(): Promise<Role[]> {
  const response = await axios.get("api/Roles/Select");
  return response.data;
}

export async function fetchRoleByIdService(id: number): Promise<Role> {
  const response = await axios.get(`api/Roles/${id}`);
  return response.data;
}

export async function registerRoleService(role: Role): Promise<void> {
  await axios.post("api/Roles/Register", role);
}

export async function editRoleService(id: number, role: Role): Promise<void> {
  await axios.put(`api/Roles/Edit/${id}`, role);
}

export async function enableRoleService(id: number): Promise<void> {
  await axios.put(`api/Roles/Enable/${id}`);
}
export async function disableRoleService(id: number): Promise<void> {
  await axios.put(`api/Roles/Disable/${id}`);
}

export async function removeRoleService(id: number): Promise<void> {
  await axios.put(`api/Roles/Remove/${id}`);
}