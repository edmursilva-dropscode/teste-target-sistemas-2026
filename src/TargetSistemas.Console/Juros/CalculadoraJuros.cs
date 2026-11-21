using System.Globalization;
using System.Text.RegularExpressions;

namespace TargetSistemas;

public static class CalculadoraJuros
{
    public const decimal TaxaDiaria = 0.025m;

    public static ResultadoJuros Calcular(decimal valorOriginal, DateOnly vencimento, DateOnly dataReferencia)
    {
        if (valorOriginal <= 0)
            return ResultadoJuros.Falha("Informe um valor maior que zero.");

        var dias = dataReferencia.DayNumber - vencimento.DayNumber;
        if (dias < 0)
            dias = 0;

        // Juros simples de 2,5% ao dia sobre o valor original, sem capitalização e sem multa extra.
        var juros = decimal.Round(valorOriginal * TaxaDiaria * dias, 2, MidpointRounding.AwayFromZero);

        return ResultadoJuros.Ok(valorOriginal, vencimento, dataReferencia, dias, juros, valorOriginal + juros);
    }

    public static ResultadoJuros Interpretar(string? textoValor, string? textoData, DateOnly dataReferencia)
    {
        if (string.IsNullOrWhiteSpace(textoValor))
            return ResultadoJuros.Falha("Informe o valor original.");

        if (!TentarLerValor(textoValor, out var valor))
            return ResultadoJuros.Falha("O valor informado não é um número válido.");

        if (string.IsNullOrWhiteSpace(textoData))
            return ResultadoJuros.Falha("Informe a data de vencimento no formato dd/MM/yyyy.");

        if (!TentarLerData(textoData, out var vencimento))
            return ResultadoJuros.Falha("A data de vencimento não é válida. Use o formato dd/MM/yyyy.");

        return Calcular(valor, vencimento, dataReferencia);
    }

    public static bool TentarLerValor(string texto, out decimal valor)
    {
        valor = 0;
        var limpo = texto.Trim().Replace("R$", "", StringComparison.OrdinalIgnoreCase).Trim();
        if (limpo.Length == 0)
            return false;

        if (Regex.IsMatch(limpo, @"^-?\d+(\.\d+)?$"))
            return decimal.TryParse(limpo, NumberStyles.Number, CultureInfo.InvariantCulture, out valor);

        return decimal.TryParse(limpo, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), out valor);
    }

    public static bool TentarLerData(string texto, out DateOnly data)
    {
        return DateOnly.TryParseExact(
            texto.Trim(),
            ["dd/MM/yyyy", "d/M/yyyy"],
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out data);
    }
}
