using CatalogoApi.Context;
using CatalogoApi.Filters;
using CatalogoApi.Models;
using CatalogoApi.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
    public ActionResult<IEnumerable<Categoria>> Get()
    {

        _logger.LogInformation("===============GET api/categorias/ produtos=================");
        var categoria = _repository.GetAll();
        if (categoria is null)
            return NotFound("Categoria não encontrada");

        return Ok(categoria);

    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public ActionResult<Categoria> Get(int id)
    {

        var categoria = _repository.Get(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id {id} não encontrada...");
            return NotFound("Categoria não encontrado");
        }

        return Ok(categoria);
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
    public ActionResult Post(Categoria categoria)
    {
        if (categoria is null)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos");
        }
        var cartegoriaCriada = _repository.Create(categoria);

        return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria);
    }

    [HttpPut("id{id:int}")]
    public ActionResult Put(int id, Categoria categoria)
    {
        if (id != categoria.CategoriaId)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

        _repository.Update(categoria);

        return Ok(categoria);

    }

    [HttpDelete("id{id:int}")]
    public ActionResult Delete(int id)
    {
        var categoria = _repository.Get(c => c.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id {id} não encontrada...");
            return NotFound("Categoria não encontrada");
        }
       var categoraiExcluida = _repository.Delete(categoria);
        return Ok(categoraiExcluida);
    }

}