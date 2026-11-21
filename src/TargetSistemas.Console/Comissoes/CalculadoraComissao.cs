namespace TargetSistemas;

public static class CalculadoraComissao
{
    public static decimal ObterPercentual(decimal valor)
    {
        // Abaixo de 100 não há comissão. De 100 até menos de 500, 1%. A partir de 500, 5%.
        if (valor < 100m)
            return 0m;

        if (valor < 500m)
            return 0.01m;

        return 0.05m;
    }

    public static decimal CalcularComissaoDaVenda(decimal valor)
    {
        var comissao = valor * ObterPercentual(valor);

        // Cada comissão é arredondada para centavos antes de entrar na soma do vendedor.
        return decimal.Round(comissao, 2, MidpointRounding.AwayFromZero);
    }

    public static ResultadoComissoes Processar(IReadOnlyList<Venda> vendas)
    {
        var aceitas = new List<ComissaoVenda>();
        var erros = new List<string>();

        for (var indice = 0; indice < vendas.Count; indice++)
        {
            var venda = vendas[indice];
            var numero = indice + 1;
            var vendedor = venda.Vendedor?.Trim();

            if (string.IsNullOrWhiteSpace(vendedor))
            {
                erros.Add($"Venda {numero}: vendedor não informado. O registro foi ignorado.");
                continue;
            }

            // Valor zero ou negativo não é uma venda válida e fica fora dos totais.
            if (venda.Valor <= 0)
            {
                erros.Add($"Venda {numero}: o valor precisa ser maior que zero. O registro foi ignorado.");
                continue;
            }

            aceitas.Add(new ComissaoVenda
            {
                Vendedor = vendedor,
                Valor = venda.Valor,
                Percentual = ObterPercentual(venda.Valor),
                Comissao = CalcularComissaoDaVenda(venda.Valor)
            });
        }

        var ordem = new List<string>();
        var grupos = new Dictionary<string, List<ComissaoVenda>>();

        foreach (var venda in aceitas)
        {
            if (!grupos.TryGetValue(venda.Vendedor, out var lista))
            {
                lista = new List<ComissaoVenda>();
                grupos.Add(venda.Vendedor, lista);
                ordem.Add(venda.Vendedor);
            }

            lista.Add(venda);
        }

        var resumos = ordem.Select(nome =>
        {
            var lista = grupos[nome];
            return new ResumoVendedor
            {
                Vendedor = nome,
                QuantidadeVendas = lista.Count,
                TotalVendido = lista.Sum(item => item.Valor),
                ComissaoTotal = lista.Sum(item => item.Comissao),
                Vendas = lista
            };
        }).ToList();

        return new ResultadoComissoes
        {
            Vendedores = resumos,
            Erros = erros
        };
    }
}
