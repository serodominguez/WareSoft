export interface GoodsReceipt {
  idReceipt: number | null;
  code: string;
  type: string;
  storeName: string;
  idSupplier: number | null;
  companyName: string;
  documentDate: string;
  documentType: string;
  documentNumber: string;
  annotations: string;
  auditCreateDate: string;
  statusReceipt: string;
}

export interface GoodsReceiptDetail {
  idProduct: number;
  quantity: number;
  cost: number;
}

export interface GoodsReceiptRegister {
  documentDate: string | null;
  type: string;
  documentType: string;
  documentNumber: string;
  annotations: string;
  idSupplier: number | null;
  idUser: number;
  idStore: number;
  details: GoodsReceiptDetail[];
}