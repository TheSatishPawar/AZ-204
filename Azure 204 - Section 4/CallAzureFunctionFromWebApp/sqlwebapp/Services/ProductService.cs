using sqlwebapp.Models;
using System.Data.SqlClient;
using System.Text.Json;

namespace sqlwebapp.Services
{
    public class ProductService : IProductService
    {
        private readonly IConfiguration _configuration;
        public ProductService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnection()
        {
            return "https://ssp-first-funtionapp.azurewebsites.net/api/GetProducts?code=JMRZ1t3u1npCDAOhdQmTd8fwqRqzxGWc_y33q4wqIEdWAzFuMKEaQA==";
        }

        public async Task<List<Product>> GetProducts()
        {
            using(HttpClient client = new HttpClient())
            {
                HttpResponseMessage responseMessage = await client.GetAsync(GetConnection());

                string content = await responseMessage.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<List<Product>>(content);
            }
        }
    }
}
