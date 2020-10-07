using HotlineKatalog.Models.RequestModels;
using HotlineKatalog.Models.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotlineKatalog.Services.Interfaces
{
    public interface IHotlineKatalogService
    {
        Task<List<GoodResponseModel>> GetGoods(GoodsRequestModel model);

        Task<GoodResponseModel> GetGood(int id);
    }
}
