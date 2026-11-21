namespace TargetSistemas.Testes;

public class EstoqueTestes
{
    [Fact]
    public void Carrega_os_cinco_produtos_do_json_oficial()
    {
        var controle = Carregar();

        Assert.Equal(5, controle.Produtos.Count);
        Assert.Equal(150, controle.Obter(101)!.Saldo);
        Assert.Equal("Caneta Azul", controle.Obter(101)!.DescricaoProduto);
        Assert.Equal(75, controle.Obter(102)!.Saldo);
        Assert.Equal("Caderno Universitário", controle.Obter(102)!.DescricaoProduto);
        Assert.Equal(200, controle.Obter(103)!.Saldo);
        Assert.Equal("Borracha Branca", controle.Obter(103)!.DescricaoProduto);
        Assert.Equal(320, controle.Obter(104)!.Saldo);
        Assert.Equal("Lápis Preto HB", controle.Obter(104)!.DescricaoProduto);
        Assert.Equal(90, controle.Obter(105)!.Saldo);
        Assert.Equal("Marcador de Texto Amarelo", controle.Obter(105)!.DescricaoProduto);
        Assert.Empty(controle.Historico);
    }

    [Fact]
    public void Registra_entrada_e_saida_validas_com_identificadores_seguidos()
    {
        var controle = Carregar();

        var entrada = controle.RegistrarEntrada(105, 10, "  reposição  ");
        var saida = controle.RegistrarSaida(105, 4, "uso interno");
        var ajuste = controle.RegistrarSaida(105, 6, "ajuste");

        Assert.True(entrada.Sucesso);
        Assert.True(saida.Sucesso);
        Assert.True(ajuste.Sucesso);
        Assert.Equal(90, controle.Obter(105)!.Saldo);

        Assert.Equal([1, 2, 3], controle.Historico.Select(item => item.Identificador).ToArray());
        Assert.Equal(TipoMovimentacao.Entrada, entrada.Movimentacao!.Tipo);
        Assert.Equal("reposição", entrada.Movimentacao.Descricao);
        Assert.Equal(90, entrada.Movimentacao.EstoqueAnterior);
        Assert.Equal(100, entrada.Movimentacao.EstoqueFinal);
        Assert.Equal(100, saida.Movimentacao!.EstoqueAnterior);
        Assert.Equal(96, saida.Movimentacao.EstoqueFinal);
        Assert.Equal(90, ajuste.Movimentacao!.EstoqueFinal);
        Assert.Equal(105, entrada.Movimentacao.CodigoProduto);
    }

    [Fact]
    public void Usa_o_tipo_como_descricao_quando_o_texto_vem_vazio()
    {
        var controle = Carregar();

        var movimentacao = controle.RegistrarSaida(101, 1, "   ");

        Assert.Equal("Saída", movimentacao.Movimentacao!.Descricao);
        Assert.Equal(TipoMovimentacao.Saida, movimentacao.Movimentacao.Tipo);
    }

    [Fact]
    public void Recusa_saida_acima_do_saldo_produto_inexistente_e_quantidade_invalida()
    {
        var controle = Carregar();
        var saldoInicial = controle.Obter(101)!.Saldo;

        var acima = controle.RegistrarSaida(101, saldoInicial + 1, "excesso");
        var inexistente = controle.RegistrarEntrada(999, 1, "produto novo");
        var zero = controle.RegistrarEntrada(101, 0, "zero");
        var negativa = controle.RegistrarSaida(101, -3, "negativa");

        Assert.False(acima.Sucesso);
        Assert.Contains("ultrapassa", acima.Mensagem);
        Assert.False(inexistente.Sucesso);
        Assert.Contains("não encontrado", inexistente.Mensagem);
        Assert.False(zero.Sucesso);
        Assert.False(negativa.Sucesso);
        Assert.Contains("maior que zero", zero.Mensagem);

        Assert.Equal(saldoInicial, controle.Obter(101)!.Saldo);
        Assert.Empty(controle.Historico);

        var valida = controle.RegistrarEntrada(101, 2, "depois das falhas");
        Assert.Equal(1, valida.Movimentacao!.Identificador);
        Assert.Equal(saldoInicial + 2, controle.Obter(101)!.Saldo);
        Assert.Single(controle.Historico);
    }

    [Fact]
    public void Nova_carga_do_json_volta_ao_estoque_original()
    {
        var caminho = ApoioTestes.ArquivoDados("estoque.json");
        var primeiro = ControleEstoque.Carregar(caminho);
        primeiro.RegistrarSaida(101, 10, "temporaria");
        Assert.Equal(140, primeiro.Obter(101)!.Saldo);

        var segundo = ControleEstoque.Carregar(caminho);
        Assert.Equal(150, segundo.Obter(101)!.Saldo);
        Assert.Empty(segundo.Historico);
    }

    private static ControleEstoque Carregar() =>
        ControleEstoque.Carregar(ApoioTestes.ArquivoDados("estoque.json"));
}
