using HotlineKatalog.Models.RequestModels;
using HotlineKatalog.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotlineKatalog.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class HotlineKatalogController : Controller
    {
        private readonly IHotlineKatalogService _hotlineKatalogService;

        public HotlineKatalogController(IHotlineKatalogService hotlineKatalogService)
        {
            _hotlineKatalogService = hotlineKatalogService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GoodsRequestModel model)
        {
            var response = await _hotlineKatalogService.GetGoods(model);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var response = await _hotlineKatalogService.GetGood(id);

            return Ok(response);
        }
    }
}
