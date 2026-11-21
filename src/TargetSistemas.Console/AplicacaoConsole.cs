namespace TargetSistemas;

public sealed class AplicacaoConsole
{
    private readonly TextReader _entrada;
    private readonly TextWriter _saida;
    private readonly string _diretorioDados;
    private readonly DateOnly _dataReferencia;
    private ControleEstoque? _estoque;

    public AplicacaoConsole(TextReader entrada, TextWriter saida, string diretorioDados, DateOnly dataReferencia)
    {
        _entrada = entrada;
        _saida = saida;
        _diretorioDados = diretorioDados;
        _dataReferencia = dataReferencia;
    }

    public void Executar()
    {
        while (true)
        {
            ExibirMenuPrincipal();
            var opcao = LerOpcao();
            if (opcao is null || opcao == "0")
            {
                _saida.WriteLine("Programa encerrado.");
                return;
            }

            switch (opcao)
            {
                case "1":
                    CalcularComissoes();
                    break;
                case "2":
                    MovimentarEstoque();
                    break;
                case "3":
                    CalcularJuros();
                    break;
                default:
                    _saida.WriteLine("Opção inválida. Escolha 1, 2, 3 ou 0.");
                    _saida.WriteLine();
                    break;
            }
        }
    }

    private void CalcularComissoes()
    {
        _saida.WriteLine();
        _saida.WriteLine("Comissões de vendas");
        _saida.WriteLine();

        ResultadoComissoes resultado;
        try
        {
            var caminho = Path.Combine(_diretorioDados, "vendas.json");
            var vendas = LeitorDados.LerVendas(caminho);
            resultado = CalculadoraComissao.Processar(vendas);
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or System.Text.Json.JsonException)
        {
            _saida.WriteLine("Não foi possível ler as vendas: " + ex.Message);
            _saida.WriteLine();
            return;
        }

        if (resultado.Erros.Count > 0)
        {
            _saida.WriteLine("Alguns registros foram ignorados:");
            foreach (var erro in resultado.Erros)
                _saida.WriteLine("- " + erro);

            _saida.WriteLine();
        }

        if (resultado.Vendedores.Count == 0)
        {
            _saida.WriteLine("Nenhuma venda válida para calcular.");
            _saida.WriteLine();
            return;
        }

        foreach (var vendedor in resultado.Vendedores)
        {
            _saida.WriteLine(vendedor.Vendedor);
            _saida.WriteLine("Vendas: " + vendedor.QuantidadeVendas);
            _saida.WriteLine("Total vendido: " + Formatacao.Moeda(vendedor.TotalVendido));
            _saida.WriteLine("Comissão total: " + Formatacao.Moeda(vendedor.ComissaoTotal));

            foreach (var venda in vendedor.Vendas)
            {
                _saida.WriteLine(
                    "  Valor " + Formatacao.Moeda(venda.Valor)
                    + " | " + Formatacao.Percentual(venda.Percentual)
                    + " | Comissão " + Formatacao.Moeda(venda.Comissao));
            }

            _saida.WriteLine();
        }

        _saida.WriteLine("TOTAL");
        _saida.WriteLine("Vendas: " + resultado.QuantidadeVendas);
        _saida.WriteLine("Total vendido: " + Formatacao.Moeda(resultado.TotalVendido));
        _saida.WriteLine("Comissão total: " + Formatacao.Moeda(resultado.ComissaoTotal));
        _saida.WriteLine();
    }

    private void MovimentarEstoque()
    {
        var controle = ObterEstoque();
        if (controle is null)
            return;

        while (true)
        {
            _saida.WriteLine();
            _saida.WriteLine("Movimentação de estoque");
            _saida.WriteLine();
            _saida.WriteLine("1 - Consultar estoque");
            _saida.WriteLine("2 - Registrar entrada");
            _saida.WriteLine("3 - Registrar saída");
            _saida.WriteLine("4 - Ver histórico");
            _saida.WriteLine("0 - Voltar");
            _saida.WriteLine();

            var opcao = LerOpcao();
            if (opcao is null || opcao == "0")
                return;

            switch (opcao)
            {
                case "1":
                    ConsultarEstoque(controle);
                    break;
                case "2":
                    RegistrarMovimento(controle, TipoMovimentacao.Entrada);
                    break;
                case "3":
                    RegistrarMovimento(controle, TipoMovimentacao.Saida);
                    break;
                case "4":
                    ExibirHistorico(controle);
                    break;
                default:
                    _saida.WriteLine("Opção inválida. Escolha 1, 2, 3, 4 ou 0.");
                    _saida.WriteLine();
                    break;
            }
        }
    }

    private ControleEstoque? ObterEstoque()
    {
        if (_estoque is not null)
            return _estoque;

        try
        {
            var caminho = Path.Combine(_diretorioDados, "estoque.json");
            // Uma carga por execução. Sair do submenu não apaga saldo nem histórico.
            _estoque = ControleEstoque.Carregar(caminho);
            return _estoque;
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or System.Text.Json.JsonException)
        {
            _saida.WriteLine();
            _saida.WriteLine("Não foi possível ler o estoque: " + ex.Message);
            _saida.WriteLine();
            return null;
        }
    }

    private void ConsultarEstoque(ControleEstoque controle)
    {
        _saida.WriteLine();
        _saida.WriteLine($"{"Código",-8}{"Descrição",-32}{"Estoque",8}");
        foreach (var produto in controle.Produtos)
        {
            _saida.WriteLine($"{produto.CodigoProduto,-8}{produto.DescricaoProduto,-32}{produto.Saldo,8}");
        }

        _saida.WriteLine();
    }

    private void RegistrarMovimento(ControleEstoque controle, TipoMovimentacao tipo)
    {
        _saida.WriteLine();
        var codigoTexto = LerLinha("Código do produto: ");
        if (codigoTexto is null)
            return;

        if (!int.TryParse(codigoTexto.Trim(), out var codigo))
        {
            _saida.WriteLine("Informe um código de produto válido.");
            _saida.WriteLine();
            return;
        }

        var quantidadeTexto = LerLinha("Quantidade: ");
        if (quantidadeTexto is null)
            return;

        if (!int.TryParse(quantidadeTexto.Trim(), out var quantidade))
        {
            _saida.WriteLine("A quantidade deve ser um número inteiro maior que zero.");
            _saida.WriteLine();
            return;
        }

        var descricao = LerLinha("Descrição da movimentação: ");
        if (descricao is null)
            return;

        var resultado = tipo == TipoMovimentacao.Entrada
            ? controle.RegistrarEntrada(codigo, quantidade, descricao)
            : controle.RegistrarSaida(codigo, quantidade, descricao);

        if (!resultado.Sucesso || resultado.Movimentacao is null)
        {
            _saida.WriteLine(resultado.Mensagem);
            _saida.WriteLine();
            return;
        }

        var movimento = resultado.Movimentacao;
        _saida.WriteLine($"Movimentação {movimento.Identificador} registrada.");
        _saida.WriteLine("Produto: " + movimento.CodigoProduto + " - " + movimento.DescricaoProduto);
        _saida.WriteLine("Tipo: " + DescreverTipo(movimento.Tipo));
        _saida.WriteLine("Descrição: " + movimento.Descricao);
        _saida.WriteLine("Quantidade: " + movimento.Quantidade);
        _saida.WriteLine("Estoque anterior: " + movimento.EstoqueAnterior);
        _saida.WriteLine("Estoque final: " + movimento.EstoqueFinal);
        _saida.WriteLine();
    }

    private void ExibirHistorico(ControleEstoque controle)
    {
        _saida.WriteLine();
        if (controle.Historico.Count == 0)
        {
            _saida.WriteLine("Nenhuma movimentação registrada nesta execução.");
            _saida.WriteLine();
            return;
        }

        foreach (var movimento in controle.Historico)
        {
            _saida.WriteLine(
                movimento.Identificador
                + " | " + DescreverTipo(movimento.Tipo)
                + " | " + movimento.CodigoProduto + " - " + movimento.DescricaoProduto
                + " | quantidade " + movimento.Quantidade
                + " | estoque " + movimento.EstoqueAnterior + " -> " + movimento.EstoqueFinal
                + " | " + movimento.Descricao);
        }

        _saida.WriteLine();
    }

    private void CalcularJuros()
    {
        _saida.WriteLine();
        var textoValor = LerLinha("Valor original: ");
        if (textoValor is null)
            return;

        var textoData = LerLinha("Data de vencimento (dd/MM/yyyy): ");
        if (textoData is null)
            return;

        var resultado = CalculadoraJuros.Interpretar(textoValor, textoData, _dataReferencia);
        if (!resultado.Sucesso)
        {
            _saida.WriteLine(resultado.Mensagem);
            _saida.WriteLine();
            return;
        }

        _saida.WriteLine("Valor original: " + Formatacao.Moeda(resultado.ValorOriginal));
        _saida.WriteLine("Data de vencimento: " + Formatacao.Data(resultado.Vencimento));
        _saida.WriteLine("Data atual: " + Formatacao.Data(resultado.DataReferencia));
        _saida.WriteLine("Dias de atraso: " + resultado.DiasAtraso);
        _saida.WriteLine("Juros: " + Formatacao.Moeda(resultado.Juros));
        _saida.WriteLine("Valor final: " + Formatacao.Moeda(resultado.ValorFinal));
        if (resultado.DiasAtraso == 0)
            _saida.WriteLine(resultado.Mensagem);

        _saida.WriteLine();
    }

    private void ExibirMenuPrincipal()
    {
        _saida.WriteLine("Desafio Target Sistemas");
        _saida.WriteLine();
        _saida.WriteLine("1 - Calcular comissões de vendas");
        _saida.WriteLine("2 - Movimentar estoque");
        _saida.WriteLine("3 - Calcular juros por atraso");
        _saida.WriteLine("0 - Sair");
        _saida.WriteLine();
    }

    private string? LerOpcao()
    {
        var texto = LerLinha("Opção: ");
        return texto?.Trim();
    }

    private string? LerLinha(string prompt)
    {
        _saida.Write(prompt);
        return _entrada.ReadLine();
    }

    private static string DescreverTipo(TipoMovimentacao tipo) =>
        tipo == TipoMovimentacao.Entrada ? "Entrada" : "Saída";
}
