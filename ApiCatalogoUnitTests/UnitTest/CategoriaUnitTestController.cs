
using CatalogoApi.Context;
using CatalogoApi.Controllers;
using CatalogoApi.Repositories;
using CatalogoApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiCatalogoUnitTests.UnitTest;

public class CategoriaUnitTestController
{
    public ICategoriaRepository repository;

    public ILogger<CategoriaController> logger;

    public static DbContextOptions<AppDbContext> dbContextOptions { get; }

    public static string connectionString =
       "Server=localhost;DataBase=CatalogoDB;Uid=root;Pwd=admin123";

    static CategoriaUnitTestController()
    {
        dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
    }

    public CategoriaUnitTestController()
    {
        var context = new AppDbContext(dbContextOptions);
        repository = new CategoriaRepository(context);
    }


}
