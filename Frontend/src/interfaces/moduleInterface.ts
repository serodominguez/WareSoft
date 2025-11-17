export interface Module {
  idModule: number | null;
  moduleName: string;
  auditCreateDate: string;
  statusModule: string;
}

export interface FilterParams {
  pageNumber?: number;
  pageSize?: number;
  order?: string;
  sort?: string;
  textFilter?: string | null;
  numberFilter?: number | null;
  stateFilter?: number;
  startDate?: string | null;
  endDate?: string | null;
}

export interface ModuleState {
  modules: Module[];
  selectedModule: Module | null;
  totalModules: number;
  loading: boolean;
  error: string | null;
  lastFilterParams?: FilterParams;
}

export interface BaseResponse{
  isSuccess: boolean;
  data: any;
  totalRecords: number;
  message: any;
  errors: any;
}