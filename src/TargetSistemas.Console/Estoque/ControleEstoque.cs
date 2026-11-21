namespace TargetSistemas;

public sealed class ControleEstoque
{
    private readonly List<Produto> _produtos;
    private readonly List<MovimentacaoEstoque> _historico = new();
    private int _proximoIdentificador = 1;

    private ControleEstoque(List<Produto> produtos)
    {
        _produtos = produtos;
    }

    public static ControleEstoque Carregar(string caminho)
    {
        var produtos = LeitorDados.LerProdutos(caminho)
            .Select(produto => new Produto
            {
                CodigoProduto = produto.CodigoProduto,
                DescricaoProduto = produto.DescricaoProduto,
                Saldo = produto.Saldo
            })
            .ToList();

        return new ControleEstoque(produtos);
    }

    public IReadOnlyList<Produto> Produtos => _produtos;

    public IReadOnlyList<MovimentacaoEstoque> Historico => _historico;

    public Produto? Obter(int codigo) =>
        _produtos.FirstOrDefault(produto => produto.CodigoProduto == codigo);

    public ResultadoMovimentacao RegistrarEntrada(int codigo, int quantidade, string? descricao) =>
        Registrar(TipoMovimentacao.Entrada, codigo, quantidade, descricao);

    public ResultadoMovimentacao RegistrarSaida(int codigo, int quantidade, string? descricao) =>
        Registrar(TipoMovimentacao.Saida, codigo, quantidade, descricao);

    private ResultadoMovimentacao Registrar(TipoMovimentacao tipo, int codigo, int quantidade, string? descricao)
    {
        if (quantidade <= 0)
            return ResultadoMovimentacao.Falha("A quantidade deve ser um número inteiro maior que zero.");

        var produto = Obter(codigo);
        if (produto is null)
            return ResultadoMovimentacao.Falha($"Produto {codigo} não encontrado.");

        if (tipo == TipoMovimentacao.Saida && quantidade > produto.Saldo)
        {
            // A saída não pode deixar saldo negativo. O estoque e o próximo identificador ficam como estavam.
            return ResultadoMovimentacao.Falha(
                $"A saída de {quantidade} unidades ultrapassa o estoque disponível ({produto.Saldo}) de {produto.DescricaoProduto}.");
        }

        if (tipo == TipoMovimentacao.Entrada && produto.Saldo > int.MaxValue - quantidade)
            return ResultadoMovimentacao.Falha("A quantidade informada é grande demais para o estoque atual.");

        var anterior = produto.Saldo;
        produto.Saldo = tipo == TipoMovimentacao.Entrada
            ? anterior + quantidade
            : anterior - quantidade;

        var texto = string.IsNullOrWhiteSpace(descricao)
            ? (tipo == TipoMovimentacao.Entrada ? "Entrada" : "Saída")
            : descricao.Trim();

        var movimentacao = new MovimentacaoEstoque
        {
            Identificador = _proximoIdentificador,
            CodigoProduto = produto.CodigoProduto,
            DescricaoProduto = produto.DescricaoProduto,
            Descricao = texto,
            Tipo = tipo,
            Quantidade = quantidade,
            EstoqueAnterior = anterior,
            EstoqueFinal = produto.Saldo
        };

        _proximoIdentificador++;
        _historico.Add(movimentacao);
        return ResultadoMovimentacao.Ok(movimentacao);
    }
}
