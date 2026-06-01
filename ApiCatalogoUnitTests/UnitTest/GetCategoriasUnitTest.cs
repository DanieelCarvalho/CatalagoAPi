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

public  class GetCategoriasUnitTest : IClassFixture<CategoriaUnitTestController>
{

    private readonly CategoriaController _controller;
    public ILogger<CategoriaController> logger { get; private set; }

    public GetCategoriasUnitTest(CategoriaUnitTestController controller)
    {
        _controller = new CategoriaController(
            controller.repository,
            NullLogger<CategoriaController>.Instance
        );
    }


    [Fact]
    public async Task GetCategoriaById_OkResult()
    {
        //Arrange
        var categoriaId = 2;

        //Action
        var data = await _controller.Get(categoriaId);

        data.Result.Should().BeOfType<OkObjectResult>() //Verifica se o resultado é do tipo OkObjectResult
            .Which.StatusCode.Should().Be(200); // verifica se o código de status é do OkObjectResult é 200
    }

    [Fact]

    public async Task GetCategoriaById_NotFoundResult()
    {
        //Arrange
        var categoriaId = 999;
        //Action
        var data = await _controller.Get(categoriaId);
        data.Result.Should().BeOfType<NotFoundObjectResult>() //Verifica se o resultado é do tipo NotFoundResult
            .Which.StatusCode.Should().Be(404); // verifica se o código de status é do NotFoundResult é 404
    }


    [Fact]
    public async Task GetCategoriaById_Return_BadRequest()
    {
        //Arrange
        int categoriaId = -1;
        //Action
        var data = await _controller.Get(categoriaId);
        data.Result.Should().BeOfType<BadRequestObjectResult>() //Verifica se o resultado é do tipo BadRequestResult
            .Which.StatusCode.Should().Be(400); // verifica se o código de status é do BadRequestResult é 400
    }

    [Fact]
     public async Task GetCategorias_Return_ListOfCategoriaDTO()
     {
         //Arrange
         //Action
         var data = await _controller.Get();
         //Assert(xunit)

        data.Result.Should().BeOfType<OkObjectResult>() //Verifica se o resultado é do tipo OkObjectResult
            .Which.Value.Should().BeAssignableTo<IEnumerable<CategoriaDTO>>()
            .And.NotBeNull(); // verifica se o código de status é do OkObjectResult é 200

    }

    [Fact]
    public async Task GetCategorias_return_BadRequest()
    {
        //Arrange
        //Action
        var data = await _controller.Get();
        //Assert(xunit)
        data.Result.Should().BeOfType<BadRequestResult>(); //Verifica se o resultado é do tipo BadRequestResult
            //.Which.StatusCode.Should().Be(400); // verifica se o código de status é do BadRequestResult é 400
    }


}
