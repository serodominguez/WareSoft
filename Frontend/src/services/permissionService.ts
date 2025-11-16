import axios from 'axios';
import { PermissionResponse } from '@/interfaces/permissionInterface';


export async function fetchPermissionsByRole(roleId: number): Promise<PermissionResponse> {
    const response = await axios.get<PermissionResponse>(`api/Permission/Role/${roleId}`);
    return response.data;
}

export async function updatePermissions(permissions: Array<{idPermission: number, status: boolean}>): Promise<any> {
    const response = await axios.put('api/Permission/Update', permissions);
    return response.data;
}