export interface GoodsReceipt {
  idReceipt: number | null;
  code: string;
  type: string,
  storeName: string,
  companyName: string,
  documentDate: string,
  documentType: string,
  auditCreateDate: string;
  statusReceipt: string;
}