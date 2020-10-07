using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace HotlineKatalog.Models.Enums
{
    //[JsonConverter(typeof(StringEnumConverter))]
    public enum OrderingType
    {
        Asc,
        Desc
    }
}
