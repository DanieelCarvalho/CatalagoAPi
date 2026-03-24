using CatalogoApi.Models;
using CatalogoApi.Pagination;
using X.PagedList;

namespace CatalogoApi.Repositories.Interface;

public interface ICategoriaRepository : IRepository<Categoria>
{

    Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParams);

    Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriaFiltroNome categoriaParams);
}
