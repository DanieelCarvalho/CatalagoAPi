using CatalogoApi.Context;
using CatalogoApi.Models;
using CatalogoApi.Pagination;
using CatalogoApi.Repositories.Interface;

namespace CatalogoApi.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

    public PagedList<Categoria> GetCategorias(CategoriasParameters categoriasParams)
    {
        var categorias = GetAll().OrderBy(c => c.CategoriaId).AsQueryable();

        var  categoriasOrdenadas = PagedList<Categoria>.ToPagedList(categorias, 
                                                                     categoriasParams.PageNumber, 
                                                                     categoriasParams.PageSize);

        return categoriasOrdenadas;
    }

    public PagedList<Categoria> GetCategoriasFiltroNome(CategoriaFiltroNome categoriaParams)
    {
       var categorias = GetAll().AsQueryable();

        if (!string.IsNullOrEmpty(categoriaParams.Nome))
        {
            categorias = categorias.Where(c => 
            c.Nome.Contains(categoriaParams.Nome, StringComparison.OrdinalIgnoreCase));
        }

        var categoriasFiltradas = PagedList<Categoria>.ToPagedList(categorias,
            categoriaParams.PageNumber, categoriaParams.PageSize); 


        return categoriasFiltradas;
    }
}
