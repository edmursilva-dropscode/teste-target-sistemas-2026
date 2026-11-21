using System.Text.Json;

namespace TargetSistemas;

public static class LeitorDados
{
    private static readonly JsonSerializerOptions Opcoes = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public static IReadOnlyList<Venda> LerVendas(string caminho)
    {
        var arquivo = Ler<ArquivoVendas>(caminho);
        return arquivo.Vendas ?? new List<Venda>();
    }

    public static IReadOnlyList<Produto> LerProdutos(string caminho)
    {
        var arquivo = Ler<ArquivoEstoque>(caminho);
        return arquivo.Estoque ?? new List<Produto>();
    }

    private static T Ler<T>(string caminho)
    {
        if (!File.Exists(caminho))
            throw new FileNotFoundException("Arquivo de dados não encontrado.", caminho);

        var json = File.ReadAllText(caminho);
        return JsonSerializer.Deserialize<T>(json, Opcoes)
            ?? throw new InvalidDataException("O arquivo não contém dados válidos: " + caminho);
    }

    private sealed class ArquivoVendas
    {
        public List<Venda>? Vendas { get; set; }
    }

    private sealed class ArquivoEstoque
    {
        public List<Produto>? Estoque { get; set; }
    }
}
