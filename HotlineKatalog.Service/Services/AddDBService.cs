using AutoMapper;
using HotlineKatalog.DAL.Abstract;
using HotlineKatalog.Domain.Entities;
using HotlineKatalog.Models.Enums;
using HotlineKatalog.Models.InternalModels;
using HotlineKatalog.Models.ResponseModels;
using HotlineKatalog.Services.Interfaces;
using HotlineKatalog.WebSockets.Constants;
using HotlineKatalog.WebSockets.Handlers;
using HotlineKatalog.WebSockets.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace HotlineKatalog.Services.Services
{
    public class AddDBService : IAddDBService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly WebSocketMessageHandler _messageHandler;
        private readonly IMapper _mapper;

        public AddDBService(IUnitOfWork unitOfWork, WebSocketMessageHandler messageHandler, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _messageHandler = messageHandler;
            _mapper = mapper;
        }

        public async Task<Good> AddToDB(PriceInternalModel priceInternal)
        {
            try
            {
                var producer = _unitOfWork.Repository<Producer>().Find(x => x.Name == priceInternal.ProducerName);

                if (producer == null)
                {
                    producer = new Producer()
                    {
                        Name = priceInternal.ProducerName
                    };

                    _unitOfWork.Repository<Producer>().Insert(producer);
                    _unitOfWork.SaveChanges();
                }

                var good = _unitOfWork.Repository<Good>().Get(x => x.Name == priceInternal.Name)
                                                            .Include(x => x.Shops)
                                                            .IncludeFilter(x => x.Prices.OrderByDescending(p => p.Date).FirstOrDefault(f => f.ShopId == priceInternal.Shop.Id))
                                                            .FirstOrDefault();

                var price = new Price()
                {
                    Date = DateTime.UtcNow.Date,
                    ShopId = priceInternal.Shop.Id,
                    Value = priceInternal.Price
                };

                if (good == null)
                {
                    good = new Good()
                    {
                        Name = priceInternal.Name,
                        ProducerId = producer.Id,
                        Specification = new Specification()
                        {
                            Json = JsonConvert.SerializeObject(priceInternal.Specification)
                        }
                    };

                    good.Prices.Add(price);

                    good.Shops.Add(new ShopGood()
                    {
                        ShopId = priceInternal.Shop.Id,
                        Url = priceInternal.Url
                    });
                }
                else if (good.Shops.Any(x => x.ShopId == priceInternal.Shop.Id))
                {
                    var message = new MessageResponseModel()
                    {
                        Type = Check(good, price.Value),
                        Good = _mapper.Map<Good, GoodResponseModel>(good, opt => opt.AfterMap((src, dest) =>
                         {
                             var urls = src.Prices.Select(p => new { Id = p.Id, Url = p.Good.Shops.FirstOrDefault(s => s.GoodId == p.GoodId).Url });

                             dest.Prices.Select(x => x.Url = urls.FirstOrDefault(u => u.Id == x.Id).Url).ToList();
                         })),
                        NewValue = price.Value
                    };

                    if (message.Type != SendType.NotChange)
                        await _messageHandler.SendMessageToAllAsync(new WebSocketEventResponseModel()
                        {
                            EventType = WebSocketEventType.PriceChange,
                            Data = message
                        });

                    good.Prices.Add(price);
                }
                else
                {
                    good.Prices.Add(price);

                    good.Shops.Add(new ShopGood()
                    {
                        ShopId = priceInternal.Shop.Id,
                        Url = priceInternal.Url
                    });
                }

                priceInternal.Category.Goods.Add(good);

                _unitOfWork.SaveChanges();

                return good;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SendType Check(Good good, int value)
        {
            var response = SendType.NotChange;
            if (good.Prices.Any() && good.Prices.FirstOrDefault().Value > value)
                response = SendType.Down;
            else if (good.Prices.Any() && good.Prices.FirstOrDefault().Value < value)
                response = SendType.Up;

            return response;
        }
    }
}
