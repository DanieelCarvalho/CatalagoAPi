using CatalogoApi.Context;
using CatalogoApi.Models;
using CatalogoApi.Pagination;
using CatalogoApi.Repositories.Interface;

namespace CatalogoApi.Repositories;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }

    public PagedList<Produto> GetProdutos(ProdutosParameters produtosParams)
    {
        var produtos=  GetAll().OrderBy(p => p.ProdutoId).AsQueryable();
        var ProdutosOrdenados = PagedList<Produto>.ToPagedList(produtos, 
                    produtosParams.PageNumber, produtosParams.PageSize);

        return ProdutosOrdenados;

    }

    public IEnumerable<Produto> GetProdutosByCategoriaId(int id)
    {
        return GetAll().Where(p => p.CategoriaId == id);
    }

    public PagedList<Produto> GetProdutosFiltroPreco(ProdutosFiltroPreco produtosFiltroParams)
    {
        var produtos = GetAll().AsQueryable();

        if(produtosFiltroParams.Preco.HasValue && !string.IsNullOrEmpty(produtosFiltroParams.PrecoCriterio))
        {
            if (produtosFiltroParams.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.Preco > produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
            }
            else if (produtosFiltroParams.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.Preco < produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
            }
            else if (produtosFiltroParams.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.Preco == produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
            }
        }

        var produtosOrdenados = PagedList<Produto>.ToPagedList(produtos, 
                                                               produtosFiltroParams.PageNumber, 
                                                               produtosFiltroParams.PageSize);

        return produtosOrdenados;

    }
}
