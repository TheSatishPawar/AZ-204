using Microsoft.Azure.Cosmos;

string cosmosDbEndpointUri = "https://sspcosmosdbtest.documents.azure.com:443/";
string cosmosDbKey = "WUbar3IwsJcMlAaHJMq8RFCORCYaPTcrY3lMOcQBZ48QsI912BCzjt394rX0gkIwOG084xQ7Ozan6zkcUMqlLw==";

//await CreateDatabase("cosmosdarnd");

await CreateContainer("cosmosdarnd", "cosmosdb", "/customerName");

async Task CreateDatabase(string databaseName)
{
    CosmosClient cosmosClient = new CosmosClient(cosmosDbEndpointUri, cosmosDbKey);

    await cosmosClient.CreateDatabaseAsync(databaseName);
    Console.WriteLine("Database created");
}

async Task CreateContainer(string databaseName, string partitionKey, string containerName)
{
    CosmosClient cosmosClient = new CosmosClient(cosmosDbEndpointUri, cosmosDbKey);

    Database database = cosmosClient.GetDatabase(databaseName);

    await database.CreateContainerAsync(new ContainerProperties(partitionKey, containerName));
    Console.WriteLine("Container created");
}