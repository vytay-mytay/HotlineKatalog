using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hakaya.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public async Task<IActionResult> Index()
        {
            return Redirect("swagger");
        }

        [HttpGet("WebSocketInfo")]
        public IActionResult WebSocketInfo()
        {
            return View();
        }
    }
}
