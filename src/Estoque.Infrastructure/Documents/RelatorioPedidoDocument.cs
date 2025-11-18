using Estoque.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Estoque.Infrastructure.Documents;

public class RelatorioPedidoDocument(
    Pedido pedido,
    List<PedidoItem> items,
    IWebHostEnvironment env)
    : IDocument
{
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        var logoPath = Path.Combine(env.WebRootPath, "img", "logo.png");

        container.Page(page =>
        {
            page.Size(PageSizes.A3);
            page.Margin(20);
            page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));
            page.PageColor(Colors.White);

            page.Content()
                .Padding(20)
                .Background(Colors.White)
                .Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.ConstantItem(100)
                            .Height(50)
                            .Image(logoPath);

                        row.RelativeItem().Column(c =>
                        {
                            c.Item()
                                .Text("RELATÓRIO DE PEDIDOS")
                                .FontSize(16)
                                .Bold()
                                .AlignCenter();
                        });
                    });

                    col.Item().PaddingTop(15)
                        .Background("#FFC72C")
                        .Height(30)
                        .AlignMiddle()
                        .Text("Informações do Pedido")
                        .FontColor(Colors.White)
                        .Bold()
                        .AlignCenter();

                    col.Item().PaddingTop(10)
                        .Column(c =>
                        {
                            c.Item().Row(r =>
                            {
                                r.RelativeItem().Row(row =>
                                {
                                    row.ConstantItem(120)
                                        .Container()
                                        .Background("#f5f5f5")
                                        .Border(1)
                                        .BorderColor("#bdbdbd")
                                        .Padding(5)
                                        .AlignMiddle()
                                        .Text("Número do Pedido:")
                                        .Bold();

                                    row.RelativeItem()
                                        .Container()
                                        .Background("#ffffff")
                                        .Border(1)
                                        .BorderColor("#bdbdbd")
                                        .Padding(5)
                                        .AlignMiddle()
                                        .Text($"{pedido?.NumeroPedido}")
                                        .Bold()
                                        .FontSize(9);
                                });

                                r.RelativeItem().AlignRight().Row(row =>
                                {
                                    row.ConstantItem(150)
                                        .Container()
                                        .Background("#f5f5f5")
                                        .Border(1)
                                        .BorderColor("#bdbdbd")
                                        .Padding(5)
                                        .AlignMiddle()
                                        .Text("Data do Pedido:")
                                        .Bold();

                                    row.RelativeItem()
                                        .Container()
                                        .Background("#ffffff")
                                        .Border(1)
                                        .BorderColor("#bdbdbd")
                                        .Padding(5)
                                        .AlignMiddle()
                                        .Text($"{pedido?.DataPedido}")
                                        .Bold()
                                        .FontSize(9);
                                });
                            });

                            c.Item().Row(r =>
                            {
                                r.RelativeItem().Row(row =>
                                {
                                    row.ConstantItem(120)
                                        .Container()
                                        .Background("#f5f5f5")
                                        .Border(1)
                                        .BorderColor("#bdbdbd")
                                        .Padding(5)
                                        .AlignMiddle()
                                        .Text("Status:")
                                        .Bold();

                                    row.RelativeItem()
                                        .Container()
                                        .Background("#ffffff")
                                        .Border(1)
                                        .BorderColor("#bdbdbd")
                                        .Padding(5)
                                        .AlignMiddle()
                                        .Text($"{pedido?.Status}")
                                        .Bold()
                                        .FontSize(9);
                                });

                                r.RelativeItem().AlignRight().Row(row =>
                                {
                                    row.ConstantItem(150)
                                        .Container()
                                        .Background("#f5f5f5")
                                        .Border(1)
                                        .BorderColor("#bdbdbd")
                                        .Padding(5)
                                        .AlignMiddle()
                                        .Text("Operação:")
                                        .Bold();

                                    row.RelativeItem()
                                        .Container()
                                        .Background("#ffffff")
                                        .Border(1)
                                        .BorderColor("#bdbdbd")
                                        .Padding(5)
                                        .AlignMiddle()
                                        .Text($"{pedido?.Operacao}")
                                        .Bold()
                                        .FontSize(9);
                                });
                            });
                        });

                    // Itens
                    col.Item().PaddingTop(15)
                        .Background("#FFC72C")
                        .Height(30)
                        .AlignMiddle()
                        .Text("Itens")
                        .FontColor(Colors.White)
                        .Bold()
                        .AlignCenter();

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Produto")
                                .FontSize(11).Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Preço Unitário")
                                .FontSize(11).Bold();
                        });

                        var isAlternate = false;
                        foreach (var i in items)
                        {
                            var bgColor = isAlternate ? Colors.Grey.Lighten4 : Colors.White;
                            isAlternate = !isAlternate;

                            table.Cell().Background(bgColor).Padding(5)
                                .Text(i.ProdutoId)
                                .FontSize(9);

                            table.Cell().Background(bgColor).Padding(5)
                                .Text(i.PrecoTabela)
                                .FontSize(9);
                        }
                    });
                });
        });
    }
}