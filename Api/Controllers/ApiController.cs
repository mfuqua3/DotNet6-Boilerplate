using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Area("api")]
[Route("[area]/[controller]")]
[Route("[area]/v{version:apiVersion}/[controller]")]
[Authorize]
public abstract class ApiController : Controller
{
    
}