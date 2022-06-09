using Microsoft.Azure.Cosmos;
using CommandLine;
using CosmicWorks.Tool.Models;
using Flurl.Http;
using Humanizer;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace CosmicWorks.Tool
{
    internal class Program
    {
        private static string? _endpointUrl;
        private static string? _authorizationKey;

        internal static async Task Main(string[] args)
        {
            ParserResult<Options> parserResult = await Parser.Default.ParseArguments<Options>((IEnumerable<string>)args).WithParsedAsync<Options>(new Func<Options, Task>(Program.RunAsync));
        }

        private static async Task RunAsync(Options options)
        {
            if (options.Emulator)
            {
                _endpointUrl = "https://localhost:8081";
                _authorizationKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            }
            else
            {
                _endpointUrl = String.IsNullOrWhiteSpace(options?.Endpoint) ? Prompt.GetString("Enter your Endpoint Uri:\t") : options.Endpoint;
                _authorizationKey = String.IsNullOrWhiteSpace(options?.Key) ? Prompt.GetString("Enter your Authorization Key:\t") : options.Key;
            }

            Revision revision = options?.Revision ?? Revision.v4;
            string name = options?.Name ?? "cosmicworks";
            IEnumerable<Dataset> datasets = options?.Datasets ?? Enumerable.Empty<Dataset>();

            Console.WriteLine($"Endpoint:\t{_endpointUrl}");
            Console.WriteLine($"Auth Key:\t{_authorizationKey}");
            Console.WriteLine($"Revision:\t{revision}");
            Console.WriteLine("Datasets:" + Environment.NewLine + "\t" + String.Join<Dataset>(Environment.NewLine + "\t", datasets) + Environment.NewLine);
            CosmosClient client = new CosmosClient(
                _endpointUrl,
                _authorizationKey,
                new CosmosClientOptions()
                {
                    AllowBulkExecution = true
                }
            );
            Database database = await client.CreateDatabaseIfNotExistsAsync(name);
            Console.WriteLine("Database:\t[cosmicworks]\tStatus:\tCreated");
            foreach (Dataset dataset in datasets)
            {
                if (revision == Revision.v4)
                {
                    if (dataset == Dataset.product)
                    {
                        await Program.BulkUpsertContent<Product>(database, dataset, "products.json", "/categoryId", (Func<Product, string>)(e => e.categoryId));
                        Console.WriteLine(String.Format("{0}Container:\t[{1}]\tStatus:\tPopulated", Environment.NewLine, Dataset.product));
                    }
                    else if (dataset == Dataset.customer)
                    {
                        await Program.BulkUpsertContent<Customer>(database, dataset, "customers.json", "/customerId", (Func<Customer, string>)(e => e.customerId));
                        Console.WriteLine(String.Format("{0}Container:\t[{1}]\tStatus:\tPopulated", Environment.NewLine, Dataset.customer));
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Revision and names do not map to known values");
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Revision and names do not map to known values");
                }
            }
        }

        private static async Task BulkUpsertContent<T>(
          Database database,
          Dataset dataset,
          string file,
          string partitionKey,
          Func<T, string> partitionKeyValue)
          where T : IEntity
        {
            int parallelism = 200;
            string containerName = dataset.ToString().ToLower().Pluralize();
            Container container = await database.CreateContainerIfNotExistsAsync(containerName ?? "", partitionKey, new int?(4000));
            Console.WriteLine("Container:\t[" + containerName + "]\tStatus:\tReady" + Environment.NewLine);


            string filePath = Path.Combine(
                Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().Location
                )!,
                "Data",
                file
            );

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var entities = await JsonSerializer.DeserializeAsync<IEnumerable<T>>(stream);

            Console.WriteLine($"{dataset} Items Count:\t{entities?.Count()}");

            List<Task> concurrentOperations = new List<Task>(parallelism);
            foreach (T entity in entities ?? Enumerable.Empty<T>())
            {
                if (entity is not null)
                {
                    concurrentOperations.Add(
                        container.UpsertItemAsync<T>(
                            entity,
                            new PartitionKey?(new PartitionKey(partitionKeyValue(entity)))
                        ).ContinueWith(
                            (Action<Task<ItemResponse<T>>>)(response =>
                                {
                                    if (response.IsFaulted)
                                        Console.WriteLine(String.Format("Crash:\t[{0}]\tContainer:{1}\tReason:{2}", entity.id, containerName, response.Exception.Message));
                                    else
                                        Console.WriteLine(String.Format("Entity:\t[{0}]\tContainer:{1}\tStatus:\t{2}", response.Result.Resource.id, containerName, response.Status));
                                }
                            )
                        )
                    );
                }
            }

            await Task.WhenAll((IEnumerable<Task>)concurrentOperations);
        }
    }
}
