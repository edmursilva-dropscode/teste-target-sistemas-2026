namespace TargetSistemas.Testes;

public class ConsoleFluxoTestes
{
    [Fact]
    public void Calcula_comissoes_pelo_console_e_volta_ao_menu()
    {
        var saida = ApoioTestes.ExecutarConsole("1\n0\n");

        Assert.Contains("João Silva", saida);
        Assert.Contains("R$ 10.754,70", saida);
        Assert.Contains("R$ 495,69", saida);
        Assert.Contains("Maria Souza", saida);
        Assert.Contains("R$ 465,96", saida);
        Assert.Contains("Carlos Oliveira", saida);
        Assert.Contains("R$ 379,38", saida);
        Assert.Contains("Ana Lima", saida);
        Assert.Contains("R$ 404,99", saida);
        Assert.Contains("R$ 37.321,30", saida);
        Assert.Contains("R$ 1.746,02", saida);
        Assert.Contains("1 - Calcular comissões de vendas", saida);
        Assert.True(saida.Split("Desafio Target Sistemas").Length >= 3);
        Assert.Contains("Programa encerrado.", saida);
    }

    [Fact]
    public void Movimenta_estoque_pelo_console_e_recusa_saida_invalida()
    {
        var entrada = string.Join('\n',
            "2",
            "1",
            "2",
            "102",
            "5",
            "compra",
            "3",
            "101",
            "10",
            "venda",
            "3",
            "101",
            "1000",
            "nao deve entrar",
            "1",
            "4",
            "0",
            "0");

        var saida = ApoioTestes.ExecutarConsole(entrada);

        Assert.Contains("Caneta Azul", saida);
        Assert.Contains("Caderno Universitário", saida);
        Assert.Contains("Estoque final: 80", saida);
        Assert.Contains("Estoque final: 140", saida);
        Assert.Contains("ultrapassa o estoque disponível", saida);
        Assert.Contains("Movimentação 1 registrada.", saida);
        Assert.Contains("Movimentação 2 registrada.", saida);
        Assert.DoesNotContain("Movimentação 3 registrada.", saida);
        Assert.Contains("1 | Entrada | 102 - Caderno Universitário", saida);
        Assert.Contains("2 | Saída | 101 - Caneta Azul", saida);
        Assert.Contains("Programa encerrado.", saida);
    }

    [Fact]
    public void Mantem_estoque_ao_sair_do_submenu_e_continua_os_identificadores()
    {
        var entrada = string.Join('\n',
            "2",
            "1",
            "3",
            "101",
            "10",
            "venda",
            "0",
            "1",
            "2",
            "1",
            "4",
            "3",
            "101",
            "1000",
            "excesso",
            "2",
            "101",
            "1",
            "reposicao",
            "0",
            "0");

        var saida = ApoioTestes.ExecutarConsole(entrada);

        Assert.Contains(LinhaEstoque(101, "Caneta Azul", 150), saida);
        Assert.Contains("Movimentação 1 registrada.", saida);
        Assert.Contains("Estoque final: 140", saida);
        Assert.Contains("R$ 495,69", saida);
        Assert.Contains(LinhaEstoque(101, "Caneta Azul", 140), saida);
        Assert.Contains("1 | Saída | 101 - Caneta Azul", saida);
        Assert.Contains("ultrapassa o estoque disponível", saida);
        Assert.Contains("Movimentação 2 registrada.", saida);
        Assert.Contains("Estoque final: 141", saida);
        Assert.DoesNotContain("Movimentação 3 registrada.", saida);
        Assert.Contains("Programa encerrado.", saida);
    }

    [Fact]
    public void Nova_execucao_carrega_o_estoque_original()
    {
        var primeira = ApoioTestes.ExecutarConsole(string.Join('\n',
            "2", "3", "101", "10", "venda", "0", "0"));

        Assert.Contains("Estoque final: 140", primeira);
        Assert.Contains("Programa encerrado.", primeira);

        var segunda = ApoioTestes.ExecutarConsole(string.Join('\n',
            "2", "1", "4", "0", "0"));

        Assert.Contains(LinhaEstoque(101, "Caneta Azul", 150), segunda);
        Assert.DoesNotContain(LinhaEstoque(101, "Caneta Azul", 140), segunda);
        Assert.Contains("Nenhuma movimentação registrada nesta execução.", segunda);
    }

    [Fact]
    public void Calcula_juros_de_tres_dias_pelo_console()
    {
        var vencimento = DateOnly.FromDateTime(DateTime.Today).AddDays(-3);
        var entrada = "3\n200,00\n" + vencimento.ToString("dd/MM/yyyy") + "\n0\n";

        var saida = ApoioTestes.ExecutarConsole(entrada);

        Assert.Contains("Dias de atraso: 3", saida);
        Assert.Contains("Juros: R$ 15,00", saida);
        Assert.Contains("Valor final: R$ 215,00", saida);
        Assert.Contains("Valor original: R$ 200,00", saida);
        Assert.Contains("Programa encerrado.", saida);
    }

    [Fact]
    public void Informa_opcao_invalida_e_encerra_pelo_menu()
    {
        var saida = ApoioTestes.ExecutarConsole("9\nabc\n0\n");

        Assert.Equal(2, Contar(saida, "Opção inválida. Escolha 1, 2, 3 ou 0."));
        Assert.Contains("Programa encerrado.", saida);
    }

    private static string LinhaEstoque(int codigo, string descricao, int saldo) =>
        $"{codigo,-8}{descricao,-32}{saldo,8}";

    private static int Contar(string texto, string trecho)
    {
        var quantidade = 0;
        var indice = 0;
        while ((indice = texto.IndexOf(trecho, indice, StringComparison.Ordinal)) >= 0)
        {
            quantidade++;
            indice += trecho.Length;
        }

        return quantidade;
    }
}
