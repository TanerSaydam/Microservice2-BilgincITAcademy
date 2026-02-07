using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.CategoryWebAPI.Controllers;

[ApiController]
//[ApiVersion(1, Deprecated = true)]
//[ApiVersion(2)]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public class ValuesController : ControllerBase //api-version=1
{
    [HttpGet]
    //[MyAuthorize("getall")]
    //[MapToApiVersion(1)]
    [ApiVersion(1, Deprecated = true)]
    public IActionResult Get1()
    {
        Console.WriteLine("Hello world");
        return Ok(new { Message = "Version 1" });
    }

    [HttpGet]
    //[MapToApiVersion(2)]
    [ApiVersion(2)]
    public IActionResult Get2()
    {
        return Ok(new { Message = "Version 1" });
    }
}
