using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGoodsReceiptRepository
    {
        Task<string> GenerateCodeAsync();
        IQueryable<GoodsReceiptEntity> GetGoodsReceiptQueryable();
        Task<GoodsReceiptEntity?> GetGoodsReceiptByIdAsync(int receiptId);
        public Task<bool> RegisterGoodsReceiptAsync(GoodsReceiptEntity entity);
    }
}
