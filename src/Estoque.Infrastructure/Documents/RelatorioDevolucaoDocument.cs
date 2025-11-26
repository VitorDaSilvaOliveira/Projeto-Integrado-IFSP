using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Estoque.Infrastructure.Documents;

public class RelatorioDevolucaoDocument(
    Devolucao devolucao,
    List<DevolucaoItem> items,
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

            page.Content().Padding(20).Column(col =>
            {
                // Cabeçalho
                col.Item().Row(row =>
                {
                    row.ConstantItem(100).Height(50).Image(logoPath);

                    row.RelativeItem().Column(c =>
                    {
                        c.Item()
                            .Text("RELATÓRIO DE DEVOLUÇÃO")
                            .FontSize(16)
                            .Bold()
                            .AlignCenter();
                    });
                });

                col.Item().PaddingTop(15)
                    .Background("#FFC72C")
                    .Height(30)
                    .AlignMiddle()
                    .Text("Informações da Devolução")
                    .FontColor(Colors.White)
                    .Bold()
                    .AlignCenter();

                // Informações da Devolução
                col.Item().PaddingTop(10).Column(c =>
                {
                    c.Item().Row(r =>
                    {
                        // Número
                        r.RelativeItem().Row(row =>
                        {
                            row.ConstantItem(140).Container().Background("#f5f5f5").Border(1)
                                .BorderColor("#bdbdbd").Padding(5).Text("Número da Devolução:").Bold();

                            row.RelativeItem().Container().Background("#ffffff").Border(1)
                                .BorderColor("#bdbdbd").Padding(5).Text($"{devolucao.IdDevolucao}");
                        });

                        // Data
                        r.RelativeItem().AlignRight().Row(row =>
                        {
                            row.ConstantItem(140).Container().Background("#f5f5f5").Border(1)
                                .BorderColor("#bdbdbd").Padding(5).Text("Data da Devolução:").Bold();

                            row.RelativeItem().Container().Background("#ffffff").Border(1)
                                .BorderColor("#bdbdbd").Padding(5)
                                .Text($"{devolucao.DataDevolucao:dd/MM/yyyy}");
                        });
                    });

                    // Linha 2 – Cliente + Status
                    //c.Item().Row(r =>
                    //{
                    //    r.RelativeItem().Row(row =>
                    //    {
                    //        row.ConstantItem(140).Container().Background("#f5f5f5").Border(1)
                    //            .BorderColor("#bdbdbd").Padding(5).Text("Cliente:").Bold();

                    //        row.RelativeItem().Container().Background("#ffffff").Border(1)
                    //            .BorderColor("#bdbdbd").Padding(5)
                    //            .Text($"{devolucao.Cliente?.Nome ?? "—"}");
                    //    });

                    //    r.RelativeItem().AlignRight().Row(row =>
                    //    {
                    //        row.ConstantItem(140).Container().Background("#f5f5f5").Border(1)
                    //            .BorderColor("#bdbdbd").Padding(5).Text("Status:").Bold();

                    //        row.RelativeItem().Container().Background("#ffffff").Border(1)
                    //            .BorderColor("#bdbdbd").Padding(5)
                    //            .Text($"Devolvido");
                    //    });
                    //});

                    // Linha 3 – Operação + Entrega
                    c.Item().Row(r =>
                    {
                        r.RelativeItem().Row(row =>
                        {
                            row.ConstantItem(140).Container().Background("#f5f5f5").Border(1)
                                .BorderColor("#bdbdbd").Padding(5).Text("Operação:").Bold();

                            row.RelativeItem().Container().Background("#ffffff").Border(1)
                                .BorderColor("#bdbdbd").Padding(5)
                                .Text($"Devolução");
                        });

                        r.RelativeItem().AlignRight().Row(row =>
                        {
                            row.ConstantItem(140).Container().Background("#f5f5f5").Border(1)
                                .BorderColor("#bdbdbd").Padding(5).Text("Entrega:").Bold();

                            row.RelativeItem().Container().Background("#ffffff").Border(1)
                                .BorderColor("#bdbdbd").Padding(5)
                                .Text($"{devolucao.DataDevolucao:dd/MM/yyyy}");
                        });
                    });
                    //var formaPagto = devolucao.FormaPagamento.HasValue
                    //    ? ((FormasPagamento)devolucao.FormaPagamento.Value).ToString()
                    //    : "—";

                    // Linha 4 – Pagamento
                    //c.Item().Row(r =>
                    //{
                    //    r.RelativeItem().Row(row =>
                    //    {
                    //        row.ConstantItem(140).Container().Background("#f5f5f5").Border(1)
                    //            .BorderColor("#bdbdbd").Padding(5).Text("Forma de Pagamento:").Bold();

                    //        row.RelativeItem().Container().Background("#ffffff").Border(1)
                    //            .BorderColor("#bdbdbd").Padding(5)
                    //            .Text(formaPagto);
                    //    });

                    //    r.RelativeItem().AlignRight().Row(row =>
                    //    {
                    //        row.ConstantItem(140).Container().Background("#f5f5f5").Border(1)
                    //            .BorderColor("#bdbdbd").Padding(5).Text("Parcelas:").Bold();

                    //        row.RelativeItem().Container().Background("#ffffff").Border(1)
                    //            .BorderColor("#bdbdbd").Padding(5)
                    //            .Text($"{devolucao.Parcelas ?? 1}");
                    //    });
                    //});

                    // Observações
                    if (!string.IsNullOrWhiteSpace(devolucao.Observacao))
                    {
                        c.Item().PaddingTop(10)
                            .Container().Border(1).BorderColor("#bdbdbd").Padding(5)
                            .Background("#ffffff")
                            .Text($"Observações: {devolucao.Observacao}");
                    }
                });

                // Título Itens
                col.Item().PaddingTop(15)
                    .Background("#FFC72C")
                    .Height(30)
                    .AlignMiddle()
                    .Text("Itens")
                    .FontColor(Colors.White)
                    .Bold()
                    .AlignCenter();

                // Tabela de Itens
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Produto").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Código").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Quantidade").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Preço Unitário").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Total Item").Bold();
                    });

                    var alt = false;

                    foreach (var i in items)
                    {
                        var bg = alt ? Colors.Grey.Lighten4 : Colors.White;
                        alt = !alt;

                        table.Cell().Background(bg).Padding(5).Text(i.Produto?.Nome ?? "—");
                        table.Cell().Background(bg).Padding(5).Text(i.Produto?.Codigo ?? "—");
                        table.Cell().Background(bg).Padding(5).Text(i.QuantidadeDevolvida.ToString());
                    }
                });

                // Total da Devolução
                //col.Item().PaddingTop(20)
                //    .AlignRight()
                //    .Text($"Total da Devolução: R$ {devolucao.ValorTotal:N2}")
                //    .FontSize(14)
                //    .Bold();
            });

            // Rodapé
            page.Footer()
                .AlignCenter()
                .Text($"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}");
        });
    }
}
