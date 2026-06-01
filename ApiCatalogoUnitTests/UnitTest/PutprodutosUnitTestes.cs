using CatalogoApi.Controllers;
using CatalogoApi.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;


namespace ApiCatalogoUnitTests.UnitTest;

public class PutprodutosUnitTestes : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;
    public ILogger<ProdutosController> logger { get; private set; }
    public PutprodutosUnitTestes(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository,
            NullLogger<ProdutosController>.Instance);
    }


    // testes de uniade para Put

    [Fact]
    public async  Task PutProduto_Return_OkResult()
    {

        //Arrange
        var prodId = 3;

        var produtoDtoUpdated = new ProdutoDTOCreated
        {
            ProdutoId = prodId,
            Nome = "Produto Atualizado",
            Preco= 22.5m,
            Descricao = "Descrição atualizada",
            ImagemUrl = "imagem_atualizada.jpg",
            CategoriaId = 2
        };
        //Act
        var result = await _controller.Put(prodId, produtoDtoUpdated);

        //Assert
       
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
       
    }

    [Fact]
    public async Task PutProduto_Return_BadRequest()
    {
        //Arrange
        var prodId = 1000;

        var produtoDtoUpdated = new ProdutoDTOCreated
        {
            ProdutoId = 14,
            Nome = "Produto Atualizado",
            Preco = 22.5m,
            Descricao = "Descrição atualizada",
            ImagemUrl = "imagem_atualizada.jpg",
            CategoriaId = 2
        };
        //Act
        var data = await _controller.Put(prodId, produtoDtoUpdated);
        //Assert
     
        data.Result.Should().BeOfType<BadRequestObjectResult>().Which.StatusCode.Should().Be(400);
    }
}
