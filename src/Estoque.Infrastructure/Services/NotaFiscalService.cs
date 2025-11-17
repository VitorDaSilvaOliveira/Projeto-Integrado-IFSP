using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Utils;
using JJMasterData.Core.UI.Components;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZXing;
using ZXing.Common;
using System.Drawing;
using System.Drawing.Imaging;

namespace Estoque.Infrastructure.Services;

public class NotaFiscalService(EstoqueDbContext context, IComponentFactory componentFactory)
{
    public async Task<JJFormView> GetFormViewNotaFiscalAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("vw_PedidoNF");
        formView.ShowTitle = true;
        return formView;
    }

    public void GenerateDanfe(int pedidoId, Stream outputPdf)
    {
        var linhas = context.Vw_PedidoNF.Where(x => x.PedidoId == pedidoId).ToList();
        var pedido = linhas.First();

        var clienteNome = ObjectUtils.SafeGetString(pedido, "ClienteNome");
        var clienteCnpj = ObjectUtils.SafeGetString(pedido, "ClienteCNPJ");
        var clienteTelefone = ObjectUtils.SafeGetString(pedido, "ClienteTelefone");
        var clienteEmail = ObjectUtils.SafeGetString(pedido, "ClienteEmail");

        var itens = linhas.Select(i => new
        {
            Codigo = ObjectUtils.SafeGetInt(i, "id_Produto"),
            Descricao = ObjectUtils.SafeGetString(i, "ProdutoNome"),
            Quantidade = ObjectUtils.SafeGetDecimal(i, "Quantidade"),
            ValorUnit = ObjectUtils.SafeGetDecimal(i, "PrecoTabela"),
            ValorTotal = ObjectUtils.SafeGetDecimal(i, "ItemTotal")
        }).ToList();

        var somaItens = itens.Sum(x => x.ValorTotal);
        var valorNf = ObjectUtils.SafeGetDecimal(pedido, "ValorNF");
        if (valorNf == 0) valorNf = somaItens;

        var cNf = new Random().Next(10000000, 99999999).ToString("D8");
        var chaveAcesso = DanfeUtils.GerarChaveAcesso(pedido, cNf);

        var writer = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Height = 100,     
                Width = 600, 
                Margin = 0,
                PureBarcode = true
            }
        };
        var pixelData = writer.Write(chaveAcesso);
        byte[] barcodePng;
        using (var bmp = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb))
        {
            var data = bmp.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, data.Scan0, pixelData.Pixels.Length);
            bmp.UnlockBits(data);
            using var ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            barcodePng = ms.ToArray();
        }

        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(10);
                page.DefaultTextStyle(t => t.FontSize(8));

                // === LAYOUT PRINCIPAL ===
                page.Content().Column(content =>
                {
                    // CABEÇALHO (Topo da página)
                    content.Item().Height(120).Border(1).Padding(6).Row(row =>
                    {
                        // Emitente
                        row.RelativeItem(3).Column(c =>
                        {
                            c.Item().Text("VIP PENHA ELETRÔNICOS LTDA").FontSize(16).Bold();
                            c.Item().Text("CNPJ: 12.345.678/0001-95 | IE: ISENTO");
                            c.Item().Text("R. Dr. João Ribeiro, 424 - Penha - SP - CEP 03607-000");
                            c.Item().Text("Telefone: (11) 2222-3333 | E-mail: contato@vippenha.com");
                        });

                        // Título DANFE
                        row.RelativeItem(2).BorderLeft(1).PaddingLeft(8).Column(c =>
                        {
                            c.Item().AlignCenter().Text("DANFE").FontSize(18).Bold();
                            c.Item().AlignCenter().Text("Documento Auxiliar da Nota Fiscal Eletrônica");
                            c.Item().AlignCenter().Text("Modelo 55").FontSize(9);
                            c.Item().AlignCenter().Text($"Nº: {ObjectUtils.SafeGetString(pedido, "NumeroPedido") ?? "(---)"}").Bold();
                            c.Item().AlignCenter().Text("Série: 1");
                        });

                        // Código de barras
                        row.ConstantItem(200).BorderLeft(1).PaddingLeft(5).Column(c =>
                        {
                            c.Item().Image(barcodePng, ImageScaling.FitWidth);
                            c.Item().AlignCenter().Text("CHAVE DE ACESSO").FontSize(7);
                            c.Item().AlignCenter().Text(chaveAcesso).FontSize(7);
                        });
                    });

                    // NATUREZA / PROTOCOLO
                    content.Item().Height(40).Border(1).Padding(3).Row(r =>
                    {
                        r.RelativeItem().Text($"NATUREZA DA OPERAÇÃO: VENDA DE MERCADORIA").Bold();
                        r.ConstantItem(250).AlignRight().Text("PROTOCOLO: 000000000000000 - 01/11/2025 10:20:00");
                    });

                    // DESTINATÁRIO
                    content.Item().Height(80).Border(1).Padding(4).Column(c =>
                    {
                        c.Item().Text("DESTINATÁRIO / REMETENTE").Bold();
                        c.Item().Text($"Nome / Razão Social: {clienteNome}");
                        c.Item().Text($"CPF/CNPJ: {clienteCnpj} | Fone: {clienteTelefone}");
                        c.Item().Text($"Email: {clienteEmail}");
                    });

                    // IMPOSTOS + TRANSPORTE
                    content.Item().Height(100).Row(r =>
                    {
                        r.RelativeItem(5).Border(1).Padding(4).Column(col =>
                        {
                            col.Item().Text("CÁLCULO DO IMPOSTO").Bold();
                            col.Item().Text($"Base de Cálculo do ICMS: 0,00");
                            col.Item().Text($"Valor do ICMS: 0,00");
                            col.Item().Text($"Valor Total dos Produtos: {somaItens:F2}");
                            col.Item().Text($"Valor Total da Nota: {valorNf:F2}");
                        });

                        r.RelativeItem(5).Border(1).Padding(4).Column(col =>
                        {
                            col.Item().Text("TRANSPORTADOR / VOLUMES TRANSPORTADOS").Bold();
                            col.Item().Text("Frete: 0 - Por conta do Remetente");
                        });
                    });

                    // PRODUTOS
                    content.Item().MinHeight(350).Padding(2).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(1);
                            cols.RelativeColumn(5);
                            cols.RelativeColumn(1);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                        });

                        // Cabeçalho
                        table.Header(header =>
                        {
                            header.Cell().Border(1).AlignCenter().Text("CÓD").Bold();
                            header.Cell().Border(1).AlignCenter().Text("DESCRIÇÃO DO PRODUTO / SERVIÇO").Bold();
                            header.Cell().Border(1).AlignCenter().Text("QTDE").Bold();
                            header.Cell().Border(1).AlignCenter().Text("V. UNIT.").Bold();
                            header.Cell().Border(1).AlignCenter().Text("V. TOTAL").Bold();
                        });

                        void LinhaProduto(string c1, string c2, string c3, string c4, string c5, bool temProduto)
                        {
                            var top = temProduto ? 1 : 0;

                            table.Cell().BorderLeft(1).BorderTop(top).AlignCenter().Text(c1);
                            table.Cell().BorderLeft(1).BorderTop(top).Text(c2);
                            table.Cell().BorderLeft(1).BorderTop(top).AlignRight().Text(c3);
                            table.Cell().BorderLeft(1).BorderTop(top).AlignRight().Text(c4);
                            table.Cell().BorderLeft(1).BorderRight(1).BorderTop(top).AlignRight().Text(c5);
                        }

                        // Linhas reais
                        foreach (var item in itens)
                        {
                            LinhaProduto(
                                item.Codigo.ToString(),
                                item.Descricao,
                                item.Quantidade.ToString("N2"),
                                item.ValorUnit.ToString("F2"),
                                item.ValorTotal.ToString("F2"),
                                true
                            );
                        }

                        // Linhas vazias (só verticais)
                        int totalLinhas = 35;
                        int linhasFaltando = totalLinhas - itens.Count;

                        for (int i = 0; i < linhasFaltando; i++)
                        {
                            LinhaProduto(" ", " ", " ", " ", " ", false);
                        }
                    });

                    // DADOS ADICIONAIS
                    content.Item().Height(100).Border(1).Padding(4).Column(c =>
                    {
                        c.Item().Text("DADOS ADICIONAIS").Bold();
                        c.Item().Text(ObjectUtils.SafeGetString(pedido, "Observacoes") ?? "Nenhuma observação registrada.");
                    });
                });

                // Rodapé
                page.Footer().Height(30).Padding(2).BorderTop(1).Column(f =>
                {
                    f.Item().AlignCenter().Text("Documento emitido em ambiente de homologação - Sem valor fiscal").FontSize(8);
                    f.Item().AlignCenter().Text("Consulta de autenticidade: https://homologacao.nfe.fazenda.sp.gov.br/").FontSize(8);
                });
            });
        });

        document.GeneratePdf(outputPdf);
    }
}