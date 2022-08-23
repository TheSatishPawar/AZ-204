using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB
{
    public class ChangeFeedProcessorForLease
    {
        private readonly string _cosmosDbEndpointUri;
        private readonly string _cosmosDbKey;
        private readonly string _databaseName;
        private readonly string _containerName;
        private readonly string _monitoredContainerName;
        private readonly string _leaseContainerName;
        public ChangeFeedProcessorForLease(string cosmosDbEndpointUri,string cosmosDbKey, string databaseName, string containerName,
            string monitoredContainerName, string leaseContainerName)
        {
            _cosmosDbEndpointUri = cosmosDbEndpointUri;
            _cosmosDbKey = cosmosDbKey;
            _databaseName = databaseName;
            _containerName = containerName;
            _monitoredContainerName = monitoredContainerName;
            _leaseContainerName = leaseContainerName;
        }
        public async Task StartChangeProcessor()
        {
            CosmosClient cosmosClient = new CosmosClient(_cosmosDbEndpointUri, _cosmosDbKey);
            Container leaseContainer = cosmosClient.GetContainer(_databaseName, _leaseContainerName);

            Microsoft.Azure.Cosmos.ChangeFeedProcessor changeFeedProcessor = cosmosClient.GetContainer(_databaseName, _monitoredContainerName)
                .GetChangeFeedProcessorBuilder<Order>(processorName: "ManageChanges", onChangesDelegate: ManageChanges)
                .WithInstanceName("appHost")
                .WithLeaseContainer(leaseContainer)
                .Build();

            Console.WriteLine("Starting change feed processor");
            await changeFeedProcessor.StartAsync();
            Console.Read();
            await changeFeedProcessor.StopAsync();
            Console.WriteLine("stop change feed processor");


        }

        static async Task ManageChanges(ChangeFeedProcessorContext context,
            IReadOnlyCollection<Order> itemCollection,
            CancellationToken cancellationToken)
        {
            foreach(var item in itemCollection)
            {
                Console.WriteLine("ID {0}", item.id);
                Console.WriteLine("Order ID {0}", item.OrderId);
                Console.WriteLine("Creation Time {0}", item.creationTime);
            }
        }
    }
}
