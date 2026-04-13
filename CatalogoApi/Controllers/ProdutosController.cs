using Azure;
using CatalogoApi.Context;
using CatalogoApi.DTOs;
using CatalogoApi.DTOs.Mappings;
using CatalogoApi.Models;
using CatalogoApi.Pagination;
using CatalogoApi.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using X.PagedList;

namespace CatalogoApi.Controllers;


[Route("api/[controller]")]
[ApiController]
[ApiConventionType(typeof(DefaultApiConventions))]
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

    /// <summary>
    /// Exibe uma relação de produtos 
    /// </summary>
    /// <returns>
    /// Retorna uma lista de objetos Produto
    /// </returns>
    [HttpGet]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<IEnumerable<ProdutoDTOResponse>>> Get()
    {
        var produtos = await _produtoRepository.GetAllAsync();

        if (produtos is null)
            return NotFound("Produtos não encontrados");

        var produtosDto = produtos.ToProdutoDTOResponseList();

        return Ok(produtosDto);

    }
    /// <summary>
    /// Obtem o produto pelo id
    /// </summary>
    /// <param name="id">Código do produto</param>
    /// <returns>Um objeto Produdo</returns>
    [Authorize]
    [HttpGet("{id}", Name="ObterProduto")]
    public async Task<ActionResult<ProdutoDTOResponse>> Get(int id)
    {
        var produto = await _produtoRepository.GetAsync(p => p.ProdutoId == id);

        if (produto is null){
            _logger.LogWarning("Produto com id {id} não encontrado", id);
            return NotFound("Produto não encontrado"); 
        }

        var produtoDto = produto.ToProdutoDTOResponse();
        return Ok(produtoDto);
    }
    [Authorize]
    [HttpGet("categoria/{categoriaId}")]
    public async Task<ActionResult<IEnumerable<ProdutoDTOResponse>>> GetProdutosByCategoriaId([FromRoute]  int categoriaId)
    {
        var produtos = await _produtoRepository.GetProdutosByCategoriaIdAsync(categoriaId);

        if (produtos is null)
            return NotFound("Produtos não encontrados para a categoria informada");

        var produtosDto = produtos.ToProdutoDTOResponseList(); 
        return Ok(produtosDto);
    }
    [Authorize]
    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTOResponse>>> Get([FromQuery] ProdutosParameters produtosParams)
    {
        var produtos = await _produtoRepository.GetProdutosAsync(produtosParams);
        if (produtos is null || !produtos.Any())
            return NotFound("Produtos não encontrados");

        return ObterProdutos(produtos);
    }
    [Authorize]
    [HttpGet("filter/preco/pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTOResponse>>> GetProdutosFilterPreco([FromQuery] ProdutosFiltroPreco produtosFiltroPreco)
    {
        var produtos = await _produtoRepository.GetProdutosFiltroPrecoAsync(produtosFiltroPreco);
        return ObterProdutos(produtos);
    }

    private ActionResult<IEnumerable<ProdutoDTOResponse>> ObterProdutos(IPagedList<Produto> produtos)
    {
        var metadata = new
        {
            produtos.Count,
            produtos.PageSize,
            produtos.PageCount,
            produtos.TotalItemCount,
            produtos.HasNextPage,
            produtos.HasPreviousPage
        };
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

        var produtosDto = produtos.ToProdutoDTOResponseList();
        return Ok(produtosDto);
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProdutoDTOCreated>> Post(ProdutoDTOCreated produtoDto)
    {
        if (produtoDto is null)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos...");
        }


        var produto = produtoDto.ToProduto();

        var novoProduto = await _produtoRepository.CreateAsync(produto);

        var novoProdutoDto = novoProduto.ToProdutoDTOCreated();
        return new CreatedAtRouteResult("ObterProduto", new { id = novoProdutoDto.ProdutoId }, novoProduto);
    }
    [Authorize]
    [HttpPut("id{id:int}")]
    public async Task<ActionResult<ProdutoDTOResponse>> Put(int id, ProdutoDTOCreated produtoDto)
    {
        if (id != produtoDto.ProdutoId)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

        var produto = produtoDto.ToProduto();

        var produtoAtualizado = await _produtoRepository.UpdateAsync(produto);

        var produtoAtualizadoDto = produtoAtualizado.ToProdutoDTOResponse();

        return Ok(produtoAtualizadoDto);

    }
    [Authorize]
    [HttpPatch("{id}/UpdatePartial")]
    public async Task<ActionResult<ProdutoDTOUpdateResponse>> Patch(int id, 
           JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
    {
        if (patchProdutoDTO is null || id <=0)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos");
        }
        var produto = await _produtoRepository.GetAsync(p => p.ProdutoId == id);

        if (produto is null)
        {
            _logger.LogWarning("Produto com id {id} não encontrado", id);
            return NotFound("Produto não encontrado");
        }

        var produtoUpdateRequest = produto.ToProdutoUpdateRequest();
        patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);

        if (!ModelState.IsValid || !TryValidateModel(produtoUpdateRequest))
        {
            return BadRequest(ModelState);
        }

      
        produto.UpdateFromRequest(produtoUpdateRequest);

       
        var produtoAtualizado = await _produtoRepository.UpdateAsync(produto);

       
        var response = produtoAtualizado.ToProdutoUpdateResponseDTO();

        return Ok(response);
    }
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ProdutoDTOResponse>> Delete(int id)
    {
        var produto = await _produtoRepository.GetAsync(p => p.ProdutoId == id);
        if (produto is null)
        {
            _logger.LogWarning("Produto com id {id} não encontrado", id);
            return NotFound("Produto não encontrado");
        }

        var produtoExcluido =await _produtoRepository.DeleteAsync(produto);

        var categoriaExcluidaDto = produtoExcluido.ToProdutoDTOResponse();


        return Ok(categoriaExcluidaDto);
    }

}
