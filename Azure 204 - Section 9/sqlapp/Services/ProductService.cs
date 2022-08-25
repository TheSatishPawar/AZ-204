using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using sqlapp.Models;
using System.Data.SqlClient;

namespace sqlapp.Services
{
    public class ProductService : IProductService
    {
        private readonly IConfiguration _configuration;
        public ProductService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            string teneantId = "--teneantId--";
            string clinetId = "--clinetId--";
            string clientSecret = "--clientSecret--";

            ClientSecretCredential clientSecretCredential = new ClientSecretCredential(teneantId, clinetId, clientSecret);
            string keyValutUri = "--keyValutUri--";
            string secretName = "sqldbconnection";

            SecretClient secretClient = new SecretClient(new Uri(keyValutUri), clientSecretCredential);

            var secret = secretClient.GetSecret(secretName);

            //// Resource group -> select Key Vault from list -> Access policies -> Select Application Name and set Secret Permissions as "Get" and save

            return new SqlConnection(secret.Value.Value);
        }

        public List<Product> GetProducts()
        {
            SqlConnection conn = GetConnection();
            List<Product> products = new List<Product>();
            string statement = "SELECT ProductID,ProductName,Quantity from Products";

            conn.Open();
            SqlCommand cmd = new SqlCommand(statement, conn);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Product product = new Product()
                    {
                        ProductID = reader.GetInt32(0),
                        ProductName = reader.GetString(1),
                        Quantity = reader.GetInt32(2)
                    };

                    products.Add(product);
                }
            }
            conn.Close();
            return products;
        }
    }
}
