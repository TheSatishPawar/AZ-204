using CosmosDB;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

string cosmosDbEndpointUri = "--End Point--";
string cosmosDbKey = "--Key--";


string databaseName = "cosmosdbrnd";
string containerName = "cosmosdb";
string partitonKey = "/CustomerName";


// used for feed processor to track item changes in cosmosdb container
 string monitoredContainerName = "cosmosdb";
 string leaseContainerName = "leases";

//await CreateDatabase(databaseName);

//await CreateContainer(databaseName, containerName, partitonKey);

//await AddItem("O1", "Laptop", "C1", 100);
//await AddItem("O2", "Mobile", "C2", 200);
//await AddItem("O3", "Desktop", "C3", 75);
//await AddItem("O4", "Laptop", "C1", 25);

//await ReadItems();

//await ReplaceItem();

//await DeleteItem();

////// Add item array
//await AddArrayItem("C1", "customer1", "NY",
//    new List<Order>()
//    {
//        new Order{OrderId="O1",Category="Laptop",Quantity=100,id=Guid.NewGuid().ToString()},
//        new Order{OrderId="O2",Category="Mobile",Quantity=200,id=Guid.NewGuid().ToString()}
//    });

//await AddArrayItem("C2", "customer2", "MU",
//    new List<Order>()
//    {
//        new Order{OrderId="O3",Category="Desktop",Quantity=75,id=Guid.NewGuid().ToString()}
//    });

//await AddArrayItem("C3", "customer3", "PN",
//    new List<Order>()
//    {
//        new Order{OrderId="O4",Category="Laptop",Quantity=25,id=Guid.NewGuid().ToString()}
//    });

// Read item arrays
//await ReadArrayItems();


//await CallStoredProcedure();


////create item to run create trigger - if quantity is not in item then set as quantity =0 in trigger
//await CreateItem();

//// Calling change feed processor to trak changes in cosmosdb container
await CallChangeFeedProcessor();


async Task CreateDatabase(string databaseName)
{
    CosmosClient cosmosClient = new CosmosClient(cosmosDbEndpointUri, cosmosDbKey);

    await cosmosClient.CreateDatabaseAsync(databaseName);
    Console.WriteLine("Database created");
}

async Task CreateContainer(string databaseName, string containerName, string partitionKey)
{
    CosmosClient cosmosClient = new CosmosClient(cosmosDbEndpointUri, cosmosDbKey);

    Database database = cosmosClient.GetDatabase(databaseName);

    await database.CreateContainerAsync(new ContainerProperties(containerName, partitionKey));
    Console.WriteLine("Container created");
}

async Task AddItem(string orderId, string category, string customerName, int quantity)
{
    CosmosClient cosmosClient = new CosmosClient(cosmosDbEndpointUri, cosmosDbKey);

    Database database = cosmosClient.GetDatabase(databaseName);
    Container container = database.GetContainer(containerName);

    Order order = new Order()
    {
        id = Guid.NewGuid().ToString(),
        OrderId = orderId,
        Category = category,
        CustomerName = customerName,
        Quantity = quantity
    };

    ItemResponse<Order> response = await container.CreateItemAsync<Order>(order);
    Console.WriteLine("Created Order Id {0}", orderId);
    Console.WriteLine("Request Units {0}", response.RequestCharge);
}

async Task ReadItems()
{
    CosmosClient cosmosClient = new CosmosClient(cosmosDbEndpointUri, cosmosDbKey);

    Database database = cosmosClient.GetDatabase(databaseName);
    Container container = database.GetContainer(containerName);

    string sqlQuery = "SELECT c.OrderId,c.Category,c.CustomerName,c.Quantity FROM cosmosdbrnd c";

    QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);

    FeedIterator<Order> feedIterator = container.GetItemQueryIterator<Order>(queryDefinition);

    while (feedIterator.HasMoreResults)
    {
        FeedResponse<Order> orders = await feedIterator.ReadNextAsync();
        foreach (var item in orders)
        {
            Console.WriteLine("Order ID {0}", item.OrderId);
            Console.WriteLine("Category {0}", item.Category);
            Console.WriteLine("CustomerName {0}", item.CustomerName);
            Console.WriteLine("Quantity {0}", item.Quantity);
        }
    }
}

async Task ReplaceItem()
{
    CosmosClient cosmosClient = new CosmosClient(cosmosDbEndpointUri, cosmosDbKey);

    Database database = cosmosClient.GetDatabase(databaseName);
    Container container = database.GetContainer(containerName);

    string orderId = "O1";

    string sqlQuery = $"SELECT c.id, c.OrderId,c.Category,c.CustomerName,c.Quantity FROM cosmosdbrnd c where c.OrderId ='{orderId}'";

    string id = "";
    string CustomerName = "";

    QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);

    FeedIterator<Order> feedIterator = container.GetItemQueryIterator<Order>(queryDefinition);

    while (feedIterator.HasMoreResults)
    {
        FeedResponse<Order> orders = await feedIterator.ReadNextAsync();
        foreach (var order in orders)
        {
            id = order.id;
            CustomerName = order.CustomerName;
        }
    }

    ItemResponse<Order> response = await container.ReadItemAsync<Order>(id, new PartitionKey(CustomerName));

    var item = response.Resource;
    item.Quantity = 150;

    await container.ReplaceItemAsync<Order>(item, id, new PartitionKey(CustomerName));

    Console.WriteLine("Item is updated");
}

async Task DeleteItem()
{
    CosmosClient cosmosClient = new CosmosClient(cosmosDbEndpointUri, cosmosDbKey);

    Database database = cosmosClient.GetDatabase(databaseName);
    Container container = database.GetContainer(containerName);

    string orderId = "O1";

    string sqlQuery = $"SELECT c.id, c.OrderId,c.Category,c.CustomerName,c.Quantity FROM cosmosdbrnd c where c.OrderId ='{orderId}'";

    string id = "";
    string CustomerName = "";

    QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);

    FeedIterator<Order> feedIterator = container.GetItemQueryIterator<Order>(queryDefinition);

    while (feedIterator.HasMoreResults)
    {
        FeedResponse<Order> orders = await feedIterator.ReadNextAsync();
        foreach (var order in orders)
        {
            id = order.id;
            CustomerName = order.CustomerName;
        }
    }

    ItemResponse<Order> response = await container.DeleteItemAsync<Order>(id, new PartitionKey(CustomerName));


    Console.WriteLine("Item is deleted");
}

async Task AddArrayItem(string customerId, string customerName, string customerCity, List<Order> orders)
{
    CosmosClient cosmosClient = new CosmosClient(cosmosDbEndpointUri, cosmosDbKey);

    Database database = cosmosClient.GetDatabase(databaseName);
    Container container = database.GetContainer(containerName);

    Customer customer = new Customer()
    {
        CustomerId = customerId,
        CustomerCity = customerCity,
        CustomerName = customerName,
        Orders = orders
    };

    await container.CreateItemAsync(customer, new PartitionKey(customerName));
    Console.WriteLine("Created CustomerId Id {0}", customerId);
}

async Task ReadArrayItems()
{
    CosmosClient cosmosClient = new CosmosClient(cosmosDbEndpointUri, cosmosDbKey);

    Database database = cosmosClient.GetDatabase(databaseName);
    Container container = database.GetContainer(containerName);

    string sqlQuery = "SELECT * FROM cosmosdbrnd";

    QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);

    FeedIterator<Customer> feedIterator = container.GetItemQueryIterator<Customer>(queryDefinition);

    while (feedIterator.HasMoreResults)
    {
        FeedResponse<Customer> customers = await feedIterator.ReadNextAsync();

        foreach (var item in customers.Resource)
        {
            Console.WriteLine("CustomerId {0}", item.CustomerId);
            Console.WriteLine("CustomerName {0}", item.CustomerName);
            Console.WriteLine("CustomerCity {0}", item.CustomerCity);

            foreach (var order in item.Orders)
            {
                Console.WriteLine("Order ID {0}", order.id);
                Console.WriteLine("Category {0}", order.Category);
                Console.WriteLine("Quantity {0}", order.Quantity);
            }

        }
    }
}

async Task CallStoredProcedure()
{
    CosmosClient cosmosClient = new CosmosClient(cosmosDbEndpointUri, cosmosDbKey);
    Container container = cosmosClient.GetContainer(databaseName, containerName);

    dynamic[] orderItems = new dynamic[]
    {
        new {id=Guid.NewGuid().ToString(),
        OrderId="O1",
        Category="Laptop",
        Quantity=100,
        CustomerName ="C1"},
        new {id=Guid.NewGuid().ToString(),
        OrderId="O2",
        Category="Mobile",
        Quantity=150,
        CustomerName ="C1"},
        new {id=Guid.NewGuid().ToString(),
        OrderId="O3",
        Category="Laptop",
        Quantity=75,
        CustomerName ="C1"}
    };

    var result = await container.Scripts.ExecuteStoredProcedureAsync<string>("createitems", new PartitionKey("C1"), new[] { orderItems });

    Console.WriteLine(result);
}


async Task CreateItem()
{
    CosmosClient cosmosClient = new CosmosClient(cosmosDbEndpointUri, cosmosDbKey);
    Container container = cosmosClient.GetContainer(databaseName, containerName);

    dynamic orderItem =
        new
        {
            id = Guid.NewGuid().ToString(),
            OrderId = "O1",
            Category = "Laptop",
            CustomerName = "C1"
        };


    await container.CreateItemAsync(orderItem, null, new ItemRequestOptions { PreTriggers = new List<string> { "validateItem" } });

    Console.WriteLine("Item inserted");
}


async Task CallChangeFeedProcessor()
{
    var changeFeedProcessor = new ChangeFeedProcessorForLease(cosmosDbEndpointUri, cosmosDbKey, databaseName, containerName, monitoredContainerName, leaseContainerName);
    await changeFeedProcessor.StartChangeProcessor();
}
