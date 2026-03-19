using CatalogoApi.Models;
using CatalogoApi.Pagination;

namespace CatalogoApi.Repositories.Interface;

public interface ICategoriaRepository : IRepository<Categoria>
{

    PagedList<Categoria> GetCategorias(CategoriasParameters categoriasParams);

    PagedList<Categoria> GetCategoriasFiltroNome(CategoriaFiltroNome categoriaParams);
}
