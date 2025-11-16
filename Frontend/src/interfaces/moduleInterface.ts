export interface Module {
  idModule: number | null;
  moduleName: string;
  auditCreateDate: string;
  statusModule: string;
}

export interface ModuleState {
  modules: Module[];
  selectedModule: Module | null;
  totalModules: number;
  loading: boolean;
  error: string | null;
}

export interface BaseResponse{
  isSuccess: boolean;
  data: any;
  totalRecords: number;
  message: any;
  errors: any;
}