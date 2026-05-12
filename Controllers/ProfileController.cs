namespace CMVC.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/profile")]
public class ProfileController:ControllerBase
{
    //[Authorize]
    [HttpGet]
    public IActionResult Profile()
    {
        return Ok("You are logged in");
    }
}
