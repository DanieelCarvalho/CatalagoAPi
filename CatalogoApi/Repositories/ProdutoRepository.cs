using CatalogoApi.Context;
using CatalogoApi.Models;
using CatalogoApi.Repositories.Interface;

namespace CatalogoApi.Repositories;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }
    public IEnumerable<Produto> GetProdutosByCategoriaId(int id)
    {
        return GetAll().Where(p => p.CategoriaId == id);
    }

}
