using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotlineKatalog.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class HotlineKatalogController : Controller
    {

        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Test()
        {
            return Ok();
        }
    }
}
