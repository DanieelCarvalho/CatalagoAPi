using CatalogoApi.Context;
using CatalogoApi.Models;
using CatalogoApi.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApi.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
  
    private readonly IProdutoRepository _produtoRepository;

    public ProdutosController(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> Get()
    {
        var produtos = _produtoRepository.GetAll();

        if (produtos is null)
            return NotFound("Produtos não encontrados");

        return Ok(produtos);

    }

    [HttpGet("{id:int:min(1)}", Name="ObterProduto")]
    public ActionResult<Produto> Get(int id)
    {
        var produto = _produtoRepository.Get(p => p.ProdutoId == id);

        if (produto is null)
            return NotFound("Produto não encontrado");
        return Ok(produto);
    }

    [HttpGet("produtos/{id}")]
    public ActionResult<IEnumerable<Produto>> GetProdutosByCategoriaId(int id)
    {
        var produtos = _produtoRepository.GetProdutosByCategoriaId(id);
        if (produtos is null)
            return NotFound("Produtos não encontrados para a categoria informada");
        return Ok(produtos);
    }

    [HttpPost]
    public ActionResult Post(Produto produto)
    {
        if (produto is null)
            return BadRequest("Produto é nulo");
        var novoProduto = _produtoRepository.Create(produto);
        return new CreatedAtRouteResult("ObterProduto", new { id = novoProduto.ProdutoId }, novoProduto);
    }

    [HttpPut("id{id:int}")]
    public ActionResult Put(int id, Produto produto)
    {
        if (id != produto.ProdutoId)
            return BadRequest();

        var ProdutoAtualizado = _produtoRepository.Update(produto);



        return Ok(ProdutoAtualizado);

    }

    [HttpDelete("id{id:int}")]
    public ActionResult Delete(int id)
    {
        var produto = _produtoRepository.Get(p => p.ProdutoId == id);
        if (produto is null)
            return NotFound("Produto não encontrado");

        var deletado = _produtoRepository.Delete(produto);
      

        return Ok(deletado);
    }

}
