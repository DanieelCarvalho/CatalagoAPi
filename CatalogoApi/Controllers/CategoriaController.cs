using CatalogoApi.DTOs;
using CatalogoApi.DTOs.Mappings;
using CatalogoApi.Filters;
using CatalogoApi.Models;
using CatalogoApi.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;


namespace CatalogoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriaController  : ControllerBase
{
    private readonly IRepository<Categoria> _repository;
    private readonly ILogger<CategoriaController> _logger;

    public CategoriaController(IRepository<Categoria> repository,
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

    //[HttpGet("produtos")]
    //public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
    //{
    //    // return _context.Categorias.Include(p  => p.Produtos).AsNoTracking().ToList();
    //    return _context.Categorias.Include(p => p.Produtos)
    //                              .Where(c=> c.CategoriaId <=5)
    //                              .AsNoTracking()
    //                              .ToList();

    //}

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