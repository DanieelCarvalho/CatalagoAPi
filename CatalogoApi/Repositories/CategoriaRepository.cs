using CatalogoApi.Context;
using CatalogoApi.Models;
using CatalogoApi.Repositories.Interface;

namespace CatalogoApi.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

}
