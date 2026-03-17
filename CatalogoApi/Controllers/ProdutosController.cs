using Azure;
using CatalogoApi.Context;
using CatalogoApi.DTOs;
using CatalogoApi.DTOs.Mappings;
using CatalogoApi.Models;
using CatalogoApi.Pagination;
using CatalogoApi.Repositories.Interface;
using Microsoft.AspNetCore.JsonPatch;
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
    public ActionResult<IEnumerable<ProdutoDTOResponse>> Get()
    {
        var produtos = _produtoRepository.GetAll();

        if (produtos is null)
            return NotFound("Produtos não encontrados");

        var produtosDto = produtos.ToProdutoDTOResponseList();

        return Ok(produtosDto);

    }

    [HttpGet("{id}", Name="ObterProduto")]
    public ActionResult<ProdutoDTOResponse> Get(int id)
    {
        var produto = _produtoRepository.Get(p => p.ProdutoId == id);

        if (produto is null){
            _logger.LogWarning("Produto com id {id} não encontrado", id);
            return NotFound("Produto não encontrado"); 
        }

        var produtoDto = produto.ToProdutoDTOResponse();
        return Ok(produtoDto);
    }

    [HttpGet("categoria/{categoriaId}")]
    public ActionResult<IEnumerable<ProdutoDTOResponse>> GetProdutosByCategoriaId([FromRoute]  int categoriaId)
    {
        var produtos = _produtoRepository.GetProdutosByCategoriaId(categoriaId);

        if (produtos is null)
            return NotFound("Produtos não encontrados para a categoria informada");

        var produtosDto = produtos.ToProdutoDTOResponseList(); 
        return Ok(produtosDto);
    }

    [HttpGet("pagination")]
    public ActionResult<IEnumerable<ProdutoDTOResponse>> Get([FromQuery] ProdutosParameters produtosParams)
    {
        var produtos = _produtoRepository.GetProdutos(produtosParams);
        if (produtos is null || !produtos.Any())
            return NotFound("Produtos não encontrados");
        var produtosDto = produtos.ToProdutoDTOResponseList();
        return Ok(produtosDto);
    }

    [HttpPost]
    public ActionResult<ProdutoDTOCreated> Post(ProdutoDTOCreated produtoDto)
    {
        if (produtoDto is null)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos...");
        }


        var produto = produtoDto.ToProduto();

        var novoProduto = _produtoRepository.Create(produto);

        var novoProdutoDto = novoProduto.ToProdutoDTOCreated();
        return new CreatedAtRouteResult("ObterProduto", new { id = novoProdutoDto.ProdutoId }, novoProduto);
    }

    //[HttpPut("id{id:int}")]
    //public ActionResult<ProdutoDTOResponse> Put(int id, ProdutoDTOCreated produtoDto)
    //{
    //    if (id != produtoDto.ProdutoId)
    //    {
    //        _logger.LogWarning("Dados inválidos...");
    //        return BadRequest("Dados inválidos");
    //    }

    //    var produto = produtoDto.ToProduto();

    //    var produtoAtualizado = _produtoRepository.Update(produto);

    //    var produtoAtualizadoDto = produtoAtualizado.ToProdutoDTOResponse();

    //    return Ok(produtoAtualizadoDto);

    //}

    [HttpPatch("{id}/UpdatePartial")]
    public ActionResult<ProdutoDTOUpdateResponse> Patch(int id, 
           JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
    {
        if (patchProdutoDTO is null || id <=0)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos");
        }
        var produto = _produtoRepository.Get(p => p.ProdutoId == id);

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

       
        var produtoAtualizado = _produtoRepository.Update(produto);

       
        var response = produtoAtualizado.ToProdutoUpdateResponseDTO();

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public ActionResult<ProdutoDTOResponse> Delete(int id)
    {
        var produto = _produtoRepository.Get(p => p.ProdutoId == id);
        if (produto is null)
        {
            _logger.LogWarning("Produto com id {id} não encontrado", id);
            return NotFound("Produto não encontrado");
        }

        var produtoExcluido = _produtoRepository.Delete(produto);

        var categoriaExcluidaDto = produtoExcluido.ToProdutoDTOResponse();


        return Ok(categoriaExcluidaDto);
    }

}
