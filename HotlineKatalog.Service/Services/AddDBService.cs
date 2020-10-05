using HotlineKatalog.DAL.Abstract;
using HotlineKatalog.Domain.Entities;
using HotlineKatalog.Models.InternalModels;
using HotlineKatalog.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HotlineKatalog.Services.Services
{
    public class AddDBService : IAddDBService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddDBService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
