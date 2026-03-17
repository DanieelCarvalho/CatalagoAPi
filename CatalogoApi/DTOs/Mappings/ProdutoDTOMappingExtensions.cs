using CatalogoApi.Models;

namespace CatalogoApi.DTOs.Mappings;

public static class ProdutoDTOMappingExtensions
{

    public static ProdutoDTOCreated? ToProdutoDTOCreated(this Produto produto)
    {
        if (produto is null)
            return null;


        return new ProdutoDTOCreated
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

    public static Produto? ToProduto(this ProdutoDTOCreated produtoDTOCreated)
    {
        if (produtoDTOCreated is null)
            return null;
        return new Produto
        {
          ProdutoId = produtoDTOCreated.ProdutoId,
            Nome = produtoDTOCreated.Nome,
            Descrição = produtoDTOCreated.Descrição,
            Preco = produtoDTOCreated.Preco,
            ImagemUrl = produtoDTOCreated.ImagemUrl,
            Estoque = produtoDTOCreated.Estoque,
            DataCadastro = produtoDTOCreated.DataCadastro,
            CategoriaId = produtoDTOCreated.CategoriaId,
        };
    }

    //public static IEnumerable<ProdutoDTOCreated> ToProdutosDTOList(this IEnumerable<Produto> produtos)
    //{
    //    if (produtos is null || !produtos.Any())
    //        return new List<ProdutoDTOCreated>();

    //    return produtos.Select(categoria => new ProdutoDTOCreated
    //    {
    //      ProdutoId = categoria.ProdutoId,
    //        Nome = categoria.Nome,
    //        Descrição = categoria.Descrição,
    //        Preco = categoria.Preco,
    //        ImagemUrl = categoria.ImagemUrl,
    //        Estoque = categoria.Estoque,
    //        DataCadastro = categoria.DataCadastro,
    //        CategoriaId = categoria.CategoriaId,

    //    }).ToList();
    //}

    public static ProdutoDTOResponse? ToProdutoDTOResponse(this Produto produto)
    {
        if (produto is null)
            return null;

        return new ProdutoDTOResponse
        {
            ProdutoId = produto.ProdutoId,
            Nome = produto.Nome,
            Descrição = produto.Descrição,
            Preco = produto.Preco,
            ImagemUrl = produto.ImagemUrl,
            Estoque = produto.Estoque,
            DataCadastro = produto.DataCadastro,
            CategoriaId = produto.CategoriaId
        };
    }
    public static IEnumerable<ProdutoDTOResponse> ToProdutoDTOResponseList(this IEnumerable<Produto> produtos)
    {
        if (produtos is null || !produtos.Any())
            return new List<ProdutoDTOResponse>();

        return produtos.Select(produto => new ProdutoDTOResponse
        {
            ProdutoId = produto.ProdutoId,
            Nome = produto.Nome,
            Descrição = produto.Descrição,
            Preco = produto.Preco,
            ImagemUrl = produto.ImagemUrl,
            Estoque = produto.Estoque,
            DataCadastro = produto.DataCadastro,
            CategoriaId = produto.CategoriaId
        }).ToList();
    }

    public static ProdutoDTOUpdateResponse? ToProdutoUpdateResponseDTO(this Produto produto)
    {
        if (produto is null)
            return null;

        return new ProdutoDTOUpdateResponse
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

    public static IEnumerable<ProdutoDTOUpdateResponse> ToProdutoUpdateResponseDTOList(this IEnumerable<Produto> produtos)
    {
        if (produtos is null || !produtos.Any())
            return new List<ProdutoDTOUpdateResponse>();

        return produtos.Select(produto => new ProdutoDTOUpdateResponse
        {
            ProdutoId = produto.ProdutoId,
            Nome = produto.Nome,
            Descrição = produto.Descrição,
            Preco = produto.Preco,
            ImagemUrl = produto.ImagemUrl,
            Estoque = produto.Estoque,
            DataCadastro = produto.DataCadastro,
            CategoriaId = produto.CategoriaId
        }).ToList();
    }



    public static Produto? ToProduto(this ProdutoDTOUpdateRequest produtoDTOUpdatRequest)
    {
        if (produtoDTOUpdatRequest is null)
            return null;


        return new Produto
        {
       
            DataCadastro = produtoDTOUpdatRequest.DataCadastro,
            Estoque = produtoDTOUpdatRequest.Estoque,
        };
    }

    public static Produto UpdateFromRequest(this Produto produto, ProdutoDTOUpdateRequest updateRequest)
    {
        if (updateRequest is null || produto is null)
            return produto;

        // Atualiza apenas as propriedades que estão presentes no request
        if (updateRequest.DataCadastro.HasValue)
            produto.DataCadastro = updateRequest.DataCadastro;
        produto.Estoque = updateRequest.Estoque; 
        //if (updateRequest.CategoriaId > 0)
        //    produto.CategoriaId = updateRequest.CategoriaId;

        return produto;
    }

    public static ProdutoDTOUpdateRequest ToProdutoUpdateRequest(this Produto produto)
    {
        if (produto is null)
            return null;

        return new ProdutoDTOUpdateRequest
        {
            DataCadastro = produto.DataCadastro,
            Estoque = produto.Estoque
        };
    }
}



