namespace TargetSistemas;

public sealed class ResultadoJuros
{
    public bool Sucesso { get; private init; }

    public string Mensagem { get; private init; } = "";

    public decimal ValorOriginal { get; private init; }

    public DateOnly Vencimento { get; private init; }

    public DateOnly DataReferencia { get; private init; }

    public int DiasAtraso { get; private init; }

    public decimal Juros { get; private init; }

    public decimal ValorFinal { get; private init; }

    public static ResultadoJuros Falha(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem };

    public static ResultadoJuros Ok(
        decimal valorOriginal,
        DateOnly vencimento,
        DateOnly dataReferencia,
        int diasAtraso,
        decimal juros,
        decimal valorFinal) =>
        new()
        {
            Sucesso = true,
            Mensagem = diasAtraso == 0
                ? "Não há atraso. Nenhum juro foi aplicado."
                : "Juros calculados.",
            ValorOriginal = valorOriginal,
            Vencimento = vencimento,
            DataReferencia = dataReferencia,
            DiasAtraso = diasAtraso,
            Juros = juros,
            ValorFinal = valorFinal
        };
}
