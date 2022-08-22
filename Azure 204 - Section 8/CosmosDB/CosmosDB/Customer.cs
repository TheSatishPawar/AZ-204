using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB
{
    public class Customer
    {
        [JsonProperty("id")]
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCity { get; set; }

        public List<Order> Orders { get; set; }
    }
}
