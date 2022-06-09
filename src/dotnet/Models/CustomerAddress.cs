namespace CosmicWorks.Tool.Models
{
    public class CustomerAddress
    {
        public string addressLine1 { get; set; } = String.Empty;

        public string addressLine2 { get; set; } = String.Empty;

        public string city { get; set; } = String.Empty;

        public string state { get; set; } = String.Empty;

        public string country { get; set; } = String.Empty;

        public string zipCode { get; set; } = String.Empty;

        public Location? location { get; set; }
    }
}
