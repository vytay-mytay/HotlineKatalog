using HotlineKatalog.Models.Enums;
using System;

namespace HotlineKatalog.Models.RequestModels
{
    public class GoodsRequestModel
    {
        public int Limit { get; set; }

        public int Offset { get; set; }

        public int? CategoryId { get; set; }

        public int? ShopId { get; set; }

        public FilterType Filter { get; set; }

        public OrderingType Ordering { get; set; }
    }
}
