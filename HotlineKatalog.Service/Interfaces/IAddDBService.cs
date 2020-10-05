using HotlineKatalog.Domain.Entities;
using HotlineKatalog.Models.InternalModels;
using System.Threading.Tasks;

namespace HotlineKatalog.Services.Interfaces
{
    public interface IAddDBService
    {
        Task<Good> AddToDB(PriceInternalModel price);
    }
}
