using Api.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class CoffeeController : ApiController
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult BrewCoffee()
    {
        throw new ServerIsTeapotException();
    }
    
}