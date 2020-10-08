using HotlineKatalog.Models.Enums;

namespace HotlineKatalog.Models.ResponseModels
{
    public class MessageResponseModel
    {
        public SendType Type { get; set; }

        public GoodResponseModel Good { get; set; }

        public int NewValue { get; set; }
    }
}
