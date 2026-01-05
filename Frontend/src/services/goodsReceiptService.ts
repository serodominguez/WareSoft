import axios from "axios";
import { BaseService } from "./baseService";
import { GoodsReceipt, GoodsReceiptDetail, GoodsReceiptRegister } from '@/interfaces/goodsReceiptInterface'
import { BaseResponse } from '@/interfaces/baseInterface';

class GoodsReceiptService extends BaseService<GoodsReceipt> {
  constructor() {
    super({
      endpoint: 'GoodsReceipt',
      downloadFileName: 'Entradas',
      customEndpoints: {
        create: 'GoodsReceipt/Register',
        // No hay endpoints de actualización o habilitación/deshabilitación
        update: undefined,
        enable: undefined,
        disable: undefined,
        remove: undefined
      }
    });
  }

  // Registrar nueva entrada de productos
  async register(data: GoodsReceiptRegister): Promise<BaseResponse<any>> {
    const response = await axios.post<BaseResponse<any>>('api/GoodsReceipt/Register', data);
    return response.data;
  }

  // Anular una entrada de productos
  async cancel(receiptId: number): Promise<BaseResponse<void>> {
    const response = await axios.put<BaseResponse<void>>(`api/GoodsReceipt/Cancel/${receiptId}`, {});
    return response.data;
  }

  // Obtener detalles de una entrada específica
  async getReceiptWithDetails(receiptId: number): Promise<BaseResponse<any>> {
    const response = await axios.get<BaseResponse<any>>(
      `api/GoodsReceipt/${receiptId}`
    );
    return response.data;
  }

  // Exportar PDF de una entrada
  async exportPdf(receiptId: number): Promise<Blob> {
    const response = await axios.get(
      `api/GoodsReceipt/ExportPdf/${receiptId}`,
      { responseType: 'blob' }
    );
    return response.data;
  }
}

export const goodsReceiptService = new GoodsReceiptService();