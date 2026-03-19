using CatalogoApi.Models;
using CatalogoApi.Pagination;

namespace CatalogoApi.Repositories.Interface;

public interface IProdutoRepository : IRepository<Produto>
{

    PagedList<Produto> GetProdutos(ProdutosParameters produtosParams);
    IEnumerable<Produto> GetProdutosByCategoriaId(int id);

    PagedList<Produto> GetProdutosFiltroPreco(ProdutosFiltroPreco produtosFiltroParams);

}
