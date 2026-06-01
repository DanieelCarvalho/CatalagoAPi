using CatalogoApi.Controllers;
using CatalogoApi.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogoUnitTests.UnitTest;

public class PostProdutosUnitTests : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;
    public ILogger<ProdutosController> logger { get; private set; }


    public PostProdutosUnitTests(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository,
            NullLogger<ProdutosController>.Instance);
    }


    // metodos de teste para post 
    [Fact]

    public async Task PostProduto_Return_CreatedStatusCode()
    {
        //Arrange
        var novoProdutoDto = new ProdutoDTOCreated
        {
            Nome = "Novo Peoduto",
            Descricao = "desc",
            Preco = 10.99m,
            ImagemUrl= "imagem.jpg",
            CategoriaId = 2

        };
        //Act
        var data = await _controller.Post(novoProdutoDto);

        //Assert

        var createdResult = data.Result.Should().BeOfType<CreatedAtRouteResult>();
        createdResult.Subject.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task PostProduto_Return_BadRequest()
    {
        //Arrange
        ProdutoDTOCreated novoProdutoDto = null;
        //Act
        var data = await _controller.Post(novoProdutoDto);
        //Assert
        var badRequestResult = data.Result.Should().BeOfType<BadRequestObjectResult>();
        badRequestResult.Subject.StatusCode.Should().Be(400);
    }

}
