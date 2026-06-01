using CatalogoApi.Controllers;
using CatalogoApi.Controllers;
using CatalogoApi.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ApiCatalogoUnitTests.UnitTest;

public class PutCategoriasUnitTestes : IClassFixture<CategoriaUnitTestController>
{
    private readonly CategoriaController _controller;

    public ILogger<CategoriaController> logger { get; private set; }

    public PutCategoriasUnitTestes(CategoriaUnitTestController controller)
    {
        _controller = new CategoriaController(
            controller.repository,
            NullLogger<CategoriaController>.Instance
        );
    }
     [Fact]
     public async Task PutCategoria_Return_OkResult()
    {
        //Arrange
        var categoriaId = 2;
        var categoriaDtoUpdated = new CatalogoApi.DTOs.CategoriaDTO
        {
            CategoriaId = categoriaId,
            Nome = "Categoria Atualizada",
            ImagemUrl = "www.jpg"
        };

        //Act
        var result = await _controller.Put(categoriaId, categoriaDtoUpdated);

        //Assert
        result.Should().NotBeNull();
       
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]

    public async Task PutCategoria_Return_BadRequest()
    {
        //Arrange
        var categoriaId = 1000;
        var categoriaDtoUpdated = new CatalogoApi.DTOs.CategoriaDTO
        {
            CategoriaId = 10,
            Nome = "Categoria Atualizada",
            ImagemUrl = "www.jpg"
        };
        //Act
        var data = await _controller.Put(categoriaId, categoriaDtoUpdated);
        //Assert

        data.Result.Should().BeOfType<BadRequestObjectResult>().Which.StatusCode.Should().Be(400);
    }





}
