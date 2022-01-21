using API.Errors;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly StoreContext _context;

        public BuggyController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet("testauth")]
        [Authorize]
        public ActionResult<string> GetSecretText()
        {
            return "Secret stuff";
        }
        
        [HttpGet("notfound")]
        public ActionResult GetNotFoundResult()
        {
            // Generates HTTP 400 series or error
            // The item with ID 42 does not exist in a database
            var thing = _context.Products.Find(42);
            if (thing == null)
            {
                return NotFound(new ApiResponse(404));
            }
            
            return Ok();
        }
        
        [HttpGet("servererror")]
        public ActionResult GetServerError()
        {
            // Generates HTTP 500 series or error
            // ToString operation for the value that doesn't exist will result in a server error
            var thing = _context.Products.Find(42);
            var thingToReturn = thing.ToString();

            return Ok();
        }
        
        [HttpGet("badrequest")]
        public ActionResult GetBadRequest()
        {
            // Generates HTTP 300 series or error
            return BadRequest(new ApiResponse(400));
        }
        
        [HttpGet("badrequest/{id}")]
        public ActionResult GetNotFoundResult(int id)
        {
            return Ok();
        }
    }
}