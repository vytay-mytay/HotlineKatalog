using HotlineKatalog.DAL.Abstract;
using HotlineKatalog.Domain.Entities;
using HotlineKatalog.Models.Enums;
using HotlineKatalog.Models.InternalModels;
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

        public AddDBService(IUnitOfWork unitOfWork, WebSocketMessageHandler messageHandler)
        {
            _unitOfWork = unitOfWork;
            _messageHandler = messageHandler;
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
                    var sendType = Check(good, price.Value);

                    switch (sendType)
                    {
                        case SendType.Up:
                            await _messageHandler.SendMessageToAllAsync(new WebSocketEventResponseModel()
                            {
                                EventType = WebSocketEventType.PriceChange,
                                Data = new { Change = "Up", OldValue = good.Prices.FirstOrDefault().Value, NewValue = price.Value, GoodId = good.Id, ShopId = priceInternal.Shop.Id }
                            });
                            break;
                        case SendType.Down:
                            await _messageHandler.SendMessageToAllAsync(new WebSocketEventResponseModel()
                            {
                                EventType = WebSocketEventType.PriceChange,
                                Data = new { Change = "Down", OldValue = good.Prices.FirstOrDefault().Value, NewValue = price.Value, GoodId = good.Id, ShopId = priceInternal.Shop.Id }
                            });
                            break;
                    }

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
