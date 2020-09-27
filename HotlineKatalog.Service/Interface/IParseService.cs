using System.Threading.Tasks;

namespace HotlineKatalog.Services.Interface
{
    public interface IParseService
    {
        Task<object> Parse();

        Task<object> GetName();

        Task<object> GetPrice();

        Task<object> GetSpecification();
    }
}
