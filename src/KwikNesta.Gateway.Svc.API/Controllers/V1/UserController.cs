using Microsoft.AspNetCore.Mvc;

namespace KwikNesta.Gateway.Svc.API.Controllers.V1
{
    [Route("api/v{version:apiversion}/users")]
    [ApiVersion("1.0")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Pong");
        }
    }
}