using System.Collections.Generic;

namespace CosmicWorks.Tool.Models
{
    public class Location
    {
        public string type { get; set; } = String.Empty;

        public List<float> coordinates { get; set; } = Enumerable.Empty<float>().ToList();
    }
}
