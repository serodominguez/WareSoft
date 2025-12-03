using Application.Dtos.Request.GoodsReceipt;
using Application.Dtos.Response.GoodsReceipt;
using Domain.Entities;
using Infrastructure.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class GoodsReceiptMapp
    {
        public static GoodsReceiptEntity GoodsReceiptMapping(GoodsReceiptRequestDto dto)
        {
            return new GoodsReceiptEntity
            {
                PurchaseDate = dto.PurchaseDate,
                Type = dto.Type.NormalizeString(),
                DocumentType = dto.DocumentType.NormalizeString(),
                DocumentNumber = dto.DocumentNumber.NormalizeString(),
                TotalAmount = dto.TotalAmount,
                Annotations = dto.Annotations.NormalizeString(),
                IdSupplier = dto.IdSupplier,
                IdStore = dto.IdStore,
                GoodsReceiptDetails = dto.GoodsReceiptDetails
                    .Select(details => new GoodsReceiptDetailsEntity
                    {
                        Item = details.Item,
                        IdProduct = details.IdProduct,
                        Quantity = details.Quantity,
                        UnitPrice = details.UnitPrice,
                        TotalPrice = details.TotalPrice
                    }).ToList()
            };
        }

        public static GoodsReceiptResponseDto GoodsReceiptResponseDtoMapping(GoodsReceiptEntity entity)
        {
            return new GoodsReceiptResponseDto
            {
                IdReceipt = entity.IdReceipt,
                Code = entity.Code,
                PurchaseDate = entity.PurchaseDate.ToString("dd/MM/yyyy HH:mm"),
                Type = entity.Type.ToTitleCase(),
                DocumentType = entity.DocumentType.ToTitleCase(),
                DocumentNumber = entity.DocumentNumber,
                TotalAmount = entity.TotalAmount,
                Annotations = entity.Annotations.NormalizeString(),
                IdSupplier = entity.IdSupplier,
                CompanyName = entity.Supplier.CompanyName,
                IdStore = entity.IdStore,
                StoreName = entity.Store.StoreName,
                AuditCreateUser = entity.AuditCreateUser,
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Status = entity.Status,
                StatusReceipt = ((StateTypes)(entity.Status ? 1 : 0)).ToString()
            };
        }

        public static GoodsReceiptWithDetailsResponseDto GoodsReceiptWithDetailsResponseDtoMapping(GoodsReceiptEntity entity, string? userName = null)
        {
            return new GoodsReceiptWithDetailsResponseDto
            {
                IdReceipt = entity.IdReceipt,
                Code = entity.Code,
                PurchaseDate = entity.PurchaseDate.ToString("dd/MM/yyyy HH:mm"),
                Type = entity.Type.ToTitleCase(),
                DocumentType = entity.DocumentType.ToTitleCase(),
                DocumentNumber = entity.DocumentNumber,
                TotalAmount = entity.TotalAmount,
                Annotations = entity.Annotations.NormalizeString(),
                IdSupplier = entity.IdSupplier,
                CompanyName = entity.Supplier.CompanyName,
                IdStore = entity.IdStore,
                StoreName = entity.Store.StoreName,
                AuditCreateUser = entity.AuditCreateUser,
                AuditCreateName = userName,
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusReceipt = ((StateTypes)(entity.Status ? 1 : 0)).ToString(),
                GoodsReceiptDetailsResponseDto = entity.GoodsReceiptDetails
                        .Select(d => new GoodsReceiptDetailsResponseDto
                        {
                            Item = d.Item,
                            IdProduct = d.IdProduct,
                            Code = d.Product.Code,
                            Description = d.Product.Description,
                            CategoryName = d.Product.Category.CategoryName,
                            BrandName = d.Product.Brand.BrandName,
                            Quantity = d.Quantity,
                            UnitPrice = d.UnitPrice,
                            TotalPrice = d.TotalPrice
                        })
                        .ToList()

            };
        }
    }
}
