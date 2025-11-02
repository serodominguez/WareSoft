import axios from 'axios';
import { PermissionResponse } from '@/interfaces/permissionInterface';


export async function fetchPermissionsByRole(roleId: number): Promise<PermissionResponse> {
    const response = await axios.get<PermissionResponse>(`api/Permissions/Role/${roleId}`);
    return response.data;
}