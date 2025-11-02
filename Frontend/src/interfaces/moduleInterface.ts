export interface Module {
  pK_MODULE: number | null;
  modulE_NAME: string;
  audiT_CREATE_DATE: string;
  statE_MODULE: string;
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