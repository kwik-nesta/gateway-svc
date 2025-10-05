using Microsoft.AspNetCore.Mvc;
using Refit;
using System.Text.Json;

namespace KwikNesta.Gateway.Svc.API.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult FromApiResponse<T>(ApiResponse<T> response)
        {
            if (response.IsSuccessStatusCode)
                return Ok(response.Content);

            var json = response.Error?.Content ?? "{}";
            var error = JsonSerializer.Deserialize<object>(json);
            return StatusCode((int)response.StatusCode, error);
        }

    }
}
