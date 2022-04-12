using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// Derives from a Controller as we gonna provide a view (index.html) from the API server
public class FallbackController : Controller
{
    public IActionResult Index()
    {
        return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/index.html"), "text/HTML");
    }
}