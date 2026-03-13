using CatalogoApi.Models;

namespace CatalogoApi.Repositories.Interface;

public interface IProdutoRepository : IRepository<Produto>
{
    IEnumerable<Produto> GetProdutosByCategoriaId(int id);

}
