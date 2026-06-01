
using CatalogoApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace ApiCatalogoUnitTests.UnitTest;

public  class DeleteCategoriaUnitTest : IClassFixture<CategoriaUnitTestController>
{
    private readonly CategoriaController _controller;

    public DeleteCategoriaUnitTest(CategoriaUnitTestController controller)
    {
        _controller = new CategoriaController(
            controller.repository,
            NullLogger<CategoriaController>.Instance
        );
    }


     [Fact]

     public async Task DeleteCategoria_Return_OkResult()
    {
        //Arrange
        var categoriaId = 3;
        //Act
        var result = await _controller.Delete(categoriaId);
        //Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]

    public async Task DeleteCategoria_Return_NotFoundResult()
    {
        //Arrange
        var categoriaId = 1000;
        //Act
        var result = await _controller.Delete(categoriaId);
        //Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }


}
