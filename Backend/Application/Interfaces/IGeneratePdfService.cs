using Application.Dtos.Response.GoodsReceipt;

namespace Application.Interfaces
{
    public interface IGeneratePdfService
    {
        byte[] GoodsReceiptGeneratePdf(GoodsReceiptWithDetailsResponseDto receipt);
    }
}
