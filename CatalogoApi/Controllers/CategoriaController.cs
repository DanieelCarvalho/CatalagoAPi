using CatalogoApi.DTOs;
using CatalogoApi.DTOs.Mappings;
using CatalogoApi.Filters;
using CatalogoApi.Models;
using CatalogoApi.Pagination;
using CatalogoApi.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using X.PagedList;


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
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
    {

        _logger.LogInformation("===============GET api/categorias/ produtos=================");
        var categorias = await _repository.GetAllAsync();
        if (categorias is null)
            return NotFound("Categoria não encontrada");


        var categoriasDto =  categorias.ToCategoriaDTOList();

        return Ok(categoriasDto);

    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<CategoriaDTO>> Get(int id)
    {

        var categoria = await _repository.GetAsync(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id {id} não encontrada...");
            return NotFound("Categoria não encontrado");
        }

      

        var categoriaDto = categoria.ToCategoriaDTO();

        return Ok(categoriaDto);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasPaginacao([FromQuery] CategoriasParameters categoriasParameters)
    {
        var categorias = await _repository.GetCategoriasAsync(categoriasParameters);
     

        //var categoriasDto = categorias.ToCategoriaDTOList();
        return ObterCategorias(categorias);
    }

    [HttpGet("filter/nome/pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFiltroNomePaginacao([FromQuery] CategoriaFiltroNome categoriaFiltroNome)
    {
        var categorias = await _repository.GetCategoriasFiltroNomeAsync(categoriaFiltroNome);
        return ObterCategorias(categorias);
    }

    private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(IPagedList<Categoria>? categorias)
    {
        if (categorias is null)
            return NotFound("Categoria não encontrada");
        var metadata = new
        {
            categorias.Count,
            categorias.PageSize,
            categorias.PageCount,
            categorias.TotalItemCount,
            categorias.HasNextPage,
            categorias.HasPreviousPage
        };
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        var categoriasDto = categorias.ToCategoriaDTOList();
        return Ok(categoriasDto);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

         var categoria = categoriaDto.ToCategoria();

        var cartegoriaCriada = await _repository.CreateAsync(categoria);

        var novaCategoriaDto = cartegoriaCriada.ToCategoriaDTO();

        return new CreatedAtRouteResult("ObterCategoria", 
                                        new { id = novaCategoriaDto.CategoriaId }, 
                                        categoria);
    }

    [HttpPut("id{id:int}")]
    public async Task<ActionResult> Put(int id, CategoriaDTO categoriaDto)
    {
        if (id != categoriaDto.CategoriaId)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

        var categoria = categoriaDto.ToCategoria();

        var categoriaAtualizada= await _repository.UpdateAsync(categoria);

        var categoriaAtualizadaDto = categoriaAtualizada.ToCategoriaDTO();

        return Ok(categoriaAtualizadaDto);

    }

    [HttpDelete("id{id:int}")]
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _repository.GetAsync(c => c.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id {id} não encontrada...");
            return NotFound("Categoria não encontrada");
        }
       var categoraiExcluida = await _repository.DeleteAsync(categoria);

        var categoriaExcluidaDto =  categoraiExcluida.ToCategoriaDTO();

        return Ok(categoriaExcluidaDto);
    }

}