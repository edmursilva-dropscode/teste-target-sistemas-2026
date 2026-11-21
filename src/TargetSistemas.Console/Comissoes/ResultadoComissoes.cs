namespace TargetSistemas;

public sealed class ComissaoVenda
{
    public required string Vendedor { get; init; }

    public required decimal Valor { get; init; }

    public required decimal Percentual { get; init; }

    public required decimal Comissao { get; init; }
}

public sealed class ResumoVendedor
{
    public required string Vendedor { get; init; }

    public required int QuantidadeVendas { get; init; }

    public required decimal TotalVendido { get; init; }

    public required decimal ComissaoTotal { get; init; }

    public required IReadOnlyList<ComissaoVenda> Vendas { get; init; }
}

public sealed class ResultadoComissoes
{
    public required IReadOnlyList<ResumoVendedor> Vendedores { get; init; }

    public required IReadOnlyList<string> Erros { get; init; }

    public int QuantidadeVendas => Vendedores.Sum(vendedor => vendedor.QuantidadeVendas);

    public decimal TotalVendido => Vendedores.Sum(vendedor => vendedor.TotalVendido);

    public decimal ComissaoTotal => Vendedores.Sum(vendedor => vendedor.ComissaoTotal);
}
