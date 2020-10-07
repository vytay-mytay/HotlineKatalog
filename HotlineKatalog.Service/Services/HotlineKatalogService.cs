using AutoMapper;
using HotlineKatalog.DAL.Abstract;
using HotlineKatalog.Domain.Entities;
using HotlineKatalog.Models.Enums;
using HotlineKatalog.Models.RequestModels;
using HotlineKatalog.Models.ResponseModels;
using HotlineKatalog.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace HotlineKatalog.Services.Services
{
    public class HotlineKatalogService : IHotlineKatalogService
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IMapper _mapper;

        public HotlineKatalogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GoodResponseModel>> GetGoods(GoodsRequestModel model)
        {
            var allGoods = _unitOfWork.Repository<Good>().Get(g => (model.CategoryId == null
                                                                    || model.CategoryId.Value == g.CategoryId)
                                                                && (model.ShopId == null
                                                                    || g.Shops.Select(s => s.ShopId).Contains(model.ShopId.Value)))
                                                            .Select(x => new
                                                            {
                                                                x.Id,
                                                                x.CategoryId,
                                                                x.ProducerId,
                                                                LastPrice = x.Prices.OrderByDescending(c => c.Date).FirstOrDefault().Value
                                                            });

            var isAsc = model.Ordering == OrderingType.Asc;

            var orderedGoods = model.Filter switch
            {
                FilterType.Producers => isAsc ? allGoods.OrderBy(x => x.ProducerId) : allGoods.OrderByDescending(x => x.ProducerId),
                FilterType.Price => isAsc ? allGoods.OrderBy(x => x.LastPrice) : allGoods.OrderByDescending(x => x.LastPrice),
                FilterType.Category => isAsc ? allGoods.OrderBy(x => x.CategoryId) : allGoods.OrderByDescending(x => x.CategoryId)
            };

            //var posts = orderedGoods.Skip(model.Offset).Take(model.Limit).ToList();
            var goodIds = orderedGoods.Skip(model.Offset).Take(model.Limit);

            var goods = _unitOfWork.Repository<Good>().Get(x => goodIds.Select(x => x.Id).Contains(x.Id))
                                                        .Include(x => x.Category)
                                                        //.Include(x=>x.Specification)
                                                        .Include(x => x.Producer)
                                                        .Include(x => x.Shops)
                                                        .IncludeFilter(x => x.Prices.OrderByDescending(c => c.Date).FirstOrDefault())
                                                        .ToList();

            var response = _mapper.Map<List<GoodResponseModel>>(goods);

            return response;
        }

        public async Task<GoodResponseModel> GetGood(int id)
        {
            var good = _unitOfWork.Repository<Good>().Get(x => x.Id == id)
                                                        .Include(x => x.Category)
                                                        .Include(x => x.Specification)
                                                        .Include(x => x.Producer)
                                                        .Include(x => x.Shops)
                                                        .Include(x => x.Prices)
                                                        .FirstOrDefault();

            if (good == null)
                return new GoodResponseModel();

            var response = _mapper.Map<GoodResponseModel>(good);

            return response;
        }
    }
}
