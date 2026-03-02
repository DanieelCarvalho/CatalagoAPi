using CatalogoApi.Context;
using Microsoft.AspNetCore.Mvc;


namespace CatalogoApi.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriaController  : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriaController(AppDbContext context)
    {
        _context = context;
    }


}