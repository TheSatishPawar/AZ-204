using sqlwebapp.Models;

namespace sqlwebapp.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProducts();
    }
}