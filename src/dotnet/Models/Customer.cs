using System.Collections.Generic;

namespace CosmicWorks.Tool.Models
{
    public class Customer : IEntity
    {
        public string id { get; set; } = String.Empty;

        public string type { get; set; } = String.Empty;

        public string customerId { get; set; } = String.Empty;

        public string title { get; set; } = String.Empty;

        public string firstName { get; set; } = String.Empty;

        public string lastName { get; set; } = String.Empty;

        public string emailAddress { get; set; } = String.Empty;

        public string phoneNumber { get; set; } = String.Empty;

        public string creationDate { get; set; } = String.Empty;

        public List<CustomerAddress> addresses { get; set; } = Enumerable.Empty<CustomerAddress>().ToList();

        public Password? password { get; set; }

        public int salesOrderCount { get; set; } = 0;
    }
}
