using CatalogoApi.Models;

namespace CatalogoApi.DTOs.Mappings;

public static class ProdutoDTOMappingExtensions
{

    public static ProdutoDTO? ToProdutosDTO(this Produto produto)
    {
        if (produto is null)
            return null;


        return new ProdutoDTO
        {
          ProdutoId = produto.ProdutoId,
            Nome = produto.Nome,
            Descrição = produto.Descrição,
            Preco = produto.Preco,
            ImagemUrl = produto.ImagemUrl,
            Estoque = produto.Estoque,
            DataCadastro = produto.DataCadastro,
            CategoriaId = produto.CategoriaId,
        };
    }

    public static Produto? ToProdutos(this ProdutoDTO produtoDTO)
    {
        if (produtoDTO is null)
            return null;
        return new Produto
        {
          ProdutoId = produtoDTO.ProdutoId,
            Nome = produtoDTO.Nome,
            Descrição = produtoDTO.Descrição,
            Preco = produtoDTO.Preco,
            ImagemUrl = produtoDTO.ImagemUrl,
            Estoque = produtoDTO.Estoque,
            DataCadastro = produtoDTO.DataCadastro,
            CategoriaId = produtoDTO.CategoriaId,
        };
    }

    public static IEnumerable<ProdutoDTO> ToProdutosDTOList(this IEnumerable<Produto> produtos)
    {
        if (produtos is null || !produtos.Any())
            return new List<ProdutoDTO>();

        return produtos.Select(categoria => new ProdutoDTO
        {
          ProdutoId = categoria.ProdutoId,
            Nome = categoria.Nome,
            Descrição = categoria.Descrição,
            Preco = categoria.Preco,
            ImagemUrl = categoria.ImagemUrl,
            Estoque = categoria.Estoque,
            DataCadastro = categoria.DataCadastro,
            CategoriaId = categoria.CategoriaId,

        }).ToList();
    }



}
