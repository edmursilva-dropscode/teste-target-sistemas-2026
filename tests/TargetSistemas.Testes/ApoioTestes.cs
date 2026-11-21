using System.Diagnostics;
using System.Text;

namespace TargetSistemas.Testes;

internal static class ApoioTestes
{
    public static string RaizDoRepositorio()
    {
        var diretorio = new DirectoryInfo(AppContext.BaseDirectory);
        while (diretorio is not null)
        {
            if (File.Exists(Path.Combine(diretorio.FullName, "TargetSistemas.sln")))
                return diretorio.FullName;

            diretorio = diretorio.Parent;
        }

        throw new DirectoryNotFoundException("Não foi possível localizar a pasta do projeto.");
    }

    public static string ArquivoDados(string nome) =>
        Path.Combine(RaizDoRepositorio(), "src", "TargetSistemas.Console", "Dados", nome);

    public static string DllDoConsole()
    {
        var raiz = RaizDoRepositorio();
        var candidatos = new[]
        {
            Path.Combine(raiz, "src", "TargetSistemas.Console", "bin", "Debug", "net10.0", "TargetSistemas.Console.dll"),
            Path.Combine(raiz, "src", "TargetSistemas.Console", "bin", "Release", "net10.0", "TargetSistemas.Console.dll")
        };

        var dll = candidatos.FirstOrDefault(File.Exists);
        if (dll is null)
            throw new FileNotFoundException("O console ainda não foi compilado.", candidatos[0]);

        return dll;
    }

    public static string ExecutarConsole(string entrada)
    {
        var dll = DllDoConsole();
        var processo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "exec \"" + dll + "\"",
            WorkingDirectory = Path.GetDirectoryName(dll)!,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        using var execucao = Process.Start(processo)
            ?? throw new InvalidOperationException("Não foi possível iniciar o console.");

        var saida = execucao.StandardOutput.ReadToEndAsync();
        var erro = execucao.StandardError.ReadToEndAsync();
        execucao.StandardInput.Write(entrada.Replace("\r\n", "\n"));
        execucao.StandardInput.Close();

        if (!execucao.WaitForExit(20000))
        {
            execucao.Kill(entireProcessTree: true);
            throw new TimeoutException("O console não encerrou no tempo esperado.");
        }

        var textoSaida = saida.GetAwaiter().GetResult();
        var textoErro = erro.GetAwaiter().GetResult();
        if (execucao.ExitCode != 0)
            throw new InvalidOperationException($"O console terminou com código {execucao.ExitCode}.{Environment.NewLine}{textoErro}{Environment.NewLine}{textoSaida}");

        return textoSaida;
    }
}
