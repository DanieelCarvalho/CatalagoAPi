
using CatalogoApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoUnitTests.UnitTest;

public class GetProdutosImitTest : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;

    public GetProdutosImitTest(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository, controller.logger);
    }

    [Fact]
    public async Task GetProdutoById_OKResult()
    {
        //Arrange
        var produtId = 2;

        //Action
        var data = await _controller.Get(produtId);

        //Assert(xunit)
        //var okResult = Assert.IsType<OkObjectResult>(data.Result);
        //Assert.Equal(200, okResult.StatusCode);

        //Assert (fluentassertions)

        data.Result.Should().BeOfType<OkObjectResult>() //Verifica se o resultado é do tipo OkObjectResult
            .Which.StatusCode.Should().Be(200); // verifica se o código de status é do OkObjectResult é 200
    }
}
