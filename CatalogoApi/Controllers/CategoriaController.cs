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
using Microsoft.Extensions.Caching.Memory;

namespace CatalogoApi.Controllers;

[EnableCors("OrigensComAcessoPermitido")]
[Route("[controller]")]
[ApiController]
[Authorize]


[ApiConventionType(typeof(DefaultApiConventions))]

//[EnableRateLimiting("Fixedwindow")]
public class CategoriaController  : ControllerBase
{
    private readonly ICategoriaRepository _repository;
    private readonly ILogger<CategoriaController> _logger;
    private readonly IMemoryCache _cache;
    private const string CacheCategoriasKey = "CacheCategorias";


    public CategoriaController(ICategoriaRepository repository,
                               ILogger<CategoriaController> logger,
                               IMemoryCache cache)
    {
        _repository = repository;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// Obtem uma lista de onjetos Categorias
    /// </summary>
    /// <returns>
    /// Uma lista de objetos Categoria
    /// </returns>
    /// 
    [HttpGet]
    [DisableRateLimiting]
    [ServiceFilter(typeof(ApiLogginFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
    {

        if(!_cache.TryGetValue(CacheCategoriasKey, out IEnumerable<CategoriaDTO>? categoriasDto))
        {

            _logger.LogInformation("===============GET api/categorias/ produtos=================");
            var categorias = await _repository.GetAllAsync();

            //Executa este bloco SE categorias for nulo OU
            //SE categorias não tiver nenhum elemento

            if (categorias is not null && categorias.Any())
            {

                categoriasDto = categorias.ToCategoriaDTOList();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
                    SlidingExpiration = TimeSpan.FromSeconds(15),
                    Priority = CacheItemPriority.High
                };
                _cache.Set(CacheCategoriasKey, categoriasDto, cacheOptions);
            }
            else
            {
                _logger.LogWarning("Não existem categorias");
                return NotFound("Não existem categorias");
            }


            

        }

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
        if(id == null || id <= 0)
        {
            _logger.LogWarning("Id inválido...");
            return BadRequest("Id de categoria inválido");
        }

        var cacheCategoriaKey = $"Categoria_{id}";

        if(!_cache.TryGetValue(cacheCategoriaKey, out CategoriaDTO? categoriaDto))
        {
           var categoria = await _repository.GetAsync(c => c.CategoriaId == id);

            if (categoria is not null)
            {
               categoriaDto = categoria.ToCategoriaDTO();
           
               var cacheOptions = new MemoryCacheEntryOptions
               {
                   AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
                   SlidingExpiration = TimeSpan.FromSeconds(15),
                   Priority = CacheItemPriority.High
               };

                _cache.Set(cacheCategoriaKey, categoriaDto, cacheOptions);


            }
            else
            {
                _logger.LogWarning($"Categoria com id {id} não encontrada...");
                return NotFound("Categoria não encontrado");
            }

        }

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

        _cache.Remove(CacheCategoriasKey);

        var cacheKey = $"Categoria_{cartegoriaCriada.CategoriaId}";

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
            SlidingExpiration = TimeSpan.FromSeconds(15),
            Priority = CacheItemPriority.High
        };

        _cache.Set(cacheKey, cartegoriaCriada.ToCategoriaDTO(), cacheOptions);

        var novaCategoriaDto = cartegoriaCriada.ToCategoriaDTO();

        return new CreatedAtRouteResult("ObterCategoria", 
                                        new { id = novaCategoriaDto.CategoriaId }, 
                                        categoria);
    }

    [HttpPut("id{id:int}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
    {
        if (id != categoriaDto.CategoriaId || id <= 0)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

        if(categoriaDto is null)
        {
            _logger.LogWarning("Categoria com id {id} não encontrado", id);
            return NotFound("Categoria não encontrada");
        }
        var categoria = categoriaDto.ToCategoria();

        var categoriaAtualizada= await _repository.UpdateAsync(categoria);

        _cache.Set($"CacheCategoria_{id}", categoriaAtualizada, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
            SlidingExpiration = TimeSpan.FromSeconds(15),
            Priority = CacheItemPriority.High
        });

        _cache.Remove(CacheCategoriasKey);

        var categoriaAtualizadaDto = categoriaAtualizada.ToCategoriaDTO();

        return Ok(categoriaAtualizadaDto);

    }
    [HttpDelete("id{id:int}")]
    //[Authorize(Policy = "AdminOnly")]
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

        _cache.Remove($"CacheCategoria_{id}");
        _cache.Remove(CacheCategoriasKey);

        var categoriaExcluidaDto =  categoraiExcluida.ToCategoriaDTO();

        return Ok(categoriaExcluidaDto);
    }

}