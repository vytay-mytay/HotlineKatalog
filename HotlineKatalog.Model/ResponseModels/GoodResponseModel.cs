using System.Collections.Generic;

namespace HotlineKatalog.Models.ResponseModels
{
    public class GoodResponseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public CategoryResponseModel Category { get; set; }

        public ProducerResponseModel Producer { get; set; }

        public List<PriceResponseModel> Prices { get; set; }

        public SpecificationResponseModel Specification { get; set; }
    }
}
