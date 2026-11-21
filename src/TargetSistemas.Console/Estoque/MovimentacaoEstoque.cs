namespace TargetSistemas;

public enum TipoMovimentacao
{
    Entrada,
    Saida
}

public sealed class MovimentacaoEstoque
{
    public required int Identificador { get; init; }

    public required int CodigoProduto { get; init; }

    public required string DescricaoProduto { get; init; }

    public required string Descricao { get; init; }

    public required TipoMovimentacao Tipo { get; init; }

    public required int Quantidade { get; init; }

    public required int EstoqueAnterior { get; init; }

    public required int EstoqueFinal { get; init; }
}

public sealed class ResultadoMovimentacao
{
    public bool Sucesso { get; private init; }

    public string Mensagem { get; private init; } = "";

    public MovimentacaoEstoque? Movimentacao { get; private init; }

    public static ResultadoMovimentacao Falha(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem };

    public static ResultadoMovimentacao Ok(MovimentacaoEstoque movimentacao) =>
        new()
        {
            Sucesso = true,
            Mensagem = "Movimentação registrada.",
            Movimentacao = movimentacao
        };
}
