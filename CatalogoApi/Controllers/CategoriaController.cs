using CatalogoApi.DTOs;
using CatalogoApi.DTOs.Mappings;
using CatalogoApi.Filters;
using CatalogoApi.Models;
using CatalogoApi.Pagination;
using CatalogoApi.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace CatalogoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriaController  : ControllerBase
{
    private readonly ICategoriaRepository _repository;
    private readonly ILogger<CategoriaController> _logger;

    public CategoriaController(ICategoriaRepository repository,
                               ILogger<CategoriaController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    [ServiceFilter(typeof(ApiLogginFilter))]
    public ActionResult<IEnumerable<CategoriaDTO>> Get()
    {

        _logger.LogInformation("===============GET api/categorias/ produtos=================");
        var categorias = _repository.GetAll();
        if (categorias is null)
            return NotFound("Categoria não encontrada");


        var categoriasDto = categorias.ToCategoriaDTOList();

        return Ok(categoriasDto);

    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public ActionResult<CategoriaDTO> Get(int id)
    {

        var categoria = _repository.Get(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id {id} não encontrada...");
            return NotFound("Categoria não encontrado");
        }

      

        var categoriaDto = categoria.ToCategoriaDTO();

        return Ok(categoriaDto);
    }

    [HttpGet("pagination")]
    public ActionResult<IEnumerable<CategoriaDTO>> GetCategoriasPaginacao([FromQuery] CategoriasParameters categoriasParameters)
    {
        var categorias = _repository.GetCategorias(categoriasParameters);
        if (categorias is null)
            return NotFound("Categoria não encontrada");
        var metadata = new
        {
            categorias.TotalCount,
            categorias.PageSize,
            categorias.CurrentPage,
            categorias.TotalPages,
            categorias.HasNext,
            categorias.HasPrevious
        };
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

        var categoriasDto = categorias.ToCategoriaDTOList();
        return Ok(categoriasDto);
    }

    [HttpGet("filter/nome/pagination")]
    public ActionResult<IEnumerable<CategoriaDTO>> GetCategoriasFiltroNomePaginacao([FromQuery] CategoriaFiltroNome categoriaFiltroNome)
    {
        var categorias = _repository.GetCategoriasFiltroNome(categoriaFiltroNome);
        return ObterCategorias(categorias);
    }

    private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(PagedList<Categoria>? categorias)
    {
        if (categorias is null)
            return NotFound("Categoria não encontrada");
        var metadata = new
        {
            categorias.TotalCount,
            categorias.PageSize,
            categorias.CurrentPage,
            categorias.TotalPages,
            categorias.HasNext,
            categorias.HasPrevious
        };
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        var categoriasDto = categorias.ToCategoriaDTOList();
        return Ok(categoriasDto);
    }

    [HttpPost]
    public ActionResult<CategoriaDTO> Post(CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

         var categoria = categoriaDto.ToCategoria();

        var cartegoriaCriada = _repository.Create(categoria);

        var novaCategoriaDto = cartegoriaCriada.ToCategoriaDTO();

        return new CreatedAtRouteResult("ObterCategoria", 
                                        new { id = novaCategoriaDto.CategoriaId }, 
                                        categoria);
    }

    [HttpPut("id{id:int}")]
    public ActionResult Put(int id, CategoriaDTO categoriaDto)
    {
        if (id != categoriaDto.CategoriaId)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

        var categoria = categoriaDto.ToCategoria();

        var categoriaAtualizada= _repository.Update(categoria);

        var categoriaAtualizadaDto = categoriaAtualizada.ToCategoriaDTO();

        return Ok(categoriaAtualizadaDto);

    }

    [HttpDelete("id{id:int}")]
    public ActionResult<CategoriaDTO> Delete(int id)
    {
        var categoria = _repository.Get(c => c.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id {id} não encontrada...");
            return NotFound("Categoria não encontrada");
        }
       var categoraiExcluida = _repository.Delete(categoria);

        var categoriaExcluidaDto = categoraiExcluida.ToCategoriaDTO();

        return Ok(categoriaExcluidaDto);
    }

}