// Sistema (Global):
// Comentar LOCALHOST quando for subir e comentar TS quando for desenvolver: -----------\
// const url = "https://localhost:7116/api";
const url = "https://ts-embalarte-sis.skydocs.com.br/api";
// -------------------------------------------------------------------------------------/

export const useToken = () => 
    useState<string>("tokenProd", () => localStorage.getItem("authToken") || "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIlQEAxMDEwRW1iNDRTVVAzIiwibmFtZSI6IkVtYmFsYXJ0ZSIsImlhdCI6MTcyMDUzNjUyNX0.Z0YYfxzFYgf5AnTE4M14uDqFmd18YLTOHoOoC1y9HkE");
  
export const useUrlProd = () => useState<string>("urlProd", () => url);

export const useUrlProdSistema = () => useState<string>("urlProdSistema", () => `${url}/sistema`);
export const useIsSideMenuOpen = () => useState<boolean>("isSideMenuOpen", () => false);
export const useIsSideMenuItemSelected = () => useState<string>("isSideMenuItemSelected", () => "selected-item-navegacao");
export const useAddRemoveGrowthStyle = () => useState<boolean>("addRemoveGrowthStyle", () => true);

// Financeiro:
export const useUrlProdFinanceiro = () => useState<string>("urlProdFinanceiro", () => `${url}/financeiro`);
export const UseShowCadastroPages = () => useState<boolean>("showCadastroPages", () => false);
export const useShowMovimentacoesPages = () => useState<boolean>('showMovimentacaoPages', () => true);
export const useShowDashboardPages = () => useState<boolean>('showDashboardPages', () => false);

// Produção:
export const useUrlProdProducao = () => useState<string>("urlProdProducao", () => `${url}/producao`);
export const useUrlTreinamentoFilho = () => useState<string>("urlTreinamentoFilho", () => `${url}/producao/controladoria/treinamento-filho`);
export const useUrlProdControladoria = () => useState<string>("urlProdControladoria", () => `${url}/controladoria`);
export const useShowRotinaPages = () => useState<boolean>('showRotinaPages', () => true);
export const UseShowCadastroProdPages = () => useState<boolean>("showCadastroProdPages", () => false);

// RH:
export const useUrlProdRH = () => useState<string>("urlProdRH", () => `${url}/recursoshumanos`);
export const useUrProdGeral = () => useState<string>("urlGeral", () => url);

// ESTOQUE:
export const useUrlEstoque = () => useState<string>("urlEstoque", () => `${url}/estoque`);
export const useIDComponente = () => useState<number | null>("IDComponente", () => 10);


