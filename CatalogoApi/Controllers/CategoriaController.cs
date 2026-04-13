using CatalogoApi.DTOs;
using CatalogoApi.DTOs.Mappings;
using CatalogoApi.Filters;
using CatalogoApi.Models;
using CatalogoApi.Pagination;
using CatalogoApi.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;
using X.PagedList;
using Microsoft.AspNetCore.Http;

namespace CatalogoApi.Controllers;

[EnableCors("OrigensComAcessoPermitido")]
[Route("[controller]")]
[ApiController]
//[ApiConventionType(typeof(DefaultApiConventions))]

//[EnableRateLimiting("Fixedwindow")]
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

    /// <summary>
    /// Obtem uma lista de onjetos Categorias
    /// </summary>
    /// <returns>
    /// Uma lista de objetos Categoria
    /// </returns>
    /// 
    //[Authorize]
    [HttpGet]
    [DisableRateLimiting]
    [ServiceFilter(typeof(ApiLogginFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
    {

        _logger.LogInformation("===============GET api/categorias/ produtos=================");
        var categorias = await _repository.GetAllAsync();
        if (categorias is null)
            return NotFound("Categoria não encontrada");


        var categoriasDto =  categorias.ToCategoriaDTOList();

        return Ok(categoriasDto);

    }

    /// <summary>
    /// Obter uma Categoria pelo seu Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns> Objetos Categorias</returns>
    //[Authorize]
    [DisableCors]
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

    //[Authorize]
    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasPaginacao([FromQuery] CategoriasParameters categoriasParameters)
    {
        var categorias = await _repository.GetCategoriasAsync(categoriasParameters);
     

        //var categoriasDto = categorias.ToCategoriaDTOList();
        return ObterCategorias(categorias);
    }
    //[Authorize]
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

    /// <summary>
    /// Inclui uma nova categoria
    /// </summary>
    /// <remarks>
    /// Exemplo de request:
    /// POST api/categoria
    /// {
    /// "CategoriaId": 1,
    /// "Nome": "Bebidas",
    /// "ImagemUrl": "https://www.google.com/imagem/bebidas.png"
    /// }
    /// </remarks>
    /// <param name="categoriaDto"></param>
    /// <returns>Retorna um objeto Categorias incluído</returns>
    [Authorize]
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
    [Authorize]
    [HttpPut("id{id:int}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
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
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
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