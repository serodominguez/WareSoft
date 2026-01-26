using Application.Dtos.Response.GoodsIssue;
using Application.Dtos.Response.GoodsReceipt;
using Application.Dtos.Response.Inventory;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using Utilities.Extensions;

namespace Application.Services
{
    public class GeneratePdfService : IGeneratePdfService
    {
        private readonly IConfiguration _configuration;
        private readonly CultureInfo _bolivianCulture;

        public GeneratePdfService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Configurar cultura boliviana con coma como separador de miles
            _bolivianCulture = new CultureInfo("es-BO");
            _bolivianCulture.NumberFormat.NumberGroupSeparator = ",";
            _bolivianCulture.NumberFormat.NumberDecimalDigits = 0;
        }

        private string FormatCurrency(decimal value)
        {
            return value.ToString("N0", _bolivianCulture);
        }

        public byte[] GoodsIssueGeneratePdf(GoodsIssueWithDetailsResponseDto issue)
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
                var titleStyle = TextStyle.Default.FontSize(12).Bold().FontColor(Colors.Black);

                container.Column(column =>
                {
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("Sucursal: ").Style(titleStyle);
                        text.Span(issue.StoreName).Style(titleStyle);
                    });

                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("Documento: ").Style(titleStyle);
                        text.Span(issue.Code).Style(titleStyle);
                    });

                    column.Item().PaddingTop(15);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(leftColumn =>
                        {
                            leftColumn.Item().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span("Tipo: ").SemiBold();
                                text.Span($"{issue.Type}");
                            });

                            leftColumn.Spacing(5);

                            leftColumn.Item().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span("Fecha de Registro: ").SemiBold();
                                text.Span($"{issue.AuditCreateDate:d}");
                            });

                        });

                        row.RelativeItem().Column(rightColumn =>
                        {
                            rightColumn.Item().AlignRight().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span("Personal: ").SemiBold();
                                text.Span($"{issue.UserName}");
                            });

                            rightColumn.Spacing(5);

                            rightColumn.Item().AlignRight().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span("Registrado por: ").SemiBold();
                                text.Span($"{issue.AuditCreateName}");
                            });

                        });
                    });
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
                        text.DefaultTextStyle(x => x.FontSize(10));
                        text.Span("Total Bs.: ").SemiBold();
                        text.Span($"{FormatCurrency(issue.TotalAmount)}").Bold();
                    });

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
                        colums.RelativeColumn(4);
                        colums.RelativeColumn(3);
                        colums.RelativeColumn(3);
                        colums.RelativeColumn((float)1.5);
                        colums.RelativeColumn((float)1.5);
                        colums.RelativeColumn((float)1.5);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Nº").FontSize(10);
                        header.Cell().Element(CellStyle).Text("Código").FontSize(10);
                        header.Cell().Element(CellStyle).Text("Descripción").FontSize(10);
                        header.Cell().Element(CellStyle).Text("Material").FontSize(10);
                        header.Cell().Element(CellStyle).Text("Color").FontSize(10);
                        header.Cell().Element(CellStyle).AlignRight().Text("Cantidad").FontSize(10);
                        header.Cell().Element(CellStyle).AlignRight().Text("Precio U.").FontSize(10);
                        header.Cell().Element(CellStyle).AlignRight().Text("Subtotal").FontSize(10);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .DefaultTextStyle(x => x.SemiBold())
                                .PaddingVertical(5)
                                .BorderBottom(1)
                                .BorderColor(Colors.Black);
                        }
                    });

                    foreach (var item in issue.GoodsIssueDetails)
                    {
                        table.Cell().Element(CellStyle).Text(item.Item.ToString()).FontSize(9);
                        table.Cell().Element(CellStyle).Text(item.Code ?? string.Empty).FontSize(9);
                        table.Cell().Element(CellStyle).Text(item.Description ?? string.Empty).FontSize(9);
                        table.Cell().Element(CellStyle).Text(item.Material ?? string.Empty).FontSize(9);
                        table.Cell().Element(CellStyle).Text(item.Color ?? string.Empty).FontSize(9);
                        table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString()).FontSize(9);
                        table.Cell().Element(CellStyle).AlignRight().Text(FormatCurrency(item.UnitPrice)).FontSize(9);
                        table.Cell().Element(CellStyle).AlignRight().Text(FormatCurrency(item.TotalPrice)).FontSize(9);

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
                container.Column(column =>
                {
                    column.Item()
                        .Background(Colors.Grey.Lighten3)
                        .Padding(10)
                        .Column(innerColumn =>
                        {
                            innerColumn.Spacing(5);
                            innerColumn.Item().Text("Observaciones:").FontSize(10);
                            innerColumn.Item().Text(issue.Annotations ?? "").FontSize(9);
                        });

                    column.Item().PaddingTop(30);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(leftColumn =>
                        {
                            leftColumn.Item().Text("Entregado por:").FontSize(10).SemiBold();
                            leftColumn.Item().PaddingTop(40);
                            leftColumn.Item().BorderTop(1).BorderColor(Colors.Black).Text("").FontSize(9);
                            leftColumn.Item().AlignCenter().Text("Nombre y Firma").FontSize(8);
                        });

                        row.ConstantItem(50);

                        row.RelativeItem().Column(rightColumn =>
                        {
                            rightColumn.Item().Text("Recibido por:").FontSize(10).SemiBold();
                            rightColumn.Item().PaddingTop(40);
                            rightColumn.Item().BorderTop(1).BorderColor(Colors.Black).Text("").FontSize(9);
                            rightColumn.Item().AlignCenter().Text("Nombre y Firma").FontSize(8);
                        });
                    });
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
                var titleStyle = TextStyle.Default.FontSize(12).Bold().FontColor(Colors.Black);

                container.Column(column =>
                {
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("Sucursal: ").Style(titleStyle);
                        text.Span(receipt.StoreName).Style(titleStyle);
                    });

                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("Documento: ").Style(titleStyle);
                        text.Span(receipt.Code).Style(titleStyle);
                    });

                    column.Item().PaddingTop(15);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(leftColumn =>
                        {
                            leftColumn.Item().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span("Tipo: ").SemiBold();
                                text.Span($"{receipt.Type}");
                            });

                            leftColumn.Spacing(5);

                            leftColumn.Item().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span("Fecha de Registro: ").SemiBold();
                                text.Span($"{receipt.AuditCreateDate:d}");
                            });

                            leftColumn.Spacing(5);

                            leftColumn.Item().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span("Tipo de Documento: ").SemiBold();
                                text.Span($"{receipt.DocumentType}");
                            });

                            leftColumn.Spacing(5);

                            leftColumn.Item().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span("Fecha del Documento: ").SemiBold();
                                text.Span($"{receipt.DocumentDate:d}");
                            });
                        });

                        row.RelativeItem().Column(rightColumn =>
                        {
                            rightColumn.Item().AlignRight().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span("Proveedor: ").SemiBold();
                                text.Span($"{receipt.CompanyName}");
                            });

                            rightColumn.Spacing(5);
                            rightColumn.Item().AlignRight().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span("Registrado por: ").SemiBold();
                                text.Span($"{receipt.AuditCreateName}");
                            });

                            rightColumn.Spacing(5);

                            rightColumn.Item().AlignRight().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span("Número de Documento: ").SemiBold();
                                text.Span($"{receipt.DocumentNumber}");
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
                        text.DefaultTextStyle(x => x.FontSize(10));
                        text.Span("Total Bs.: ").SemiBold();
                        text.Span($"{FormatCurrency(receipt.TotalAmount)}").Bold();
                    });

                    //if (!string.IsNullOrWhiteSpace(receipt.Annotations))
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
                        colums.RelativeColumn(4);
                        colums.RelativeColumn(3);
                        colums.RelativeColumn(3);
                        colums.RelativeColumn((float)1.5);
                        colums.RelativeColumn((float)1.5);
                        colums.RelativeColumn((float)1.5);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Nº").FontSize(10);
                        header.Cell().Element(CellStyle).Text("Código").FontSize(10);
                        header.Cell().Element(CellStyle).Text("Descripción").FontSize(10);
                        header.Cell().Element(CellStyle).Text("Material").FontSize(10);
                        header.Cell().Element(CellStyle).Text("Color").FontSize(10);
                        header.Cell().Element(CellStyle).AlignRight().Text("Cantidad").FontSize(10);
                        header.Cell().Element(CellStyle).AlignRight().Text("Costo U.").FontSize(10);
                        header.Cell().Element(CellStyle).AlignRight().Text("Subtotal").FontSize(10);

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
                        table.Cell().Element(CellStyle).Text(item.Item.ToString()).FontSize(9);
                        table.Cell().Element(CellStyle).Text(item.Code ?? string.Empty).FontSize(9);
                        table.Cell().Element(CellStyle).Text(item.Description ?? string.Empty).FontSize(9);
                        table.Cell().Element(CellStyle).Text(item.Material ?? string.Empty).FontSize(9);
                        table.Cell().Element(CellStyle).Text(item.Color ?? string.Empty).FontSize(9);
                        table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString()).FontSize(9);
                        table.Cell().Element(CellStyle).AlignRight().Text(FormatCurrency(item.UnitCost)).FontSize(9);
                        table.Cell().Element(CellStyle).AlignRight().Text(FormatCurrency(item.TotalCost)).FontSize(9);

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
                container.Column(column =>
                {
                    // Observaciones
                    column.Item()
                        .Background(Colors.Grey.Lighten3)
                        .Padding(10)
                        .Column(innerColumn =>
                        {
                            innerColumn.Spacing(5);
                            innerColumn.Item().Text("Observaciones:").FontSize(10);
                            innerColumn.Item().Text(receipt.Annotations ?? "").FontSize(9);
                        });

                    column.Item().PaddingTop(30);

                    // Firmas
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(leftColumn =>
                        {
                            leftColumn.Item().Text("Entregado por:").FontSize(10).SemiBold();
                            leftColumn.Item().PaddingTop(40);
                            leftColumn.Item().BorderTop(1).BorderColor(Colors.Black).Text("").FontSize(9);
                            leftColumn.Item().AlignCenter().Text("Nombre y Firma").FontSize(8);
                        });

                        row.ConstantItem(50);

                        row.RelativeItem().Column(rightColumn =>
                        {
                            rightColumn.Item().Text("Recibido por:").FontSize(10).SemiBold();
                            rightColumn.Item().PaddingTop(40);
                            rightColumn.Item().BorderTop(1).BorderColor(Colors.Black).Text("").FontSize(9);
                            rightColumn.Item().AlignCenter().Text("Nombre y Firma").FontSize(8);
                        });
                    });
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

        public byte[] InventoryGeneratePdf(List<InventoryResponseDto> inventory, string storeName)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
            });

            void ComposeHeader(IContainer container)
            {
                var titleStyle = TextStyle.Default.FontSize(14).Bold().FontColor(Colors.Black);
                var subtitleStyle = TextStyle.Default.FontSize(11).SemiBold().FontColor(Colors.Black);

                container.Column(column =>
                {
                    column.Item().AlignCenter().Text("Centro Optico" +' '+storeName.ToTitleCase()).Style(titleStyle);
                    column.Item().AlignCenter().Text("Planilla de inventario").Style(subtitleStyle);
                    column.Item().AlignCenter().Text($"Fecha: {DateTime.Now:dd/MM/yyyy, h:mm:ss tt}").FontSize(9);
                    column.Item().PaddingTop(10);
                });
            }

            void ComposeContent(IContainer container)
            {
                container.PaddingVertical(10).Column(column =>
                {
                    column.Item().Element(ComposeTable);
                });
            }

            void ComposeTable(IContainer container)
            {
                container.Table(table =>
                {
                    // Definir columnas con anchos relativos
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);  // Categoría
                        columns.RelativeColumn(1.5f);  // Marca
                        columns.RelativeColumn(2);  // Descripción
                        columns.RelativeColumn(1.5f);  // Código
                        columns.RelativeColumn(1.5f);  // Color
                        columns.RelativeColumn(1.5f);  // Material
                        columns.RelativeColumn(1);  // Precio
                        columns.RelativeColumn(1);  // Cantidad
                    });

                    // Encabezado
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Categoría").FontSize(9);
                        header.Cell().Element(CellStyle).Text("Marca").FontSize(9);
                        header.Cell().Element(CellStyle).Text("Descripción").FontSize(9);
                        header.Cell().Element(CellStyle).Text("Código").FontSize(9);
                        header.Cell().Element(CellStyle).Text("Color").FontSize(9);
                        header.Cell().Element(CellStyle).Text("Material").FontSize(9);
                        header.Cell().Element(CellStyle).AlignRight().Text("Precio").FontSize(9);
                        header.Cell().Element(CellStyle).AlignRight().Text("Cantidad").FontSize(9);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .DefaultTextStyle(x => x.SemiBold())
                                .PaddingVertical(5)
                                .PaddingHorizontal(3)
                                .BorderBottom(1)
                                .BorderColor(Colors.Black);
                        }
                    });

                    // Filas de datos
                    foreach (var item in inventory)
                    {
                        table.Cell().Element(CellStyle).Text(item.CategoryName ?? "").FontSize(8);
                        table.Cell().Element(CellStyle).Text(item.BrandName ?? "").FontSize(8);
                        table.Cell().Element(CellStyle).Text(item.Description ?? "").FontSize(8);
                        table.Cell().Element(CellStyle).Text(item.Code ?? "").FontSize(8);
                        table.Cell().Element(CellStyle).Text(item.Color ?? "").FontSize(8);
                        table.Cell().Element(CellStyle).Text(item.Material ?? "").FontSize(8);
                        table.Cell().Element(CellStyle).AlignRight().Text(FormatCurrency(item.Price)).FontSize(8);
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.Stock}........").FontSize(8);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .PaddingVertical(3)
                                .PaddingHorizontal(3);
                        }
                    }
                });
            }

            void ComposeFooter(IContainer container)
            {
                container.AlignCenter()
                    .Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(9));
                        text.Span("Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
            }

            var pdfDocument = document.GeneratePdf();
            var fileBytes = pdfDocument.ToArray();
            return fileBytes;
        }
    }
}