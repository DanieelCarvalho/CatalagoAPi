namespace CatalogoApi.DTOs;

public class ProdutoDTOResponse
{

    public int ProdutoId { get; set; }
    public string? Nome { get; set; }
    public string? Descrição { get; set; }
    public decimal? Preco { get; set; }
    public string? ImagemUrl { get; set; }
    public float Estoque { get; set; }
    public DateTime? DataCadastro { get; set; }
    public int CategoriaId { get; set; }
}
