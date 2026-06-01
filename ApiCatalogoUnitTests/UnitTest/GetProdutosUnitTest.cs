
using CatalogoApi.Controllers;
using CatalogoApi.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ApiCatalogoUnitTests.UnitTest;

public class GetProdutosUnitTest : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;
    public ILogger<ProdutosController> logger { get; private set; }

    public GetProdutosUnitTest(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(
            controller.repository,
            NullLogger<ProdutosController>.Instance
        );
    }

    [Fact]
    public async Task GetProdutoById_OKResult()
    {
        //Arrange
        var produtId = 3;

        //Action
        var data = await _controller.Get(produtId);

        //Assert(xunit)
        //var okResult = Assert.IsType<OkObjectResult>(data.Result);
        //Assert.Equal(200, okResult.StatusCode);

        //Assert (fluentassertions)

        data.Result.Should().BeOfType<OkObjectResult>() //Verifica se o resultado é do tipo OkObjectResult
            .Which.StatusCode.Should().Be(200); // verifica se o código de status é do OkObjectResult é 200
    }

  [Fact]
  public async Task GetProdutoById_NotFoundResult()
  {
      //Arrange
      var produtId = 999;


      //Action
      var data = await _controller.Get(produtId);
      //Assert(xunit)
      //var notFoundResult = Assert.IsType<NotFoundResult>(data.Result);
      //Assert.Equal(404, notFoundResult.StatusCode);


      //Assert (fluentassertions)
      data.Result.Should().BeOfType<NotFoundObjectResult>() //Verifica se o resultado é do tipo NotFoundResult
          .Which.StatusCode.Should().Be(404); // verifica se o código de status é do NotFoundResult é 404
    }

    [Fact]
    public async Task GetProdutoById_Return_BadRequest()
    {
        //Arrange
        var produtId = -1;

        //Action
        var data = await _controller.Get(produtId);

        //Assert(xunit)
        //var badRequestResult = Assert.IsType<BadRequestResult>(data.Result);
        //Assert.Equal(400, badRequestResult.StatusCode);
        //Assert (fluentassertions)
        data.Result.Should().BeOfType<BadRequestObjectResult>() //Verifica se o resultado é do tipo BadRequestResult
            .Which.StatusCode.Should().Be(400); // verifica se o código de status é do BadRequestResult é 400
    }

    [Fact]
    public async Task GetProdutos_Return_ListOfProdutoDTO()
    {
        //Action
        var data = await _controller.Get();

        //Assert(xunit)
        //var okResult = Assert.IsType<OkObjectResult>(data.Result);
        //Assert.Equal(200, okResult.StatusCode);
        
        //Assert.NotEmpty(produtos);
        //Assert (fluentassertions)
        data.Result.Should().BeOfType<OkObjectResult>() //Verifica se o resultado é do tipo OkObjectResult
            .Which.Value.Should().BeAssignableTo<IEnumerable<ProdutoDTOResponse>>() // verifica se o valor do OKObjectResult é atribuével a IEnumerable<ProdutoDTOResponse>
            .And.NotBeNull(); // Verifica se a lista de produtos não é nula

    }

    [Fact]

    public async Task GetProdutos_Return_BadRequest()
    {
        //Action
        var data = await _controller.Get();

        //Assert(xunit)
        data.Result.Should().BeOfType<BadRequestResult>(); // Verifica se o resultado é do tipo BadRequest
    }



}
