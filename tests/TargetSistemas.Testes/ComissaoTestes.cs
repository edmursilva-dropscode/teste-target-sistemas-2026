namespace TargetSistemas.Testes;

public class ComissaoTestes
{
    [Theory]
    [InlineData("99.99", 0, "0.00")]
    [InlineData("100.00", 1, "1.00")]
    [InlineData("499.99", 1, "5.00")]
    [InlineData("500.00", 5, "25.00")]
    [InlineData("1200.50", 5, "60.03")]
    [InlineData("400.50", 1, "4.01")]
    public void Aplica_faixa_e_arredonda_cada_venda(string valorTexto, int percentual, string comissaoTexto)
    {
        var valor = decimal.Parse(valorTexto, System.Globalization.CultureInfo.InvariantCulture);
        var comissao = decimal.Parse(comissaoTexto, System.Globalization.CultureInfo.InvariantCulture);

        Assert.Equal(percentual / 100m, CalculadoraComissao.ObterPercentual(valor));
        Assert.Equal(comissao, CalculadoraComissao.CalcularComissaoDaVenda(valor));
    }

    [Fact]
    public void Soma_as_comissoes_individuais_do_mesmo_vendedor()
    {
        var resultado = CalculadoraComissao.Processar(
        [
            new Venda { Vendedor = "João Silva", Valor = 100m },
            new Venda { Vendedor = "João Silva", Valor = 50m },
            new Venda { Vendedor = "João Silva", Valor = 100m },
            new Venda { Vendedor = "Maria Souza", Valor = 500m }
        ]);

        var joao = Assert.Single(resultado.Vendedores, vendedor => vendedor.Vendedor == "João Silva");
        Assert.Equal(3, joao.QuantidadeVendas);
        Assert.Equal(250m, joao.TotalVendido);
        Assert.Equal(2.00m, joao.ComissaoTotal);
        Assert.Equal(joao.Vendas.Sum(venda => venda.Comissao), joao.ComissaoTotal);

        var maria = Assert.Single(resultado.Vendedores, vendedor => vendedor.Vendedor == "Maria Souza");
        Assert.Equal(25.00m, maria.ComissaoTotal);
        Assert.Equal("João Silva", resultado.Vendedores[0].Vendedor);
    }

    [Fact]
    public void Ignora_vendedor_ausente_e_valor_invalido_sem_misturar_os_totais()
    {
        var resultado = CalculadoraComissao.Processar(
        [
            new Venda { Vendedor = "Ana Lima", Valor = 200m },
            new Venda { Vendedor = "  ", Valor = 800m },
            new Venda { Vendedor = null, Valor = 800m },
            new Venda { Vendedor = "Ana Lima", Valor = 0m },
            new Venda { Vendedor = "Ana Lima", Valor = -15m }
        ]);

        Assert.Equal(4, resultado.Erros.Count);
        Assert.Contains(resultado.Erros, erro => erro.Contains("vendedor não informado"));
        Assert.Contains(resultado.Erros, erro => erro.Contains("maior que zero"));

        var ana = Assert.Single(resultado.Vendedores);
        Assert.Equal("Ana Lima", ana.Vendedor);
        Assert.Equal(1, ana.QuantidadeVendas);
        Assert.Equal(200m, ana.TotalVendido);
        Assert.Equal(2.00m, ana.ComissaoTotal);
    }

    [Fact]
    public void Calcula_os_totais_do_json_oficial()
    {
        var vendas = LeitorDados.LerVendas(ApoioTestes.ArquivoDados("vendas.json"));
        Assert.Equal(36, vendas.Count);

        var resultado = CalculadoraComissao.Processar(vendas);
        Assert.Empty(resultado.Erros);

        AssertResumo(resultado, "João Silva", 10, 10754.70m, 495.69m);
        AssertResumo(resultado, "Maria Souza", 9, 9874.30m, 465.96m);
        AssertResumo(resultado, "Carlos Oliveira", 8, 7928.35m, 379.38m);
        AssertResumo(resultado, "Ana Lima", 9, 8763.95m, 404.99m);

        Assert.Equal(36, resultado.QuantidadeVendas);
        Assert.Equal(37321.30m, resultado.TotalVendido);
        Assert.Equal(1746.02m, resultado.ComissaoTotal);
        Assert.Equal(resultado.Vendedores.Sum(vendedor => vendedor.ComissaoTotal), resultado.ComissaoTotal);

        var carlos = resultado.Vendedores.Single(vendedor => vendedor.Vendedor == "Carlos Oliveira");
        var vendaDeQuinhentos = carlos.Vendas.Single(venda => venda.Valor == 500m);
        Assert.Equal(0.05m, vendaDeQuinhentos.Percentual);
        Assert.Equal(25.00m, vendaDeQuinhentos.Comissao);

        var joao = resultado.Vendedores.Single(vendedor => vendedor.Vendedor == "João Silva");
        Assert.Equal(60.03m, joao.Vendas.Single(venda => venda.Valor == 1200.50m).Comissao);
        Assert.Equal(4.81m, joao.Vendas.Single(venda => venda.Valor == 480.75m).Comissao);

        var maria = resultado.Vendedores.Single(vendedor => vendedor.Vendedor == "Maria Souza");
        Assert.Equal(0m, maria.Vendas.Single(venda => venda.Valor == 90.75m).Comissao);
        Assert.Equal(4.01m, maria.Vendas.Single(venda => venda.Valor == 400.50m).Comissao);
    }

    private static void AssertResumo(ResultadoComissoes resultado, string nome, int vendas, decimal total, decimal comissao)
    {
        var resumo = resultado.Vendedores.Single(vendedor => vendedor.Vendedor == nome);
        Assert.Equal(vendas, resumo.QuantidadeVendas);
        Assert.Equal(total, resumo.TotalVendido);
        Assert.Equal(comissao, resumo.ComissaoTotal);
        Assert.Equal(resumo.Vendas.Sum(venda => venda.Valor), resumo.TotalVendido);
        Assert.Equal(resumo.Vendas.Sum(venda => venda.Comissao), resumo.ComissaoTotal);
    }
}
