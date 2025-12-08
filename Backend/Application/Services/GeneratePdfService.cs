using Application.Dtos.Response.GoodsReceipt;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Application.Services
{
    public class GeneratePdfService : IGeneratePdfService
    {
        private readonly IConfiguration _configuration;

        public GeneratePdfService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public byte[] GoodsReceiptGeneratePdf(GoodsReceiptWithDetailsResponseDto receipt)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
            });

            void ComposeHeader(IContainer container)
            {
                var titleStyle = TextStyle.Default.FontSize(15).Bold().FontColor(Colors.Black);

                container.Column(column =>
                {
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("Número de ingreso: ").Style(titleStyle);
                        text.Span(receipt.Code).Style(titleStyle);
                    });

                    column.Item().PaddingTop(15);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(leftColumn =>
                        {
                            leftColumn.Item().Text(text =>
                            {
                                text.Span("Fecha de Compra: ").SemiBold();
                                text.Span($"{receipt.PurchaseDate:d}");
                            });

                            leftColumn.Spacing(10);

                            leftColumn.Item().Text(text =>
                            {
                                text.Span("Proveedor: ").SemiBold();
                                text.Span($"{receipt.CompanyName}");
                            });
                        });

                        row.RelativeItem().Column(rightColumn =>
                        {
                            rightColumn.Item().AlignRight().Text(text =>
                            {
                                text.Span("Tipo de Documento: ").SemiBold();
                                text.Span($"{receipt.DocumentType}");
                            });

                            rightColumn.Spacing(10);

                            rightColumn.Item().AlignRight().Text(text =>
                            {
                                text.Span("Tienda: ").SemiBold();
                                text.Span($"{receipt.StoreName}");
                            });
                        });
                    });

                    //row.ConstantItem(100)
                    //    .Image(_configuration.GetSection("ImagePDF:Entreprise").Value)
                    //
                });
            }

            void ComposeContent(IContainer container)
            {
                container.PaddingVertical(40).Column(column =>
                {
                    column.Item().Element(ComposeTable);

                    column.Spacing(10);

                    column.Item().AlignRight().Text(text =>
                    {
                        text.Span("Total Bs.: ").SemiBold();
                        text.Span($"{receipt.TotalAmount}").Bold();
                    });

                    if (!string.IsNullOrWhiteSpace(receipt.Annotations))
                        column.Item().PaddingTop(25).Element(ComposeComments);
                });
            }

            void ComposeTable(IContainer container)
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(colums =>
                    {
                        colums.RelativeColumn(1);
                        colums.RelativeColumn(2);
                        colums.RelativeColumn(5);
                        colums.RelativeColumn(1.5f);
                        colums.RelativeColumn(1.5f);
                        colums.RelativeColumn((float)1.5);
                    });

                    table.Header(header => 
                    {
                        header.Cell().Element(CellStyle).Text("Nº");
                        header.Cell().Element(CellStyle).Text("Código");
                        header.Cell().Element(CellStyle).Text("Descripción");
                        header.Cell().Element(CellStyle).AlignRight().Text("Cantidad");
                        header.Cell().Element(CellStyle).AlignRight().Text("Precio U.");
                        header.Cell().Element(CellStyle).AlignRight().Text("Subtotal");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .DefaultTextStyle(x => x.SemiBold())
                                .PaddingVertical(5)
                                .BorderBottom(1)
                                .BorderColor(Colors.Black);
                        }
                    });

                    foreach (var item in receipt.GoodsReceiptDetails)
                    {
                        table.Cell().Element(CellStyle).Text(item.Item.ToString());
                        table.Cell().Element(CellStyle).Text(item.Code ?? string.Empty);
                        table.Cell().Element(CellStyle).Text(item.Description ?? string.Empty);
                        table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString());
                        table.Cell().Element(CellStyle).AlignRight().Text(item.UnitPrice.ToString("F2"));
                        table.Cell().Element(CellStyle).AlignRight().Text(item.TotalPrice.ToString("F2"));

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .PaddingVertical(1);
                        }
                    }
                });
            }

            void ComposeComments(IContainer container)
            {
                container
                    .Background(Colors.Grey.Lighten3)
                    .Padding(10)
                    .Column(column =>
                    {
                        column.Spacing(5);
                        column.Item().Text("Observaciones:").FontSize(14);
                        column.Item().Text(receipt.Annotations);
                    });
            }

            void ComposeFooter(IContainer container)
            {
                container.AlignCenter()
                    .Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(10));
                        text.Span("Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
            }

            var invoice = document.GeneratePdf();
            var fileBytes = invoice.ToArray();
            return fileBytes;
        }
    }
}
