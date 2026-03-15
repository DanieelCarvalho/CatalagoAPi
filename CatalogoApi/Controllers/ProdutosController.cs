using CatalogoApi.Context;
using CatalogoApi.DTOs;
using CatalogoApi.DTOs.Mappings;
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
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(IProdutoRepository produtoRepository, 
                              ILogger<ProdutosController> logger)
    {
        _produtoRepository = produtoRepository;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> Get()
    {
        var produtos = _produtoRepository.GetAll();

        if (produtos is null)
            return NotFound("Produtos não encontrados");

        var produtosDto = produtos.ToProdutosDTOList();

        return Ok(produtosDto);

    }

    [HttpGet("{id:int:min(1)}", Name="ObterProduto")]
    public ActionResult<Produto> Get(int id)
    {
        var produto = _produtoRepository.Get(p => p.ProdutoId == id);

        if (produto is null){
            _logger.LogWarning("Produto com id {id} não encontrado", id);
            return NotFound("Produto não encontrado"); 
        }

        var produtoDto = produto.ToProdutosDTO();
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
    public ActionResult Post(ProdutoDTO produtoDto)
    {
        if (produtoDto is null)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos...");
        }


        var produto = produtoDto.ToProdutos();

        var novoProduto = _produtoRepository.Create(produto);

        var novoProdutoDto = novoProduto.ToProdutosDTO();
        return new CreatedAtRouteResult("ObterProduto", new { id = novoProdutoDto.ProdutoId }, novoProduto);
    }

    [HttpPut("id{id:int}")]
    public ActionResult Put(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

        var produto = produtoDto.ToProdutos();

        var produtoAtualizado = _produtoRepository.Update(produto);

        var produtoAtualizadoDto = produtoAtualizado.ToProdutosDTO();

        return Ok(produtoAtualizadoDto);

    }

    [HttpDelete("id{id:int}")]
    public ActionResult<ProdutoDTO> Delete(int id)
    {
        var produto = _produtoRepository.Get(p => p.ProdutoId == id);
        if (produto is null)
        {
            _logger.LogWarning("Produto com id {id} não encontrado", id);
            return NotFound("Produto não encontrado");
        }

        var produtoExcluido = _produtoRepository.Delete(produto);

        var categoriaExcluidaDto = produtoExcluido.ToProdutosDTO();


        return Ok(categoriaExcluidaDto);
    }

}
