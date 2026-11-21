using System.Text.Json.Serialization;

namespace TargetSistemas;

public sealed class Produto
{
    public int CodigoProduto { get; set; }

    public string DescricaoProduto { get; set; } = "";

    [JsonPropertyName("estoque")]
    public int Saldo { get; set; }
}
