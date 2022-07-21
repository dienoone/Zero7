using O7.Core.Models.O7Models.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.Interfaces.O7Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsAsync(string businessId);
        Task<Product> GetProductAsync(int? productId, string businessId);
    }
}
