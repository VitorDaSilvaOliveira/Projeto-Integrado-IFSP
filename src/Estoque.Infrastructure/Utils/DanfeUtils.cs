using QRCoder;
using System.Drawing;
using System.Xml.Linq;

namespace Estoque.Infrastructure.Utils;

public static class DanfeUtils
{
    public static XDocument MontarXmlSimples(dynamic pedido, IEnumerable<dynamic> itens, string chaveAcesso, string cNF)
    {
        var xml = new XDocument(
            new XElement("NFe",
                new XElement("infNFe", new XAttribute("Id", "NFe" + chaveAcesso),
                    new XElement("ide",
                        new XElement("cUF", "35"),
                        new XElement("nNF", ObjectUtils.SafeGetString(pedido, "NumeroPedido") ?? "0"),
                        new XElement("serie", "1"),
                        new XElement("dhEmi", ObjectUtils.SafeGetString(pedido, "DataPedido") ?? ""),
                        new XElement("tpNF", "1"),
                        new XElement("cNF", cNF)
                    ),
                    new XElement("emit",
                        new XElement("CNPJ", "12345678000195"),
                        new XElement("xNome", "Vip Penha"),
                        new XElement("IE", "ISENTO")
                    ),
                    new XElement("dest",
                        new XElement("CNPJ", ObjectUtils.SafeGetString(pedido, "ClienteCNPJ") ?? ""),
                        new XElement("xNome", ObjectUtils.SafeGetString(pedido, "ClienteNome") ?? "")
                    ),
                    itens.Select(item =>
                        new XElement("det",
                            new XElement("prod",
                                new XElement("cProd", item.Codigo),
                                new XElement("xProd", item.Descricao),
                                new XElement("qCom", item.Quantidade),
                                new XElement("vUnCom", item.ValorUnit),
                                new XElement("vProd", item.ValorTotal)
                            )
                        )
                    )
                )
            )
        );

        return xml;
    }

    public static string GerarChaveAcesso(object pedido, string cNF)
    {
        string cUF = "35";
        string AAMM = (ObjectUtils.SafeGetObject(pedido, "DataPedido") is DateTime dt) ? dt.ToString("yyMM") : DateTime.Now.ToString("yyMM");
        string cnpj = "12345678000195";
        string mod = "55";
        string serie = "001";
        string nNF = (ObjectUtils.SafeGetString(pedido, "NumeroPedido") ?? "0").PadLeft(9, '0');
        string tpEmis = "0";
        string chave43 = $"{cUF}{AAMM}{cnpj}{mod}{serie}{nNF}{tpEmis}{cNF}";
        int dv = CalculateDV(chave43);
        return chave43 + dv;
    }

    public static int CalculateDV(string chave43)
    {
        int[] pesos = { 2, 3, 4, 5, 6, 7, 8, 9 };
        int soma = 0;
        int j = 0;
        for (int i = chave43.Length - 1; i >= 0; i--)
        {
            soma += (chave43[i] - '0') * pesos[j % pesos.Length];
            j++;
        }
        int resto = soma % 11;
        return (resto == 0 || resto == 1) ? 0 : 11 - resto;
    }

    public static byte[] GerarQrPng(string texto, int pixels)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
        using var qr = new QRCode(data);
        using Bitmap bmp = qr.GetGraphic(10);
        using var ms = new MemoryStream();
        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return ms.ToArray();
    }
}
