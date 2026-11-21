using System.Globalization;
using System.Text;

namespace TargetSistemas;

public static class Program
{
    public static int Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        var cultura = CultureInfo.GetCultureInfo("pt-BR");
        CultureInfo.CurrentCulture = cultura;
        CultureInfo.CurrentUICulture = cultura;

        try
        {
            var aplicacao = new AplicacaoConsole(
                Console.In,
                Console.Out,
                Path.Combine(AppContext.BaseDirectory, "Dados"),
                DateOnly.FromDateTime(DateTime.Today));

            aplicacao.Executar();
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Ocorreu um erro inesperado: " + ex.Message);
            return 1;
        }
    }
}
