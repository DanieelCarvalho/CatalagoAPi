using CatalogoApi.Models;
using CatalogoApi.Pagination;

namespace CatalogoApi.Repositories.Interface;

public interface IProdutoRepository : IRepository<Produto>
{

    IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParams);
    IEnumerable<Produto> GetProdutosByCategoriaId(int id);

}
