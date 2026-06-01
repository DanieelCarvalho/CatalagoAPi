

using CatalogoApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ApiCatalogoUnitTests.UnitTest;

public class PostCategoriasUnitTest : IClassFixture<CategoriaUnitTestController>
{
    private readonly CategoriaController _controller;


    public ILogger<PostCategoriasUnitTest> _logger { get; private set; }
    public PostCategoriasUnitTest(CategoriaUnitTestController controller)
    {
        _controller = new CategoriaController(controller.repository,
            NullLogger<CategoriaController>.Instance);

    }

    [Fact]

    public async Task PostCategoria_Return_CreatedStatusCode()
    {
        //Arrange
        var novaCategoriaDto = new CatalogoApi.DTOs.CategoriaDTO
        {
            Nome = "Nova Categoria",
            ImagemUrl = "imagem.jpg"
        };
        //Act
        var data = await _controller.Post(novaCategoriaDto);

        //Assert
        var createdResult = data.Result.Should().BeOfType<CreatedAtRouteResult>();
        createdResult.Subject.StatusCode.Should().Be(201);
    }

    [Fact]

    public async Task PostCategoria_Return_BadRequest()
    {
        //Arrange
        CatalogoApi.DTOs.CategoriaDTO novaCategoriaDto = null;
        //Act
        var data = await _controller.Post(novaCategoriaDto);
        //Assert
        var badRequestResult = data.Result.Should().BeOfType<BadRequestObjectResult>();
        badRequestResult.Subject.StatusCode.Should().Be(400);
    }
}
