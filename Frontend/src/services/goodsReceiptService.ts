import { BaseService } from "./baseService";
import { GoodsReceipt } from '@/interfaces/goodsReceiptInterface'

class GoodsReceiptService extends BaseService<GoodsReceipt> {
  constructor() {
    super({
      endpoint: 'GoodsReceipt',
      downloadFileName: 'Ingresos',
    });
  }
}

export const goodsReceiptService = new GoodsReceiptService();