using CatalogoApi.Models;
using CatalogoApi.Pagination;
using X.PagedList;

namespace CatalogoApi.Repositories.Interface;

public interface IProdutoRepository : IRepository<Produto>
{

    Task<IPagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParams);
    Task<IEnumerable<Produto>> GetProdutosByCategoriaIdAsync(int id);

    Task<IPagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroParams);

}
