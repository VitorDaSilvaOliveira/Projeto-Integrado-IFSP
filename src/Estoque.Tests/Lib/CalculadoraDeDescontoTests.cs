namespace Estoque.Lib
{
    public class CalculadoraDeDesconto
    {
        public decimal CalcularDesconto(decimal valor, decimal percentual)
        {
            if (valor < 0 || percentual < 0)
                throw new ArgumentException("Valor e percentual não podem ser negativos.");

            return valor - (valor * percentual / 100);
        }
    }
}
