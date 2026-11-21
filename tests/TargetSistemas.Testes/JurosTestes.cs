namespace TargetSistemas.Testes;

public class JurosTestes
{
    private static readonly DateOnly Hoje = new(2026, 11, 21);

    [Fact]
    public void Vencimento_hoje_nao_gera_juros()
    {
        var resultado = CalculadoraJuros.Calcular(1000m, Hoje, Hoje);

        Assert.True(resultado.Sucesso);
        Assert.Equal(0, resultado.DiasAtraso);
        Assert.Equal(0m, resultado.Juros);
        Assert.Equal(1000m, resultado.ValorFinal);
    }

    [Fact]
    public void Vencimento_futuro_nao_gera_juros()
    {
        var resultado = CalculadoraJuros.Calcular(800m, Hoje.AddDays(5), Hoje);

        Assert.True(resultado.Sucesso);
        Assert.Equal(0, resultado.DiasAtraso);
        Assert.Equal(0m, resultado.Juros);
        Assert.Equal(800m, resultado.ValorFinal);
    }

    [Theory]
    [InlineData(1, "1000", "25.00", "1025.00")]
    [InlineData(3, "200", "15.00", "215.00")]
    [InlineData(10, "1000", "250.00", "1250.00")]
    public void Calcula_juros_simples_pelos_dias_de_atraso(int dias, string valorTexto, string jurosTexto, string finalTexto)
    {
        var valor = decimal.Parse(valorTexto, System.Globalization.CultureInfo.InvariantCulture);
        var juros = decimal.Parse(jurosTexto, System.Globalization.CultureInfo.InvariantCulture);
        var final = decimal.Parse(finalTexto, System.Globalization.CultureInfo.InvariantCulture);

        var resultado = CalculadoraJuros.Calcular(valor, Hoje.AddDays(-dias), Hoje);

        Assert.True(resultado.Sucesso);
        Assert.Equal(dias, resultado.DiasAtraso);
        Assert.Equal(juros, resultado.Juros);
        Assert.Equal(final, resultado.ValorFinal);
        Assert.Equal(valor, resultado.ValorOriginal);
        Assert.Equal(Hoje, resultado.DataReferencia);
    }

    [Fact]
    public void Arredonda_os_juros_para_dois_centavos_antes_de_somar()
    {
        var resultado = CalculadoraJuros.Calcular(0.20m, Hoje.AddDays(-1), Hoje);

        Assert.Equal(1, resultado.DiasAtraso);
        Assert.Equal(0.01m, resultado.Juros);
        Assert.Equal(0.21m, resultado.ValorFinal);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-10")]
    [InlineData("-0,01")]
    public void Recusa_valor_zero_ou_negativo(string valor)
    {
        var resultado = CalculadoraJuros.Interpretar(valor, "21/11/2026", Hoje);

        Assert.False(resultado.Sucesso);
        Assert.Contains("maior que zero", resultado.Mensagem);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("10reais")]
    public void Recusa_valor_vazio_ou_nao_numerico(string valor)
    {
        var resultado = CalculadoraJuros.Interpretar(valor, "21/11/2026", Hoje);

        Assert.False(resultado.Sucesso);
        Assert.False(string.IsNullOrWhiteSpace(resultado.Mensagem));
    }

    [Theory]
    [InlineData("")]
    [InlineData("31/02/2026")]
    [InlineData("2026-11-21")]
    [InlineData("abc")]
    [InlineData("32/01/2026")]
    public void Recusa_data_invalida(string data)
    {
        var resultado = CalculadoraJuros.Interpretar("150,00", data, Hoje);

        Assert.False(resultado.Sucesso);
        Assert.Contains("dd/MM/yyyy", resultado.Mensagem);
    }

    [Fact]
    public void Aceita_valor_em_formato_brasileiro()
    {
        var resultado = CalculadoraJuros.Interpretar("1.000,50", "18/11/2026", Hoje);

        Assert.True(resultado.Sucesso);
        Assert.Equal(1000.50m, resultado.ValorOriginal);
        Assert.Equal(3, resultado.DiasAtraso);
        Assert.Equal(75.04m, resultado.Juros);
        Assert.Equal(1075.54m, resultado.ValorFinal);
    }
}
