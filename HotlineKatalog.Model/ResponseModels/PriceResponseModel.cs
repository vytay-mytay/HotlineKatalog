using System;

namespace HotlineKatalog.Models.ResponseModels
{
    public class PriceResponseModel
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public string Url { get; set; }

        public DateTime Date { get; set; }

        public ShopResponseModel Shop { get; set; }
    }
}
