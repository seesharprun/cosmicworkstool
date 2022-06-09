using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace CosmicWorks.Tool
{
    public class Options
    {
        [Option('r', "revision", Default = Revision.v4, HelpText = "Revision of the database[s] to deploy", Required = false)]
        public Revision Revision { get; set; } = Revision.v4;

        [Option('d', "datasets", Default = new Dataset[] { Dataset.customer, Dataset.product }, HelpText = "Selected database[s] to deploy", Required = false, Separator = ':')]
        public IEnumerable<Dataset> Datasets { get; set; } = new List<Dataset> { Dataset.customer, Dataset.product };

        [Option('n', "name", Default = "cosmicworks", HelpText = "Set name for new database", Required = false)]
        public string Name { get; set; } = "cosmicworks";

        [Option('e', "emulator", Default = false, HelpText = "Load data into emulator", Required = false)]
        public bool Emulator { get; set; } = false;

        [Option('p', "endpoint", HelpText = "Set endpoint URL for target account", Required = false)]
        public string? Endpoint { get; set; }

        [Option('k', "key", HelpText = "Set authorization key for target account", Required = false)]
        public string? Key { get; set; }

        [Usage(ApplicationAlias = "cosmicworks")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Import all datasets using the latest version", (object)new Options());
                yield return new Example("Import all datasets into the emulator using the latest version", (object)new Options()
                {
                    Emulator = true
                });
                yield return new Example("Import all datasets into an Azure Cosmos DB account using the latest version", (object)new Options()
                {
                    Endpoint = "https://cosmicworks.documents.azure.com:443/",
                    Key = "Djf5/R+o+4QDU5DE2qyMsEcaGQy67XIw/Jw=="
                });
                yield return new Example("Import all datasets using v4", (object)new Options()
                {
                    Revision = Revision.v4
                });
                yield return new Example("Import only the product dataset using v4", (object)new Options()
                {
                    Revision = Revision.v4,
                    Datasets = (IEnumerable<Dataset>)new Dataset[1]
                  {
            Dataset.product
                  }
                });
                yield return new Example("Import only the customer and product datasets using v4", (object)new Options()
                {
                    Revision = Revision.v4,
                    Datasets = (IEnumerable<Dataset>)new Dataset[2]
                  {
            Dataset.customer,
            Dataset.product
                  }
                });
            }
        }
    }
}
