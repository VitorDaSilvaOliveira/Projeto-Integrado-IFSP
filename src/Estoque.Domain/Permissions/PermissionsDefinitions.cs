namespace Estoque.Domain.Permissions;

public static class PermissionDefinitions
{
    public static readonly PermissionGroup[] All =
    [
        new("Dashboards", "Painel", [
            new PermissionItem("PainelPedidos", "Painel Pedidos"),
            new PermissionItem("PainelClientes", "Painel Clientes")
        ]),

        new("Cadastros", "Cadastros", [
            new PermissionItem("Produto", "Produtos"),
            new PermissionItem("Categoria", "Categorias"),
            new PermissionItem("Fornecedor", "Fornecedores"),
            new PermissionItem("Cliente", "Clientes")
        ]),

        new("Movimentacoes", "Movimentações", [
            new PermissionItem("EntradaSaida", "Entrada e Saídas"),
            new PermissionItem("Pedido", "Pedidos"),
            new PermissionItem("Devolucao", "Devolução"),
            new PermissionItem("NotaFiscal", "Nota Fiscal")
        ]),

        new("Relatorios", "Relatórios", [
            new PermissionItem("RelatorioPedido", "Relatório Pedidos"),
            new PermissionItem("RelatorioDevolucao", "Relatório Devolução")
        ]),

        new("PainelAdmin", "Admin", [
            new PermissionItem("Formularios", "Formulários"),
            new PermissionItem("Usuarios", "Usuários"),
            new PermissionItem("Perfis", "Perfis"),
            new PermissionItem("Auditoria", "Auditoria"),
            new PermissionItem("Log", "Log")
        ])
    ];
}

public record PermissionGroup(string Id, string Name, PermissionItem[] Items);

public record PermissionItem(string Id, string Name);
