using CatalogoApi.Context;
using CatalogoApi.Models;
using CatalogoApi.Pagination;
using CatalogoApi.Repositories.Interface;
using X.PagedList;


namespace CatalogoApi.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParams)
    {
        var categorias = await GetAllAsync();

        var categoriasOrdenadas = categorias.OrderBy(c => c.Nome).AsQueryable();

        //var  resultado = PagedList<Categoria>.ToPagedList(categoriasOrdenadas, 
        //                                                             categoriasParams.PageNumber, 
        //                                                             categoriasParams.PageSize);

        var resultado = await categoriasOrdenadas.ToPagedListAsync(categoriasParams.PageNumber, 
                                                             categoriasParams.PageSize );
        return resultado;
    }

    public async Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriaFiltroNome categoriaParams)
    {
        var categorias = await GetAllAsync();


        if (!string.IsNullOrEmpty(categoriaParams.Nome))
        {
            categorias = categorias.Where(c => 
            c.Nome.Contains(categoriaParams.Nome, StringComparison.OrdinalIgnoreCase));
        }

        //var categoriasFiltradas = PagedList<Categoria>.ToPagedList(
        //    categorias.AsQueryable(),
        //    categoriaParams.PageNumber, 
        //    categoriaParams.PageSize); 

        var categoriasFiltradas = await categorias.ToPagedListAsync(categoriaParams.PageNumber, 
                                                                    categoriaParams.PageSize);

        return categoriasFiltradas;
    }
}
