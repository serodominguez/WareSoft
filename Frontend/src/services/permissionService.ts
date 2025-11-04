import axios from 'axios';
import { PermissionResponse } from '@/interfaces/permissionInterface';


export async function fetchPermissionsByRole(roleId: number): Promise<PermissionResponse> {
    const response = await axios.get<PermissionResponse>(`api/Permissions/Role/${roleId}`);
    return response.data;
}

export async function updatePermissions(permissions: Array<{pK_PERMISSION: number, state: boolean}>): Promise<any> {
    const response = await axios.put('api/Permissions/Update', permissions);
    return response.data;
}