using System.Globalization;

namespace TargetSistemas;

public static class Formatacao
{
    private static readonly CultureInfo Portugues = CultureInfo.GetCultureInfo("pt-BR");

    public static string Moeda(decimal valor) =>
        "R$ " + valor.ToString("N2", Portugues);

    public static string Data(DateOnly data) =>
        data.ToString("dd/MM/yyyy", Portugues);

    public static string Percentual(decimal percentual) =>
        (percentual * 100m).ToString("0.##", Portugues) + "%";
}
