using API.Errors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    
    [Route("errors/{code}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : BaseApiController
    {
        // Handling resources that does not exist (mistakes in the URL)
        public IActionResult Error(int code)
        {
            return new ObjectResult(new ApiResponse(code));
        }
    }
}