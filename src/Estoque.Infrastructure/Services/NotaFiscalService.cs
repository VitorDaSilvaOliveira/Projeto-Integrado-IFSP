using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Utils;
using JJMasterData.Core.UI.Components;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Xml.Linq;

namespace Estoque.Infrastructure.Services;

public class NotaFiscalService(EstoqueDbContext context, IComponentFactory componentFactory)
{
    public async Task<JJFormView> GetFormViewNotaFiscalAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("vw_PedidoNF");
        formView.ShowTitle = true;

        return formView;
    }

    public void GenerateDanfe(int pedidoId, string outputPdfPath)
    {
        var linhas = context.Vw_PedidoNF
            .Where(x => x.PedidoId == pedidoId)
            .ToList();

        var pedido = linhas.First();

        string clienteTelefone = ObjectUtils.SafeGetString(pedido, "ClienteTelefone") ?? ObjectUtils.SafeGetString(pedido, "TelefoneCliente") ?? "(não informado)";
        string clienteEmail = ObjectUtils.SafeGetString(pedido, "ClienteEmail") ?? ObjectUtils.SafeGetString(pedido, "EmailCliente") ?? "(não informado)";

        var itens = linhas.Select(i => new
        {
            Codigo = ObjectUtils.SafeGetInt(i, "id_Produto"),
            Descricao = ObjectUtils.SafeGetString(i, "ProdutoNome") ?? "",
            Quantidade = ObjectUtils.SafeGetDecimal(i, "Quantidade"),
            ValorUnit = ObjectUtils.SafeGetDecimal(i, "PrecoTabela"),
            ValorTotal = ObjectUtils.SafeGetDecimal(i, "ItemTotal")
        }).ToList();

        decimal somaItens = itens.Sum(x => x.ValorTotal);
        decimal valorNF = ObjectUtils.SafeGetDecimal(pedido, "ValorNF");
        if (valorNF == 0) valorNF = somaItens;

        string cNF = new Random().Next(10000000, 99999999).ToString("D8");
        string chaveAcesso = DanfeUtils.GerarChaveAcesso(pedido, cNF);

        XDocument xml = DanfeUtils.MontarXmlSimples(pedido, itens, chaveAcesso, cNF);

        QuestPDF.Settings.License = LicenseType.Community;
        string qrText = $"https://homologacao.nfe.fazenda.sp.gov.br/qrcode?p={chaveAcesso}&tpAmb=2";
        byte[] qrPng = DanfeUtils.GerarQrPng(qrText, 200);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(18);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("VIP PENHA").FontSize(18).Bold();
                        col.Item().Text("CNPJ: 12.345.678/0001-95").FontSize(9);
                        col.Item().Text("IE: ISENTO").FontSize(9);
                        col.Item().Text("End.: R. Dr. João Ribeiro, 424 - Penha de França, São Paulo - SP, 03634-000").FontSize(9);
                        col.Item().Text("Tel.: (11) 98527-7600").FontSize(9);
                    });

                    row.ConstantItem(160).Column(col =>
                    {
                        col.Item().Image(qrPng, ImageScaling.FitArea);
                        col.Item().Text("QR Code - Ambiente: HOMOLOGAÇÃO").FontSize(8).AlignCenter();
                    });
                });

                page.Content().Column(col =>
                {
                    col.Spacing(6);

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Column(left =>
                        {
                            left.Item().Text("DESTINATÁRIO").Bold().FontSize(10);
                            left.Item().Text($"{ObjectUtils.SafeGetString(pedido, "ClienteNome") ?? "(Nome não informado)"}").FontSize(9);
                            left.Item().Text($"CNPJ/CPF: {ObjectUtils.SafeGetString(pedido, "ClienteCNPJ") ?? "(não informado)"}").FontSize(9);
                            left.Item().Text($"Tel: {clienteTelefone}  •  Email: {clienteEmail}").FontSize(9);
                        });

                        r.ConstantItem(260).Column(right =>
                        {
                            right.Item().Text("DADOS DA NF-e").Bold().FontSize(10);
                            right.Item().Text($"Nº: {ObjectUtils.SafeGetString(pedido, "NumeroPedido") ?? "(---)"}").FontSize(9);
                            var dataPedidoObj = ObjectUtils.SafeGetObject(pedido, "DataPedido");
                            string dataStr = dataPedidoObj is DateTime dt ? dt.ToString("yyyy-MM-dd HH:mm") : "(não informado)";
                            right.Item().Text($"Emissão: {dataStr}").FontSize(9);
                            right.Item().Text($"Chave de Acesso: {chaveAcesso}").FontSize(8);
                        });
                    });

                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(6);
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Descrição").FontSize(9).Bold();
                            header.Cell().AlignRight().Text("Qtde").FontSize(9).Bold();
                            header.Cell().AlignRight().Text("V.Unit").FontSize(9).Bold();
                            header.Cell().AlignRight().Text("V.Total").FontSize(9).Bold();
                        });

                        bool odd = false;
                        foreach (var item in itens)
                        {
                            odd = !odd;
                            table.Cell().Element(c => c.Padding(6).Background(odd ? Colors.Grey.Lighten5 : Colors.White)).Text(item.Descricao).FontSize(9);
                            table.Cell().Element(c => c.Padding(6).Background(odd ? Colors.Grey.Lighten5 : Colors.White)).AlignRight().Text(item.Quantidade.ToString("N0")).FontSize(9);
                            table.Cell().Element(c => c.Padding(6).Background(odd ? Colors.Grey.Lighten5 : Colors.White)).AlignRight().Text(item.ValorUnit.ToString("F2")).FontSize(9);
                            table.Cell().Element(c => c.Padding(6).Background(odd ? Colors.Grey.Lighten5 : Colors.White)).AlignRight().Text(item.ValorTotal.ToString("F2")).FontSize(9);
                        }

                        table.Footer(footer =>
                        {
                            footer.Cell().ColumnSpan(2).Element(c => c.Padding(6)).Text("");
                            footer.Cell().Element(c => c.Padding(6)).AlignRight().Text("Subtotal:").FontSize(9);
                            footer.Cell().Element(c => c.Padding(6)).AlignRight().Text(somaItens.ToString("F2")).FontSize(9);

                            footer.Cell().ColumnSpan(2).Element(c => c.Padding(6)).Text("");
                            footer.Cell().Element(c => c.Padding(6)).AlignRight().Text("Total (R$):").FontSize(11).Bold();
                            footer.Cell().Element(c => c.Padding(6)).AlignRight().Text(valorNF.ToString("F2")).FontSize(11).Bold();
                        });
                    });

                    col.Item().PaddingTop(8).Text("Observações:").Bold().FontSize(9);
                    col.Item().Text(ObjectUtils.SafeGetString(pedido, "Observacoes") ?? "").FontSize(9);

                });

                page.Footer().Column(f =>
                {
                    f.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    f.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Documento emitido em ambiente de homologação. DANFE gerado para fins de teste.").FontSize(8);
                        r.ConstantItem(200).AlignRight().Text($"Emitente: VIP PENHA • CNPJ: 12.345.678/0001-95").FontSize(8);
                    });
                });
            });
        });

        document.GeneratePdf(outputPdfPath);
    }
}