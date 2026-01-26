using Application.Dtos.Response.GoodsIssue;
using Application.Dtos.Response.GoodsReceipt;
using Application.Dtos.Response.Inventory;

namespace Application.Interfaces
{
    public interface IGeneratePdfService
    {
        byte[] GoodsIssueGeneratePdf(GoodsIssueWithDetailsResponseDto issue);
        byte[] GoodsReceiptGeneratePdf(GoodsReceiptWithDetailsResponseDto receipt);
        byte[] InventoryGeneratePdf(List<InventoryResponseDto> inventory, string storeName);
    }
}
