using System.Collections.Generic;

namespace CosmicWorks.Tool.Models
{
    public class Product : IEntity
    {
        public string id { get; set; } = String.Empty;

        public string categoryId { get; set; } = String.Empty;

        public string categoryName { get; set; } = String.Empty;

        public string sku { get; set; } = String.Empty;

        public string name { get; set; } = String.Empty;

        public string description { get; set; } = String.Empty;

        public double price { get; set; } = 0.0d;

        public List<Tag> tags { get; set; } = Enumerable.Empty<Tag>().ToList();
    }
}
