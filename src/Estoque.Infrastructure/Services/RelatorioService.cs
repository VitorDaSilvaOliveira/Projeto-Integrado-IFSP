using JJMasterData.Core.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;
using Estoque.Infrastructure.Data;
using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Estoque.Infrastructure.Services
{
    public class RelatorioService
    {
        private readonly EstoqueDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public RelatorioService(EstoqueDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public PdfDocument GeraPDFSLAConsumo(List<PedidoItemPDF> pedidosItens, PedidoPDF pedido)
        {
            var headers = new[] { "PRODUTO", "ADICIONAIS", "PREÇO", "CATEGORIA", "FIM GARANTIA", "QUANTIDADE" };
            var keys = new[] { "nomeCodigoProduto", "descricaoProduto", "precoVendaPedidoItem", "categoriaProduto", "fimGarantiaProdutoItem", "quantidadeProdutoItem" };
            var widths = new double[] { 40, 20, 10, 10, 10, 10 };

            var pdf = GeraRelatorioPDF("PEDIDO", headers, keys, widths, pedidosItens, pedido);
            return pdf;
        }

        public static PdfDocument GeraRelatorioPDF<T>(string titulo, string[] headers, string[] keys, double[] widths, List<T> itens, PedidoPDF pedido)
        {
            // CONFIGURAÇÃO ---------------------------------------
            var (document, page, gfx, margin, pageWidth, currentY) = NovoPDF_A4Horizontal();
            double lineHeight = 18;

            var paginas = new List<(PdfPage page, XGraphics gfx)>();
            paginas.Add((page, gfx));

            // Cabeçalho geral
            currentY = PDF_Cabecalho(page, gfx, margin, pageWidth, currentY);

            // Título
            currentY = PDF_Titulo(gfx, margin, pageWidth, currentY, titulo, XBrushes.Gray, 2);
            currentY += 0;

            // Informações do pedido
            currentY = PDFInfosPedido(page, gfx, margin, pageWidth, currentY, pedido);
            currentY += 1;

            // Cabeçalho da tabela
            var colWidthsPt = widths.Select(p => pageWidth * (p / 100.0)).ToArray();
            currentY = EscreveCabecalhoTabelaPDF(gfx, margin, pageWidth, currentY, headers, widths);
            currentY += 3;

            XFont font = new XFont("Arial", 8);
            XBrush brush = XBrushes.Black;

            // Impressão das linhas
            for (int i = 0; i < itens.Count; i++)
            {
                if (currentY + lineHeight > page.Height.Point - margin)
                {
                    page = document.AddPage();
                    page.Width = XUnit.FromMillimeter(297);
                    page.Height = XUnit.FromMillimeter(210);
                    gfx = XGraphics.FromPdfPage(page);
                    paginas.Add((page, gfx));

                    currentY = PDF_Cabecalho(page, gfx, margin, pageWidth, margin);
                    currentY = EscreveCabecalhoTabelaPDF(gfx, margin, pageWidth, currentY, headers, widths);
                    currentY += 3;
                }

                // Escreve linha da tabela (mesmo método que você já tem)
                EscreveLinhaTabela(gfx, margin, currentY, colWidthsPt, keys, itens, i, font, brush);

                currentY += lineHeight;
            }

            // Rodapé em todas as páginas
            int totalPaginas = paginas.Count;
            for (int i = 0; i < paginas.Count; i++)
            {
                var (pg, gfxPg) = paginas[i];
                PDF_Rodape(gfxPg, pg, pageWidth, i + 1, totalPaginas, margin);
            }

            return document;
        }

        public static (PdfDocument document, PdfPage page, XGraphics gfx, double margin, double pageWidth, double currentY) NovoPDF_A4Horizontal()
        {
            // Cria um novo documento PDF
            var document = new PdfDocument();

            // Adiciona uma página e define as dimensões dela (A4 horizontal)
            var page = document.AddPage();
            page.Width = XUnit.FromMillimeter(297);  // A4 horizontal
            page.Height = XUnit.FromMillimeter(210); // A4 horizontal

            // Cria um objeto gráfico para desenhar no PDF
            var gfx = XGraphics.FromPdfPage(page);

            // Configurações auxiliares
            double margin = 10;
            double pageWidth = page.Width - (margin * 2);
            double currentY = margin; // controla o ponto Y atual da página

            return (document, page, gfx, margin, pageWidth, currentY);
        }

        public static double EscreveCabecalhoTabelaPDF(XGraphics gfx, double margin, double pageWidth, double currentY, string[] headers, double[] widthsPercent)
        {
            if (headers.Length != widthsPercent.Length)
                throw new ArgumentException("O número de colunas e de larguras precisa ser igual.");

            // Fonte
            var font = new XFont("Arial", 8, XFontStyle.Bold);
            double alturaLinha = 20;

            // Posição inicial (X)
            double currentX = margin;

            for (int i = 0; i < headers.Length; i++)
            {
                // Largura da coluna em pontos (proporção da largura disponível da página)
                double colWidth = pageWidth * (widthsPercent[i] / 100.0);

                // Desenha o fundo cinza claro
                gfx.DrawRectangle(XBrushes.LightGray, currentX, currentY, colWidth, alturaLinha);

                // Desenha a borda (opcional, para tabela ficar delimitada)
                gfx.DrawRectangle(XPens.Black, currentX, currentY, colWidth, alturaLinha);

                // Escreve o texto centralizado na célula
                var format = new XStringFormat
                {
                    Alignment = XStringAlignment.Center,
                    LineAlignment = XLineAlignment.Center
                };
                gfx.DrawString(headers[i], font, XBrushes.Black,
                    new XRect(currentX, currentY, colWidth, alturaLinha), format);

                // Avança para próxima coluna
                currentX += colWidth;
            }

            // Retorna a nova posição Y (abaixo do cabeçalho)
            return currentY + alturaLinha;
        }

        public static double PDF_Cabecalho(PdfPage page, XGraphics gfx, double margin, double pageWidth, double currentY)
        {
            // LOGO
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "logo.png");
            var image = XImage.FromFile(imagePath);

            double xPos = margin;
            double yPos = currentY;
            double imageWidth = image.PixelWidth * 0.1;
            double imageHeight = image.PixelHeight * 0.1;

            gfx.DrawImage(image, xPos, yPos, imageWidth, imageHeight);

            // LINHA
            xPos += imageWidth;
            yPos += imageHeight;
            double width = pageWidth - imageWidth - 220;
            double height = 1;

            XRect div = new XRect(xPos, yPos, width, height);
            //gfx.DrawRectangle(XBrushes.Gray, div);

            // DATA DE EMISSÃO
            xPos += width;
            yPos = margin + 8;

            width = 220;
            height = 10;
            div = new XRect(xPos, yPos, width, height);

            gfx.DrawString($"DOCUMENTO EMITIDO EM : {DateTime.Now:dd/MM/yyyy HH:mm}", FontsPDF.Fonte8Verdana, XBrushes.Black, div, XStringFormats.CenterRight);

            // ➡ Atualiza o "currentY" para logo abaixo do cabeçalho
            return currentY + imageHeight + 5;
        }

        public static double PDF_Titulo(XGraphics gfx, double margin, double pageWidth, double currentY, string titulo, XBrush xBrushes, int desenharLinhas = 1)
        {
            var altura = 40; // altura do retângulo do título
            var padding = 5; // espaço interno

            // Desenha o fundo branco
            var rect = new XRect(margin, currentY, pageWidth, altura);
            gfx.DrawRectangle(xBrushes, rect);

            // Desenha linhas apenas se desenharLinhas for 1
            if (desenharLinhas == 1)
            {
                double linhaY = currentY + altura / 2;

                // Calcula largura do texto
                XSize tamanhoTexto = gfx.MeasureString(titulo, FontsPDF.Fonte20VerdanaBold);

                // Ponto X inicial do texto centralizado
                double textoX = margin + (pageWidth / 2) - (tamanhoTexto.Width / 2);
                double textoXFinal = textoX + tamanhoTexto.Width;

                // Desenha linha esquerda (da margem até antes do texto)
                gfx.DrawLine(XPens.Black, margin, linhaY, textoX - 5, linhaY); // 5px de espaço antes do texto

                // Desenha linha direita (após o texto até a margem direita)
                gfx.DrawLine(XPens.Black, textoXFinal + 5, linhaY, margin + pageWidth, linhaY); // 5px de espaço após o texto
            }

            // Desenha o texto centralizado verticalmente
            gfx.DrawString(titulo, FontsPDF.Fonte20VerdanaBold, XBrushes.Black, rect, XStringFormats.Center);

            // Atualiza o Y atual para a próxima seção do PDF
            currentY += altura + padding;

            return currentY;
        }

        public static double PDFInfosPedido(PdfPage page, XGraphics gfx, double margin, double pageWidth, double currentY, PedidoPDF pedido)
        {
            var fontLabel = FontsPDF.Fonte8VerdanaBold;
            var fontValue = FontsPDF.Fonte8Verdana;
            var penLine = new XPen(XColors.LightGray, 0.5);
            var headerBrush = new XSolidBrush(XColors.LightGray);

            double linhaHeight = 14;
            double espacamento = 6;
            double colunaEspacamento = 20;

            // 3 colunas
            double colunaLargura = (pageWidth - margin * 2 - (2 * colunaEspacamento)) / 3;
            double xCol1 = margin;
            double xCol2 = xCol1 + colunaLargura + colunaEspacamento;
            double xCol3 = xCol2 + colunaLargura + colunaEspacamento;
            double yPos = currentY;

            // Cabeçalho da seção
            gfx.DrawRectangle(headerBrush, new XRect(margin, yPos, pageWidth, 20));
            gfx.DrawString("INFORMAÇÕES DO PEDIDO", FontsPDF.Fonte10VerdanaBold, XBrushes.Black,
                new XRect(margin + 5, yPos + 3, pageWidth - margin * 2, 20), XStringFormats.TopLeft);
            yPos += 25;

            // Lista de informações (sem observações ainda)
            var infos = new (string label, string value)[]
            {
        ("NÚMERO DO PEDIDO:", pedido.numeroPedido),
        ("DATA DO PEDIDO:", pedido.dataPedido),
        ("CLIENTE:", pedido.clienteNome),
        ("VALOR TOTAL:", pedido.valorTotal),
        ("STATUS:", pedido.status),
        ("OPERAÇÃO:", pedido.operacao),
        ("USUÁRIO:", pedido.usuario),
        ("FORMA DE PAGAMENTO:", pedido.formaPagamento),
        ("PARCELAS:", pedido.parcelas)
            };

            // Exibir 3 informações por linha
            for (int i = 0; i < infos.Length; i += 3)
            {
                var col1 = infos[i];
                var col2 = i + 1 < infos.Length ? infos[i + 1] : (label: "", value: "");
                var col3 = i + 2 < infos.Length ? infos[i + 2] : (label: "", value: "");

                // Coluna 1
                gfx.DrawString(col1.label, fontLabel, XBrushes.Black,
                    new XRect(xCol1, yPos, colunaLargura / 2, linhaHeight), XStringFormats.TopLeft);
                gfx.DrawString(col1.value ?? "-", fontValue, XBrushes.Black,
                    new XRect(xCol1 + colunaLargura / 2, yPos, colunaLargura / 2, linhaHeight), XStringFormats.TopLeft);

                // Coluna 2
                gfx.DrawString(col2.label, fontLabel, XBrushes.Black,
                    new XRect(xCol2, yPos, colunaLargura / 2, linhaHeight), XStringFormats.TopLeft);
                gfx.DrawString(col2.value ?? "-", fontValue, XBrushes.Black,
                    new XRect(xCol2 + colunaLargura / 2, yPos, colunaLargura / 2, linhaHeight), XStringFormats.TopLeft);

                // Coluna 3
                gfx.DrawString(col3.label, fontLabel, XBrushes.Black,
                    new XRect(xCol3, yPos, colunaLargura / 2, linhaHeight), XStringFormats.TopLeft);
                gfx.DrawString(col3.value ?? "-", fontValue, XBrushes.Black,
                    new XRect(xCol3 + colunaLargura / 2, yPos, colunaLargura / 2, linhaHeight), XStringFormats.TopLeft);

                // Linha divisória inferior
                gfx.DrawLine(penLine, margin, yPos + linhaHeight + 2, pageWidth - margin, yPos + linhaHeight + 2);

                yPos += linhaHeight + espacamento;
            }

            // Campo de observações - mesma linha que os outros
            if (!string.IsNullOrWhiteSpace(pedido.observacoes))
            {
                yPos += 5;

                gfx.DrawString("OBSERVAÇÕES:", fontLabel, XBrushes.Black,
                    new XRect(margin, yPos, pageWidth / 6, linhaHeight), XStringFormats.TopLeft);

                gfx.DrawString(pedido.observacoes, fontValue, XBrushes.Black,
                    new XRect(margin + pageWidth / 6, yPos, pageWidth - margin * 2 - (pageWidth / 6), linhaHeight),
                    XStringFormats.TopLeft);

                // Linha inferior
                yPos += linhaHeight + espacamento;
                gfx.DrawLine(penLine, margin, yPos, pageWidth - margin, yPos);
            }


            return yPos + 10;
        }

        public static void EscreveLinhaTabela<T>(XGraphics gfx, double startX, double startY, double[] colWidths, string[] keys, List<T> lista, int rowIndex, XFont font, XBrush brush)
        {
            if (rowIndex < 0 || rowIndex >= lista.Count)
                return; // índice fora da lista

            var item = lista[rowIndex];
            double x = startX;

            // altura da linha
            double alturaLinha = font.Height + 5;

            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i];

                // pega a propriedade pelo nome
                var prop = typeof(T).GetProperty(key);
                string value = prop != null ? prop.GetValue(item)?.ToString() ?? "" : "";

                // largura da coluna
                double width = colWidths[i];

                // margem interna (2,5% em cada lado)
                double margemInterna = width * 0.025;
                double larguraTextoMax = width - 2 * margemInterna;

                // cortar o texto se for maior que a largura da coluna menos margem
                string textoFinal = value;
                var size = gfx.MeasureString(textoFinal, font);
                if (size.Width > larguraTextoMax)
                {
                    while (textoFinal.Length > 0 && gfx.MeasureString(textoFinal + "...", font).Width > larguraTextoMax)
                    {
                        textoFinal = textoFinal.Substring(0, textoFinal.Length - 1);
                    }
                    textoFinal += "...";
                }

                // formatação: centralizado horizontal e vertical
                var format = new XStringFormat
                {
                    Alignment = XStringAlignment.Center,
                    LineAlignment = XLineAlignment.Center
                };

                // desenha o texto com margem interna
                gfx.DrawString(
                    textoFinal,
                    font,
                    brush,
                    new XRect(x + margemInterna, startY, width - 2 * margemInterna, alturaLinha),
                    format
                );

                // avança para a próxima coluna
                x += width;
            }

            // desenha linha preta abaixo da linha
            gfx.DrawLine(XPens.Black, startX, startY + alturaLinha, startX + colWidths.Sum(), startY + alturaLinha);
        }

        public static void PDF_Rodape(XGraphics gfx, PdfPage page, double pageWidth, int numeroPagina, int totalPaginas, double marginBottom = 15)
        {
            // Posição vertical do retângulo do rodapé
            double alturaRodape = 12;
            double yPos = page.Height - marginBottom - alturaRodape;

            // Desenha fundo cinza
            var rect = new XRect(15, yPos, pageWidth, alturaRodape);
            gfx.DrawRectangle(XBrushes.LightGray, rect);

            // Texto centralizado à direita com padding
            string texto = $"PÁGINA {numeroPagina} DE {totalPaginas}";
            var rectComPadding = new XRect(rect.X, rect.Y, rect.Width - 5, rect.Height); // subtrai 5 para padding right
            gfx.DrawString(texto, FontsPDF.Fonte8Verdana, XBrushes.Black, rectComPadding, XStringFormats.CenterRight);
        }

        // GETS DE INFORMAÇOES
        public async Task<PedidoPDF> GetInformacoesPedidoAsync(string idPedido)
        {
            if (!int.TryParse(idPedido, out int pedidoId))
                throw new ArgumentException("ID do pedido inválido.");

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == pedidoId);

            if (pedido == null)
                throw new Exception("Pedido não encontrado.");

            // Aqui buscamos o usuário responsável pelo Id armazenado no pedido
            var usuario = await _userManager.FindByIdAsync(pedido.UsuarioResponsavel);

            var pedidoPDF = new PedidoPDF
            {
                numeroPedido = pedido.NumeroPedido ?? "-",
                dataPedido = pedido.DataPedido?.ToString("dd/MM/yyyy - HH:mm") ?? "-",
                clienteNome = pedido.Cliente?.Nome ?? "-",
                valorTotal = pedido.ValorTotal?.ToString("C2") ?? "-",
                status = pedido.Status.HasValue
                ? System.Text.RegularExpressions.Regex.Replace(pedido.Status.Value.ToString(), "(\\B[A-Z])", " $1")
                : "-",
                operacao = pedido.Operacao?.ToString() ?? "-",
                observacoes = pedido.Observacoes ?? "-",
                usuario = usuario?.NormalizedUserName ?? "-", // <- aqui pegamos o NormalizedUserName
                formaPagamento = Enum.IsDefined(typeof(FormasPagamento), pedido.FormaPagamento ?? 0)
                    ? ((FormasPagamento)pedido.FormaPagamento).ToString()
                    : "-",
                parcelas = pedido.Parcelas?.ToString() ?? "-"
            };

            return pedidoPDF;
        }
            public async Task<List<PedidoItemPDF>> GetInformacoesPedidoItensAsync(string idPedido)
            {
                if (!int.TryParse(idPedido, out int id))
                    throw new ArgumentException("O parâmetro idPedido deve ser um número válido.");

                var itens = await _context.PedidosItens
                    .Where(x => x.id_Pedido == id)
                    .Include(x => x.Produto)
                        .ThenInclude(p => p.Categoria)
                    .Include(x => x.Pedido) // para pegar a data do pedido pai
                    .Select(x => new PedidoItemPDF
                    {
                        nomeCodigoProduto =
                          (x.Produto.Codigo ?? "-") + " - " + (x.Produto.Nome ?? "-"),
                        descricaoProduto = x.Produto.Nome ?? x.Produto.Descricao ?? "-",
                        precoVendaPedidoItem = x.Produto.Preco ?? 0,
                        categoriaProduto = x.Produto.Categoria.NomeCategoria ?? "-",

                        fimGarantiaProdutoItem =
                            x.Pedido.DataPedido.HasValue && x.Produto.DiasGarantia.HasValue
                                ? x.Pedido.DataPedido.Value
                                    .AddDays(x.Produto.DiasGarantia.Value)
                                    .ToString("dd/MM/yyyy")
                                : "-",

                        quantidadeProdutoItem = x.Quantidade
                    })
                    .ToListAsync();

                return itens;
            }

        }

    public static class FontsPDF
    {
        // Fontes normais
        public static readonly XFont Fonte8Roboto = new XFont("Roboto", 8, XFontStyle.Regular);

        public static readonly XFont Fonte8Calibri = new XFont("Calibri", 8, XFontStyle.Regular);

        public static readonly XFont Fonte8Tahoma = new XFont("Tahoma", 8, XFontStyle.Regular);

        public static readonly XFont Fonte8SegoeUI = new XFont("Segoe UI", 8, XFontStyle.Regular);

        public static readonly XFont Fonte8Helvetica = new XFont("Helvetica", 8, XFontStyle.Regular);

        public static readonly XFont Fonte8Garamond = new XFont("Garamond", 8, XFontStyle.Regular);

        public static readonly XFont Fonte8Georgia = new XFont("Georgia", 8, XFontStyle.Regular);

        public static readonly XFont Fonte8PalatinoLinotype = new XFont("Palatino Linotype", 8, XFontStyle.Regular);

        public static readonly XFont Fonte8BookAntiqua = new XFont("Book Antiqua", 8, XFontStyle.Regular);

        public static readonly XFont Fonte7Arial = new XFont("Arial", 7, XFontStyle.Regular);
        public static readonly XFont Fonte6Arial = new XFont("Arial", 6, XFontStyle.Regular);
        public static readonly XFont Fonte8Arial = new XFont("Arial", 8, XFontStyle.Regular);


        public static readonly XFont Fonte8Verdana = new XFont("Verdana", 8, XFontStyle.Regular);
        public static readonly XFont Fonte10Verdana = new XFont("Verdana", 10, XFontStyle.Regular);
        public static readonly XFont Fonte12Verdana = new XFont("Verdana", 12, XFontStyle.Regular);
        public static readonly XFont Fonte14Verdana = new XFont("Verdana", 14, XFontStyle.Regular);
        public static readonly XFont Fonte16Verdana = new XFont("Verdana", 16, XFontStyle.Regular);

        // Fontes em negrito
        public static readonly XFont Fonte6VerdanaBold = new XFont("Verdana", 6, XFontStyle.Bold);
        public static readonly XFont Fonte7VerdanaBold = new XFont("Verdana", 7, XFontStyle.Bold);
        public static readonly XFont Fonte8VerdanaBold = new XFont("Verdana", 8, XFontStyle.Bold);
        public static readonly XFont Fonte10VerdanaBold = new XFont("Verdana", 10, XFontStyle.Bold);
        public static readonly XFont Fonte12VerdanaBold = new XFont("Verdana", 12, XFontStyle.Bold);
        public static readonly XFont Fonte14VerdanaBold = new XFont("Verdana", 14, XFontStyle.Bold);
        public static readonly XFont Fonte16VerdanaBold = new XFont("Verdana", 16, XFontStyle.Bold);
        public static readonly XFont Fonte20VerdanaBold = new XFont("Verdana", 20, XFontStyle.Bold);
        public static readonly XFont Fonte25VerdanaBold = new XFont("Verdana", 25, XFontStyle.Bold);
    }
    public class PedidoItemPDF
    {
        public string nomeCodigoProduto { get; set; }
        public string descricaoProduto { get; set; }
        public decimal precoVendaPedidoItem { get; set; }
        public string categoriaProduto { get; set; }
        public string fimGarantiaProdutoItem { get; set; }
        public int quantidadeProdutoItem { get; set; }
    }
    public class PedidoPDF
    {
        public string numeroPedido { get; set; }
        public string dataPedido { get; set; }
        public string clienteNome { get; set; }
        public string valorTotal { get; set; }
        public string status { get; set; }
        public string operacao { get; set; }
        public string observacoes { get; set; }
        public string usuario { get; set; }
        public string formaPagamento { get; set; }
        public string parcelas { get; set; }
    }
    public class SLAConsumoRequest
    {
        public List<PedidoItemPDF> PedidosItens { get; set; }
        public PedidoPDF Pedido { get; set; }
    }

}
