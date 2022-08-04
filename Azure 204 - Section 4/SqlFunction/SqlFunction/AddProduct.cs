using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace SqlFunction
{
    public static class AddProduct
    {
        [FunctionName("AddProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Product product = JsonConvert.DeserializeObject<Product>(requestBody);
            SqlConnection conn = GetConnection();
            conn.Open();

            string statement = "Insert Into Products Values(@param1,@param2,@param3)";

            using (SqlCommand cmd = new SqlCommand(statement, conn))
            {
                cmd.Parameters.Add("@param1", System.Data.SqlDbType.Int).Value = product.ProductID;
                cmd.Parameters.Add("@param2", System.Data.SqlDbType.VarChar,1000).Value = product.ProductName;
                cmd.Parameters.Add("@param3", System.Data.SqlDbType.Int).Value = product.Quantity;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();
            }


                return new OkObjectResult("Product added");
        }

        private static SqlConnection GetConnection()
        {
            string connectionString = "Server=tcp:ssp1.database.windows.net,1433;Initial Catalog=ssp;Persist Security Info=False;User ID=ssp;Password=satishpawar@2022;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            return new SqlConnection(connectionString);
        }
    }
}
