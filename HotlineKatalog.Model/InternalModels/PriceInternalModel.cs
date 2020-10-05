using HotlineKatalog.Domain.Entities;
using System.Collections.Generic;

namespace HotlineKatalog.Models.InternalModels
{
    public class PriceInternalModel
    {
        public string Name { get; set; }

        public Category Category { get; set; }

        public string ProducerName { get; set; }

        public Shop Shop { get; set; }

        public int Price { get; set; }

        public Dictionary<string, string> Specification { get; set; }

        public string Url { get; set; }
    }
}
