using CatalogoApi.Context;
using CatalogoApi.Repositories;
using CatalogoApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;


namespace ApiCatalogoUnitTests.UnitTest;

public class ProdutosUnitTestController
{
    public IProdutoRepository repository;
    public static DbContextOptions<AppDbContext> dbContextOptions { get; }

    public static string connectionString =
        "Server=localhost;DataBase=CatalogoDB;Uid=root;Pwd=admin123";

    static ProdutosUnitTestController()
    {
        dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
    }

    public ProdutosUnitTestController()
    {

        var context = new AppDbContext(dbContextOptions);
        repository = new ProdutoRepository(context);
    }
}