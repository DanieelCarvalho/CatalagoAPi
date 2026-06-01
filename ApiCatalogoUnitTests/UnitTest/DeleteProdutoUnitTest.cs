using CatalogoApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogoUnitTests.UnitTest;

public class DeleteProdutoUnitTest : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;

    public DeleteProdutoUnitTest(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository,
             NullLogger<ProdutosController>.Instance);
    }

    [Fact]
    public async Task DeleteProduto_Return_OkResult()
    {
        //Arrange
        var prodId = 3;
        //Act
        var result = await _controller.Delete(prodId);
        //Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteProduto_Return_NotFoundResult()
    {
        //Arrange
        var prodId = 1000;
        //Act
        var result = await _controller.Delete(prodId);
        //Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}
